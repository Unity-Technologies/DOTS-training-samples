using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using CustomAttributeNamedArgument = Mono.Cecil.CustomAttributeNamedArgument;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodBody = Mono.Cecil.Cil.MethodBody;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace Unity.Entities.CodeGen
{
    internal class JobStructForLambdaJob
    {
        public LambdaJobDescriptionConstruction LambdaJobDescriptionConstruction { get; }
        public TypeDefinition TypeDefinition;
        public MethodDefinition ScheduleTimeInitializeMethod;
        public MethodDefinition RunWithoutJobSystemMethod;
        public FieldDefinition RunWithoutJobSystemDelegateFieldBurst;
        public FieldDefinition RunWithoutJobSystemDelegateFieldNoBurst;
        public MethodDefinition ReadFromDisplayClassMethod;
        public MethodDefinition WriteToDisplayClassMethod;
        public MethodDefinition DeallocateOnCompletionMethod;
        public MethodDefinition ExecuteMethod;
        public FieldDefinition SystemInstanceField;
        
        public MethodDefinition[] ClonedMethods;
        public MethodDefinition ClonedLambdaBody => ClonedMethods.First();
        public Dictionary<FieldDefinition, FieldDefinition> ClonedFields;
        
        static Type InterfaceTypeFor(LambdaJobDescriptionKind lambdaJobDescriptionKind)
        {
            switch (lambdaJobDescriptionKind)
            {
                case LambdaJobDescriptionKind.Entities:
                case LambdaJobDescriptionKind.Chunk:
                    return typeof(IJobChunk);
                case LambdaJobDescriptionKind.Job:
                    return typeof(IJob);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        MethodDefinition AddMethod(MethodDefinition method)
        {
            TypeDefinition.Methods.Add(method);
            return method;
        }
        
        FieldDefinition AddField(FieldDefinition field)
        {
            TypeDefinition.Fields.Add(field);
            return field;
        }

        
        public static JobStructForLambdaJob CreateNewJobStruct(LambdaJobDescriptionConstruction lambdaJobDescriptionConstruction)
        {
            return new JobStructForLambdaJob(lambdaJobDescriptionConstruction);
        }
        
        JobStructForLambdaJob(LambdaJobDescriptionConstruction lambdaJobDescriptionConstruction2)
        {
            LambdaJobDescriptionConstruction = lambdaJobDescriptionConstruction2;
            var containingMethod = LambdaJobDescriptionConstruction.ContainingMethod;

            if (containingMethod.DeclaringType.NestedTypes.Any(t => t.Name == LambdaJobDescriptionConstruction.Name))
                UserError.DC0003(LambdaJobDescriptionConstruction.Name, containingMethod,LambdaJobDescriptionConstruction.ScheduleOrRunInvocationInstruction).Throw();

            var moduleDefinition = containingMethod.Module;

            var typeAttributes = TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed |
                                 TypeAttributes.AnsiClass | TypeAttributes.SequentialLayout |
                                 TypeAttributes.NestedPrivate;
            
            TypeDefinition = new TypeDefinition(containingMethod.DeclaringType.Namespace,LambdaJobDescriptionConstruction.Name, typeAttributes,moduleDefinition.ImportReference(typeof(ValueType)))
            {
                DeclaringType = containingMethod.DeclaringType,
                Interfaces = { new InterfaceImplementation(moduleDefinition.ImportReference(InterfaceTypeFor(LambdaJobDescriptionConstruction.Kind)))}
            };

            containingMethod.DeclaringType.NestedTypes.Add(TypeDefinition);


            if (LambdaJobDescriptionConstruction.LambdaWasEmittedAsInstanceMethodOnContainingType)
            {
                //if you capture no locals, but you do use a field/method on the componentsystem, the lambda gets emitted as an instance method on the component system
                //this is inconvenient for us. To make the rest of our code not have to deal with this case, we will emit an OriginalLambda method on our job type, that calls
                //the lambda as it is emitted as an instance method on the component system.  See EntitiesForEachNonCapturingInvokingInstanceMethod test for more details.
                //example:
                //https://sharplab.io/#v2:CYLg1APgAgTAjAWAFBQMwAJboMLoN7LpGYZQAs6AsgJ4CSAdgM4AuAhvQMYCmlXzAFgHtgACgCU+AL6FiMomkwVK4/HOJEAogA8uHAK7MuIlQF4AfFTpM2nHnyGixYgNxrpSdVDgA2Rem26BkZeMOisYnjukkA==
                
                MakeOriginalLambdaMethodThatRelaysToInstanceMethodOnComponentSystem();
            }
            else
                CloneLambdaMethodAndItsLocalMethods();

            ApplyFieldAttributes();

            var lambdaParameterValueProviderInformations = MakeLambdaParameterValueProviderInformations();

            MakeDeallocateOnCompletionMethod();
            
            ExecuteMethod = MakeExecuteMethod(lambdaParameterValueProviderInformations);
            
            ScheduleTimeInitializeMethod = AddMethod(MakeScheduleTimeInitializeMethod(lambdaParameterValueProviderInformations));
            
            AddRunWithoutJobSystemMembers();
            
            ApplyBurstAttributeIfRequired();
        }

        private void MakeDeallocateOnCompletionMethod()
        {
            //we only have to clean up ourselves, in Run execution mode.
            if (LambdaJobDescriptionConstruction.ExecutionMode != ExecutionMode.Run)
                return;

            var fieldsToDeallocate =
                LambdaJobDescriptionConstruction.InvokedConstructionMethods
                    .Where(m => m.MethodName ==
                                nameof(LambdaJobDescriptionConstructionMethods.WithDeallocateOnJobCompletion))
                    .Select(ca => ca.Arguments.Single())
                    .Cast<FieldDefinition>()
                    .ToList();

            if (!fieldsToDeallocate.Any())
                return;

            DeallocateOnCompletionMethod = AddMethod(new MethodDefinition("DeallocateOnCompletionMethod",MethodAttributes.Public, TypeSystem.Void));
            var ilProcessor = DeallocateOnCompletionMethod.Body.GetILProcessor();
            
            foreach (var fieldToDeallocate in fieldsToDeallocate)
            {
                var clonedField = ClonedFields[fieldToDeallocate];
                ilProcessor.Emit(OpCodes.Ldarg_0);
                ilProcessor.Emit(OpCodes.Ldflda, clonedField);
                
                var disposeReference = new MethodReference("Dispose", TypeSystem.Void, clonedField.FieldType){HasThis = true};
                var disposeMethod = ImportReference(disposeReference);
                //todo: check for null
                
                ilProcessor.Emit(OpCodes.Call, disposeMethod);
            }
            ilProcessor.Emit(OpCodes.Ret);
        }

        

        void AddRunWithoutJobSystemMembers()
        {
            if (LambdaJobDescriptionConstruction.ExecutionMode != ExecutionMode.Run)
                return;

            RunWithoutJobSystemMethod = CreateRunWithoutJobSystemMethod(TypeDefinition);

            RunWithoutJobSystemDelegateFieldNoBurst = AddField(new FieldDefinition("s_RunWithoutJobSystemDelegateFieldNoBurst", FieldAttributes.Static,ImportReference(ExecuteDelegateType)));

            if (LambdaJobDescriptionConstruction.UsesBurst)
                RunWithoutJobSystemDelegateFieldBurst = AddField(new FieldDefinition("s_RunWithoutJobSystemDelegateFieldBurst", FieldAttributes.Static,ImportReference(ExecuteDelegateType)));
        }

        public Type ExecuteDelegateType => LambdaJobDescriptionConstruction.Kind == LambdaJobDescriptionKind.Job 
            ? typeof(InternalCompilerInterface.JobRunWithoutJobSystemDelegate) 
            : typeof(InternalCompilerInterface.JobChunkRunWithoutJobSystemDelegate);

        TypeReference ImportReference(Type t) => TypeDefinition.Module.ImportReference(t);
        TypeReference ImportReference(TypeReference t) => TypeDefinition.Module.ImportReference(t);
        MethodReference ImportReference(MethodReference m) => TypeDefinition.Module.ImportReference(m);
        MethodReference ImportReference(MethodInfo m) => TypeDefinition.Module.ImportReference(m);
        TypeSystem TypeSystem => TypeDefinition.Module.TypeSystem;

        LambdaParameterValueInformations MakeLambdaParameterValueProviderInformations()
        {
            switch (LambdaJobDescriptionConstruction.Kind)
            {
                case LambdaJobDescriptionKind.Entities:
                    return LambdaParameterValueInformations.For(this);
                case LambdaJobDescriptionKind.Job:
                    return null;
                case LambdaJobDescriptionKind.Chunk:
                    var allUsedParametersOfEntitiesForEachInvocations = ClonedMethods.SelectMany(
                            m =>
                                m.Body.Instructions.Where(IsChunkEntitiesForEachInvocation).Select(i =>
                                    (m,
                                        LambdaJobDescriptionConstruction.AnalyzeForEachInvocationInstruction(m, i)
                                            .MethodLambdaWasEmittedAs)))
                        .SelectMany(m_and_dem => m_and_dem.MethodLambdaWasEmittedAs.Parameters.Select(p => (m_and_dem.m, p)))
                        .ToArray();

                    return LambdaParameterValueInformations.For(this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private MethodDefinition MakeExecuteMethod(LambdaParameterValueInformations lambdaParameterValueInformations)
        {
            switch (LambdaJobDescriptionConstruction.Kind)
            {
                case LambdaJobDescriptionKind.Entities:
                    return MakeExecuteMethod_Entities(lambdaParameterValueInformations);
                case LambdaJobDescriptionKind.Job:
                    return MakeExecuteMethod_Job();
                case LambdaJobDescriptionKind.Chunk:
                    return MakeExecuteMethod_Chunk(lambdaParameterValueInformations);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ApplyFieldAttributes()
        {
            //.Run mode doesn't go through the jobsystem, so there's no point in making all these attributes to explain the job system what everything means.
            if (LambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Run)
                return;
            
            foreach (var (methodName, attributeType) in ConstructionMethodsThatCorrespondToFieldAttributes)
            {
                foreach (var constructionMethod in LambdaJobDescriptionConstruction.InvokedConstructionMethods.Where(m => m.MethodName == methodName))
                {
                    if (constructionMethod.Arguments.Single() is FieldDefinition fieldDefinition)
                    {
                        var correspondingJobField = ClonedFields.Values.Single(f => f.Name == fieldDefinition.Name);
                        correspondingJobField.CustomAttributes.Add(new CustomAttribute(TypeDefinition.Module.ImportReference(attributeType.GetConstructor(Array.Empty<Type>()))));
                        continue;
                    }

                    UserError.DC0012(LambdaJobDescriptionConstruction.ContainingMethod, constructionMethod).Throw();
                }
            }
        }

        private  MethodDefinition CreateRunWithoutJobSystemMethod(TypeDefinition newJobStruct)
        {
            var moduleDefinition = newJobStruct.Module;
            var result =
                new MethodDefinition("RunWithoutJobSystem", MethodAttributes.Public | MethodAttributes.Static, moduleDefinition.TypeSystem.Void)
                {
                    HasThis = false,
                    Parameters =
                    {
                        new ParameterDefinition("jobData", ParameterAttributes.None,new PointerType(moduleDefinition.TypeSystem.Void)),
                    },
                };
            newJobStruct.Methods.Add(result);

            var ilProcessor = result.Body.GetILProcessor();
            if (LambdaJobDescriptionConstruction.Kind != LambdaJobDescriptionKind.Job)
            {
                result.Parameters.Insert(0,new ParameterDefinition("archetypeChunkIterator", ParameterAttributes.None,new PointerType(moduleDefinition.ImportReference(typeof(ArchetypeChunkIterator)))));
                ilProcessor.Emit(OpCodes.Ldarg_1);
                ilProcessor.Emit(OpCodes.Call,moduleDefinition.ImportReference(typeof(UnsafeUtilityEx).GetMethod(nameof(UnsafeUtilityEx.AsRef),BindingFlags.Public | BindingFlags.Static)).MakeGenericInstanceMethod(newJobStruct));
                ilProcessor.Emit(OpCodes.Ldarg_0);
                ilProcessor.Emit(OpCodes.Call,moduleDefinition.ImportReference(typeof(JobChunkExtensions).GetMethod(nameof(JobChunkExtensions.RunWithoutJobs),BindingFlags.Public | BindingFlags.Static)).MakeGenericInstanceMethod(newJobStruct));
                ilProcessor.Emit(OpCodes.Ret);
                return result;
            }
            else
            {
                ilProcessor.Emit(OpCodes.Ldarg_0);
                ilProcessor.Emit(OpCodes.Call,ExecuteMethod);
                ilProcessor.Emit(OpCodes.Ret);
                return result;    
            }
        }

        void CloneLambdaMethodAndItsLocalMethods()
        {
            
            var displayClassExecuteMethodAndItsLocalMethods = CecilHelpers.FindUsedInstanceMethodsOnSameType(LambdaJobDescriptionConstruction.MethodLambdaWasEmittedAs).Prepend(LambdaJobDescriptionConstruction.MethodLambdaWasEmittedAs).ToList();

            if (LambdaJobDescriptionConstruction.ExecutionMode != ExecutionMode.Run)
                VerifyClosureFunctionDoesNotWriteToCapturedVariable(displayClassExecuteMethodAndItsLocalMethods);
            
            (ClonedMethods, ClonedFields) = CecilHelpers.CloneClosureExecuteMethodAndItsLocalFunctions(displayClassExecuteMethodAndItsLocalMethods, TypeDefinition, "OriginalLambdaBody");

            if (LambdaJobDescriptionConstruction.DelegateProducingSequence.CapturesLocals)
            {
                ReadFromDisplayClassMethod = AddMethodToTransferFieldsWithDisplayClass("ReadFromDisplayClass",TransferDirection.DisplayClassToJob);
                if (LambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Run)
                    WriteToDisplayClassMethod = AddMethodToTransferFieldsWithDisplayClass("WriteToDisplayClass", TransferDirection.JobToDisplayClass);
            }

            //todo: ressurect
            //ApplyPostProcessingOnJobCode(clonedMethods, providerInformations);

            VerifyDisplayClassFieldsAreValid();
        }

        private void MakeOriginalLambdaMethodThatRelaysToInstanceMethodOnComponentSystem()
        {
            SystemInstanceField = new FieldDefinition("hostInstance", FieldAttributes.Public,
                LambdaJobDescriptionConstruction.ContainingMethod.DeclaringType);
            TypeDefinition.Fields.Add(SystemInstanceField);

            var fakeClonedLambdaBody = new MethodDefinition("OriginalLambdaBody", MethodAttributes.Public, TypeSystem.Void);
            foreach (var p in LambdaJobDescriptionConstruction.MethodLambdaWasEmittedAs.Parameters)
            {
                fakeClonedLambdaBody.Parameters.Add(new ParameterDefinition(p.Name ?? "p" + p.Index, p.Attributes,
                    p.ParameterType));
            }

            var ilProcessor = fakeClonedLambdaBody.Body.GetILProcessor();
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldfld, SystemInstanceField);
            for (int i = 0; i != fakeClonedLambdaBody.Parameters.Count; i++)
                ilProcessor.Emit(OpCodes.Ldarg, i + 1);
            ilProcessor.Emit(OpCodes.Callvirt, LambdaJobDescriptionConstruction.MethodLambdaWasEmittedAs);
            ilProcessor.Emit(OpCodes.Ret);

            TypeDefinition.Methods.Add(fakeClonedLambdaBody);
            ClonedMethods = new[] {fakeClonedLambdaBody};
        }

        private void VerifyDisplayClassFieldsAreValid()
        {
            if (LambdaJobDescriptionConstruction.AllowReferenceTypes)
                return;
            
            foreach (var field in ClonedFields.Values)
            {
                var typeDefinition = field.FieldType.Resolve();
                if (typeDefinition.TypeReferenceEquals(LambdaJobDescriptionConstruction.ContainingMethod.DeclaringType))
                {
                    foreach (var method in ClonedMethods)
                    {
                        var thisLoadingInstructions = method.Body.Instructions.Where(i => i.Operand is FieldReference fr && fr.FieldType.TypeReferenceEquals(typeDefinition));

                        foreach (var thisLoadingInstruction in thisLoadingInstructions)
                        {
                            var next = thisLoadingInstruction.Next;
                            if (next.Operand is FieldReference fr)
                                UserError.DC0001(method, next, fr).Throw();
                        }
                    }
                }

                if (typeDefinition.IsDelegate())
                    continue;

                if (!typeDefinition.IsValueType)
                {
                    foreach (var clonedMethod in ClonedMethods)
                    {
                        var methodInvocations = clonedMethod.Body.Instructions.Where(i => i.Operand is MethodReference mr && mr.HasThis);
                        foreach(var methodInvocation in methodInvocations)
                        {
                            var pushThisInstruction = CecilHelpers.FindInstructionThatPushedArg(clonedMethod, 0, methodInvocation);
                            if (pushThisInstruction == null)
                                continue;
                            if (pushThisInstruction.Operand is FieldReference fr && fr.FieldType.TypeReferenceEquals(typeDefinition))
                                
                                UserError.DC0002(clonedMethod, methodInvocation, (MethodReference) methodInvocation.Operand).Throw();
                        }
                    }

                    //todo: we need a better way to detect this, and a much better error message, but let's start with this stopgap version
                    //since it's already much better than the generic DC0004 we would otherwise have thrown below.
                    if (field.Name.Contains("_locals"))
                        UserError.DC0022(LambdaJobDescriptionConstruction.ContainingMethod,LambdaJobDescriptionConstruction.WithCodeInvocationInstruction).Throw();
                    
                    UserError.DC0004(LambdaJobDescriptionConstruction.ContainingMethod,LambdaJobDescriptionConstruction.WithCodeInvocationInstruction, field).Throw();
                }
            }
        }
        

        static bool IsChunkEntitiesForEachInvocation(Instruction instruction)
        {
            if (!(instruction.Operand is MethodReference mr))
                return false;
            return mr.Name == nameof(LambdaJobChunkDescriptionConstructionMethods.ForEach) && mr.DeclaringType.Name == nameof(LambdaForEachDescriptionConstructionMethods);
        }

        private void ApplyPostProcessingOnJobCode(MethodDefinition[] methodUsedByLambdaJobs,
            LambdaParameterValueInformations lambdaParameterValueInformations)
        {
            var forEachInvocations = new List<(MethodDefinition, Instruction)>();
            var methodDefinition = methodUsedByLambdaJobs.First();
            forEachInvocations.AddRange(methodDefinition.Body.Instructions.Where(IsChunkEntitiesForEachInvocation).Select(i => (methodDefinition, i)));

            foreach (var methodUsedByLambdaJob in methodUsedByLambdaJobs)
            {
                var methodBody = methodUsedByLambdaJob.Body;

                var displayClassVariable = methodBody.Variables.SingleOrDefault(v => v.VariableType.Name.Contains("DisplayClass"));
                if (displayClassVariable != null)
                {
                    TypeDefinition displayClass = displayClassVariable.VariableType.Resolve();
                    bool allDelegatesAreGuaranteedNotToOutliveMethod =
                        displayClass.IsValueType ||
                        CecilHelpers.AllDelegatesAreGuaranteedNotToOutliveMethodFor(methodUsedByLambdaJob);

                    if (!displayClass.IsValueType && allDelegatesAreGuaranteedNotToOutliveMethod)
                    {
                        CecilHelpers.PatchMethodThatUsedDisplayClassToTreatItAsAStruct(methodBody,
                            displayClassVariable, displayClass);
                        CecilHelpers.PatchDisplayClassToBeAStruct(displayClass);
                    }
                }
            }

            int counter = 1;
            foreach (var (methodUsedByLambdaJob, instruction) in forEachInvocations)
            {
                var methodBody = methodUsedByLambdaJob.Body;
                var (ldFtn, newObj) = FindClosureCreatingInstructions(methodBody, instruction);

                var newType = new TypeDefinition("", "InlineEntitiesForEachInvocation" + counter++, TypeAttributes.NestedPublic | TypeAttributes.SequentialLayout,methodUsedByLambdaJob.Module.ImportReference(typeof(ValueType)))
                {
                    DeclaringType = methodUsedByLambdaJob.DeclaringType
                };
                methodUsedByLambdaJob.DeclaringType.NestedTypes.Add(newType);

                CloneLambdaMethodAndItsLocalMethods();

                var iterateEntitiesMethod = CreateIterateEntitiesMethod(lambdaParameterValueInformations);

                var variable = new VariableDefinition(newType);
                methodBody.Variables.Add(variable);

                InstructionExtensions.MakeNOP(ldFtn.Previous);
                InstructionExtensions.MakeNOP(ldFtn);
                newObj.OpCode = OpCodes.Ldnull;
                newObj.Operand = null;

                var displayClassVariable = methodBody.Variables.SingleOrDefault(v => v.VariableType.Name.Contains("DisplayClass"));
                if (displayClassVariable == null)
                    continue;
                var ilProcessor = methodBody.GetILProcessor();

                ilProcessor.InsertAfter(instruction, new List<Instruction>
                {
                    //no need to drop the delegate from the stack, because we just removed the function that placed it on the stack in the first place.
                    //do not drop the description from the stack, as the original method returns it, and we want to maintain stack behaviour.

                    //call our new method
                    Instruction.Create(OpCodes.Ldloca, variable),
                    Instruction.Create(OpCodes.Initobj, newType),

                    Instruction.Create(OpCodes.Ldloca, variable),
                    Instruction.Create(OpCodes.Ldloca, displayClassVariable),
                    Instruction.Create(OpCodes.Call, ReadFromDisplayClassMethod),


                    Instruction.Create(OpCodes.Ldloca, variable),
                    Instruction.Create(OpCodes.Ldarga,methodBody.Method.Parameters.First(p=>p.ParameterType.Name == nameof(ArchetypeChunk))),
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Ldfld,lambdaParameterValueInformations._runtimesField),
                    Instruction.Create(OpCodes.Call, (MethodReference) iterateEntitiesMethod),
                });

#if ENABLE_DOTS_COMPILER_CHUNKS
                var chunkEntitiesInvocation = LambdaJobDescriptionConstruction.FindInstructionThatPushedArg(methodBody.Method, 0, instruction);
                if (chunkEntitiesInvocation.Operand is MethodReference mr && mr.Name == "get_"+nameof(ArchetypeChunk.Entities) && mr.DeclaringType.Name == nameof(ArchetypeChunk))
                    CecilHelpers.EraseMethodInvocationFromInstructions(ilProcessor, chunkEntitiesInvocation);
#endif

                CecilHelpers.EraseMethodInvocationFromInstructions(ilProcessor, instruction);
            }
        }

        private MethodDefinition MakeScheduleTimeInitializeMethod(LambdaParameterValueInformations lambdaParameterValueInformations)
        {
            var scheduleTimeInitializeMethod =
                new MethodDefinition("ScheduleTimeInitialize", MethodAttributes.Public, TypeDefinition.Module.TypeSystem.Void)
                {
                    HasThis = true,
                    Parameters =
                    {
                        new ParameterDefinition("componentSystem", ParameterAttributes.None,LambdaJobDescriptionConstruction.ContainingMethod.DeclaringType),
                    },
                };

            if (ReadFromDisplayClassMethod != null)
                scheduleTimeInitializeMethod.Parameters.Add(ReadFromDisplayClassMethod.Parameters.Last());

            lambdaParameterValueInformations?.EmitInvocationToScheduleTimeInitializeIntoJobChunkScheduleTimeInitialize(scheduleTimeInitializeMethod);

            var scheduleIL = scheduleTimeInitializeMethod.Body.GetILProcessor();

            if (ReadFromDisplayClassMethod != null)
            {
                scheduleIL.Emit(OpCodes.Ldarg_0);
                scheduleIL.Emit(OpCodes.Ldarg_2);
                scheduleIL.Emit(OpCodes.Call, ReadFromDisplayClassMethod);
            }

            if (SystemInstanceField != null)
            {
                scheduleIL.Emit(OpCodes.Ldarg_0);
                scheduleIL.Emit(OpCodes.Ldarg_1);
                scheduleIL.Emit(OpCodes.Stfld, SystemInstanceField);
            }
            
            scheduleIL.Emit(OpCodes.Ret);
            return scheduleTimeInitializeMethod;
        }

        private  MethodDefinition MakeExecuteMethod_Job()
        {
            var executeMethod = CecilHelpers.AddMethodImplementingInterfaceMethod(TypeDefinition.Module, 
                TypeDefinition, typeof(IJob).GetMethod(nameof(IJob.Execute)));
            
            var executeIL = executeMethod.Body.GetILProcessor();
            executeIL.Emit(OpCodes.Ldarg_0);
            executeIL.Emit(OpCodes.Call, ClonedLambdaBody);
            EmitCallToDeallocateOnCompletion(executeIL);
            executeIL.Emit(OpCodes.Ret);
            return executeMethod;
        }

        private MethodDefinition MakeExecuteMethod_Chunk(LambdaParameterValueInformations lambdaParameterValueInformations)
        {
            var executeMethod = CecilHelpers.AddMethodImplementingInterfaceMethod(TypeDefinition.Module, 
                TypeDefinition, typeof(IJobChunk).GetMethod(nameof(IJobChunk.Execute)));

            lambdaParameterValueInformations.EmitInvocationToPrepareToRunOnEntitiesInIntoJobChunkExecute(executeMethod);

            var executeIL = executeMethod.Body.GetILProcessor();

            executeIL.Emit(OpCodes.Ldarg_0);
            executeIL.Emit(OpCodes.Ldarg_1);
            executeIL.Emit(OpCodes.Ldarg_2);
            executeIL.Emit(OpCodes.Ldarg_3);
            executeIL.Emit(OpCodes.Call, ClonedLambdaBody);
            EmitCallToDeallocateOnCompletion(executeIL);
            executeIL.Emit(OpCodes.Ret);
            return executeMethod;
        }

        private MethodDefinition MakeExecuteMethod_Entities(LambdaParameterValueInformations providerInformations)
        {
            var executeMethod = CecilHelpers.AddMethodImplementingInterfaceMethod(TypeDefinition.Module, 
                TypeDefinition, typeof(IJobChunk).GetMethod(nameof(IJobChunk.Execute)));

            providerInformations.EmitInvocationToPrepareToRunOnEntitiesInIntoJobChunkExecute(executeMethod);

            var ilProcessor = executeMethod.Body.GetILProcessor();

            var iterateOnEntitiesMethod = CreateIterateEntitiesMethod(providerInformations);

            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldarga,1);

            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Ldfld,providerInformations._runtimesField);

            ilProcessor.Emit(OpCodes.Call, iterateOnEntitiesMethod);

            EmitCallToDeallocateOnCompletion(ilProcessor);
            
            ilProcessor.Emit(OpCodes.Ret);

            return executeMethod;
        }

        private void EmitCallToDeallocateOnCompletion(ILProcessor ilProcessor)
        {
            if (DeallocateOnCompletionMethod == null)
                return;
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Call, DeallocateOnCompletionMethod);
        }

        private MethodDefinition CreateIterateEntitiesMethod(LambdaParameterValueInformations lambdaParameterValueInformations)
        {
            var iterateEntitiesMethod = new MethodDefinition("IterateEntities", MethodAttributes.Public,TypeSystem.Void)
            {
                Parameters =
                {
                    new ParameterDefinition("chunk", ParameterAttributes.None, new ByReferenceType(ImportReference(typeof(ArchetypeChunk)))),
                    new ParameterDefinition("runtimes", ParameterAttributes.None, new ByReferenceType(lambdaParameterValueInformations.RuntimesType)),
                }
            };

            TypeDefinition.Methods.Add(iterateEntitiesMethod);

            var ilProcessor = iterateEntitiesMethod.Body.GetILProcessor();

            var loopTerminator = new VariableDefinition(TypeSystem.Int32);
            iterateEntitiesMethod.Body.Variables.Add(loopTerminator);
            ilProcessor.Emit(OpCodes.Ldarg_1);
            ilProcessor.Emit(OpCodes.Call, ImportReference(typeof(ArchetypeChunk).GetMethod("get_"+nameof(ArchetypeChunk.Count))));
            ilProcessor.Emit(OpCodes.Stloc, loopTerminator);

            var loopCounter = new VariableDefinition(TypeSystem.Int32);
            iterateEntitiesMethod.Body.Variables.Add(loopCounter);
            ilProcessor.Emit(OpCodes.Ldc_I4_0);
            ilProcessor.Emit(OpCodes.Stloc, loopCounter);

            var beginLoopInstruction = Instruction.Create(OpCodes.Ldloc, loopCounter);
            ilProcessor.Append(beginLoopInstruction);
            ilProcessor.Emit(OpCodes.Ldloc, loopTerminator);
            ilProcessor.Emit(OpCodes.Ceq);

            var exitDestination = Instruction.Create(OpCodes.Nop);
            ilProcessor.Emit(OpCodes.Brtrue, exitDestination);

            ilProcessor.Emit(OpCodes.Ldarg_0);
            foreach (var parameterDefinition in ClonedLambdaBody.Parameters)
                lambdaParameterValueInformations.EmitILToLoadValueForParameterOnStack(parameterDefinition, ilProcessor,loopCounter);

            ilProcessor.Emit(OpCodes.Call, ClonedLambdaBody);

            ilProcessor.Emit(OpCodes.Ldloc, loopCounter);
            ilProcessor.Emit(OpCodes.Ldc_I4_1);
            ilProcessor.Emit(OpCodes.Add);
            ilProcessor.Emit(OpCodes.Stloc, loopCounter);

            ilProcessor.Emit(OpCodes.Br, beginLoopInstruction);
            ilProcessor.Append(exitDestination);
            ilProcessor.Emit(OpCodes.Ret);
            return iterateEntitiesMethod;
        }

        private void ApplyBurstAttributeIfRequired()
        {
            if (!LambdaJobDescriptionConstruction.UsesBurst)
                return;
            
            var module = TypeDefinition.Module;
            var burstCompileAttributeConstructor = AttributeConstructorReferenceFor(typeof(BurstCompileAttribute), module);

            CustomAttributeNamedArgument CustomAttributeNamedArgumentFor(string name, Type type, object value)
            {
                return new CustomAttributeNamedArgument(name,
                    new CustomAttributeArgument(module.ImportReference(type), value));
            }

            var item = new CustomAttribute(burstCompileAttributeConstructor);

            var useBurstMethod = LambdaJobDescriptionConstruction.InvokedConstructionMethods.FirstOrDefault(m=>m.MethodName == nameof(LambdaJobDescriptionConstructionMethods.WithBurst));

            if (useBurstMethod != null && useBurstMethod.Arguments.Length == 3)
            {
                item.Properties.Add(CustomAttributeNamedArgumentFor(nameof(BurstCompileAttribute.FloatMode),typeof(FloatMode), useBurstMethod.Arguments[0]));
                item.Properties.Add(CustomAttributeNamedArgumentFor(nameof(BurstCompileAttribute.FloatPrecision),typeof(FloatPrecision), useBurstMethod.Arguments[1]));
                item.Properties.Add(CustomAttributeNamedArgumentFor(nameof(BurstCompileAttribute.CompileSynchronously),typeof(bool), useBurstMethod.Arguments[2]));
            }

            TypeDefinition.CustomAttributes.Add(item);
            RunWithoutJobSystemMethod?.CustomAttributes.Add(item);
        }


        private MethodDefinition AddMethodToTransferFieldsWithDisplayClass(string methodName, TransferDirection direction)
        {
            var method =new MethodDefinition(methodName, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot, TypeDefinition.Module.TypeSystem.Void);

            var displayClassTypeReference = LambdaJobDescriptionConstruction.DisplayClass;
            var parameterType = displayClassTypeReference.IsValueType
                ? new ByReferenceType(displayClassTypeReference)
                : (TypeReference)displayClassTypeReference;
            method.Parameters.Add(new ParameterDefinition("displayClass", ParameterAttributes.None,parameterType));

            var ilProcessor = method.Body.GetILProcessor();
            foreach (var kvp in ClonedFields)
            {
                var oldField = kvp.Key;
                var newField = kvp.Value;
                if (direction == TransferDirection.DisplayClassToJob)
                {
                    ilProcessor.Emit(OpCodes.Ldarg_0);
                    ilProcessor.Emit(OpCodes.Ldarg_1);
                    ilProcessor.Emit(OpCodes.Ldfld, oldField); //load field from displayClassVariable
                    ilProcessor.Emit(OpCodes.Stfld, newField); //store that value in corresponding field in newJobStruct
                }
                else
                {
                    ilProcessor.Emit(OpCodes.Ldarg_1);
                    ilProcessor.Emit(OpCodes.Ldarg_0);
                    ilProcessor.Emit(OpCodes.Ldfld, newField); //load field from job
                    ilProcessor.Emit(OpCodes.Stfld, oldField); //store that value in corresponding field in displayclass
                }
            }

            ilProcessor.Emit(OpCodes.Ret);
            AddMethod(method);
            return method;
        }

        static (Instruction, Instruction) FindClosureCreatingInstructions(MethodBody body, Instruction callInstruction)
        {
            body.EnsurePreviousAndNextAreSet();
            var cursor = callInstruction;
            while (cursor != null)
            {
                if ((cursor.OpCode == OpCodes.Ldftn) && cursor.Next?.OpCode == OpCodes.Newobj)
                {
                    return (cursor, cursor.Next);
                }

                cursor = cursor.Previous;
            }

            InternalCompilerError.DCICE002(body.Method, callInstruction).Throw();
            return (null,null);
        }

        private static readonly List<(string methodName, Type attributeType)> ConstructionMethodsThatCorrespondToFieldAttributes = new List<(string, Type)>
        {
            (nameof(LambdaJobDescriptionConstructionMethods.WithReadOnly), typeof(ReadOnlyAttribute)),
            (nameof(LambdaJobDescriptionConstructionMethods.WithWriteOnly), typeof(WriteOnlyAttribute)),
            (nameof(LambdaJobDescriptionConstructionMethods.WithDeallocateOnJobCompletion), typeof(DeallocateOnJobCompletionAttribute)),
            (nameof(LambdaJobDescriptionConstructionMethods.WithNativeDisableContainerSafetyRestriction), typeof(NativeDisableContainerSafetyRestrictionAttribute)),
            (nameof(LambdaJobDescriptionConstructionMethods.WithNativeDisableUnsafePtrRestriction), typeof(NativeDisableUnsafePtrRestrictionAttribute)),
            (nameof(LambdaJobDescriptionConstructionMethods.WithNativeDisableParallelForRestriction), typeof(NativeDisableParallelForRestrictionAttribute)),
        };

        private enum TransferDirection
        {
            DisplayClassToJob,
            JobToDisplayClass
        }

        public static MethodReference AttributeConstructorReferenceFor(Type attributeType, ModuleDefinition module)
        {
            return module.ImportReference(attributeType.GetConstructors().Single(c=>!c.GetParameters().Any()));
        }

        private static void VerifyClosureFunctionDoesNotWriteToCapturedVariable(IEnumerable<MethodDefinition> methods)
        {
            foreach (var method in methods)
            {
                var typeDefinitionFullName = method.DeclaringType.FullName;

                var badInstructions = method.Body.Instructions.Where(i =>
                {
                    if (i.OpCode != OpCodes.Stfld)
                        return false;
                    return ((FieldReference) i.Operand).DeclaringType.FullName == typeDefinitionFullName;
                });

                var first = badInstructions.FirstOrDefault();
                if (first == null)
                    continue;

                UserError.DC0013(((FieldReference) first.Operand), method, first).Throw();
            }
        }
    }
}
