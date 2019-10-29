using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodBody = Mono.Cecil.Cil.MethodBody;


/*
     * Input C# code will be in the format of:
     *
     * void override OnUpdate(JobHandle jobHandle)
     * {
     *     float dt = Time.deltaTime;
     *     return Entities
     *         .WithNone<Boid>()
     *         .WithBurst(maybe)
     *         .ForEach((ref Position p, in Velocity v) =>
     *          {
     *             p.Value += v.Value * dt;
     *          })
     *         .Schedule(jobHandle);
     * }
     *
     * This syntax is nice, but the way the C# compiler generates code for it is not great for us. The code the compiler creates looks like this:
     *
     *  JobHandle override OnUpdate(JobHandle jobHandle)
     *  {
     *     var displayClass = new DisplayClass()
     *     displayClass.dt = Time.deltaTime;
     *
     *     var tempValue = EntitiesForEach;
     *     tempValue = tempValue.WithNone<Boid>();
     *     tempValue = tempValue.WithBurst(true);
     *     var mydelegate = new delegate(displayClass.Invoke, displayClass);
     *     tempValue = tempValue.ForEach(mydelegate);
     *     return tempValue.Schedule(jobHandle);
     *  }
     *
     *  class DisplayClass
     *  {
     *     public void dt;
     *
     *     public void Invoke(ref Position p, in Velocity v)
     *     {
     *        p.Value += v.Value * dt;
     *     }
     *  }
     *
     *  The first thing to note is that because the lambda expression captures (=uses) the local variable dt, _and_ the c# compiler does not try to convince
     *  itself that the delegate does not live longer than OnUpdate() method, the c# compiler decides "okay, we cannot store variable dt on the stack like we always
     *  do, because someone might hold on to the delegate, and invoke it 10 minutes from now, and they still need to be able to access variable dt".  So what it
     *  does is it creates a separate class that it names DisplayClass. It instantiates a single instance of that at the beginning of the function, and any local variable
     *  that the compiler needs to ensure can stay around for longer than this stackframe is alive, gets stored in the displayclass instead. From that point on, any normal
     *  reads and writes to that local variable will read/write to the field of that single displayclass instance.
     *
     *  Also note that the code of the lambda expression was turned into a method that lives on this DisplayClass, so that the code can easily access these captured variables.
     *
     *  The good news is that the compiler does a lot of the heavy lifting for us. It already figured out all the variables the lambda expression wants to read from. But
     *  the bad news is that the code it generated causes a heap allocation of this DisplayClass. This is especially sad, because when we do "escape analysis" in our head
     *  for the mydelegate variable that holds our delegate, we can easily see that it "doesn't escape anywhere", so this worry of it being invoked 10 minutes from now is invalid.
     *
     *  This re-writer will take the output of the c# compiler as described above, and make a series of changes to it:
     *  1) We will change DisplayClass to be a struct instead of a class, to avoid the heap allocation that we can see is not required anyway.
     *     (this requires changing the DisplayClass type itself, but also the IL of the OnUpdate() method, as the IL that instantiates a heap object, and reads/writes to its field
     *     is different from the IL that you need to do the same to a struct that lives in a local IL variable.

     *  2) Note that the DisplayClass struct almost looks like a manually written job struct. Unfortunately we cannot simply change it a bit more to be like a job struct, as
     *     it's possible and very likely that the OnUpdate() method has another lambda expression somewhere, and that one will also be placed in the same DisplayClass, and our job
     *     system does not support using the same class for different jobs.
     *     So, too bad, we need to make a new custom struct for our job. We will do that, and copy all the relevant parts of DisplayStruct that we need:
     *     - we copy all the fields that DisplayClass.Invoke() uses,
     *     - we copy the Invoke method itself, and patch all the IL instructions to not refer to DisplayClass's fields, but to the new job structs' fields.
     *     - we make this new struct implement ICodeGeneratedJobForEach<T1,T2>
     *     - we replace the IL in OnUpdate() that creates the delegate, with IL that initializes a value of this new jobstruct, and with IL that populates all of the fields
     *       in the custom job struct with the values that are in the display class.
     *
     *  3) Since scheduling an ECS job requires a EntityQuery, and we can easily statically see what the query should be, this is code-generated automatically. We inject
     *     a GetEntityQuery() call in the system's OnCreate method, and use it in the final Schedule call.
     *
     *  4) We try very hard to keep as much code as possible in handwritten form, and require as little as possible code-generated code. Take a look at ICodeGeneratedJobForEach and
     *     WrappedCodeGeneratedJobForEach implementations to get an idea of all the handwritten code that works hand-in-hand with this code-generated code. We cannot escape some IL
     *     code-generation though, and the ICodeGeneratedJobForEach<Position,Velocity> interface that we make our job implement requires us to also implement ExecuteSingleEntity.
     *     We codegen that and have it invoke the user's original code which still lives in Invoke(). We also massage the arguments here a little bit. Notice how we pass position
     *     by ref, but velocity not by ref, as the users' code asked for velocity through "in".
     *
     *  5) Finally, we codegen a Schedule() call, directly on the generated struct itself (turns out this was easier than the traditional pattern of using an extension method).
     *     We use the handwritten WrappedCodeGeneratedJobForEach struct, which is an IJobChunk job itself. We initialize it by embedding our own job data inside of it, and setting
     *     the readonly values for each element. after that we "just schedule the wrapper as a normal IJobChunk".
     *
     * The final generated code looks roughly like this:
     *
     *  void override OnCreate()
     *  {
     *      _newJobQuery = GetEntityQuery_ForNewJob_From(this);
     *  }
     *
     *  static EntityQuery GetEntityQuery_ForNewJob_From(ComponentSystemBase componentSystem)
     *  {
     *      return componentSystem.GetEntityQuery(new EntityQueryDesc() {
     *         All = ComponentType.ReadWrite<Position>(), ComponentType.ReadOnly<Velocity>() }
     *         None = ComponentType.ReadOnly<Boid>()
     *      });
     *  }
     *
     *  JobHandle override OnUpdate(JobHandle inputDependencies)
     *  {
     *     var displayClass = new DisplayClass()
     *     displayClass.dt = Time.deltaTime;
     *
     *     var tempValue = EntitiesForEach;
     *     tempValue = tempValue.WithNone<Boid>();
     *     tempValue = tempValue.WithBurst(true);
     *     var newjob = new NewJob();
     *     newjob.ScheduleTimeInitialize(this, ref displayClass);
     *     return newjob.Schedule(this, _newJobQuery, inputDependencies);
     *  }
     *
     *  struct DisplayClass
     *  {
     *     public void dt;
     *
     *     public void Invoke(ref Position p, in Velocity v)
     *     {
     *        p.Value += v.Value * dt;
     *     }
     *  }
     *
     *  struct NewJob : ICodeGeneratedJobForEach<ElementProvider_IComponentData<Position>.Runtime, ElementProvider_IComponentData<Velocity>.Runtime>
     *  {
     *     public void dt;
     *
     *     public void Invoke(ref Position p, in Velocity v)
     *     {
     *         p.Value += v.Value * dt;
     *     }
     *
     *     public void ExecuteSingleEntity(int indexInChunk, ElementProvider_IComponentData<Position>.Runtime runtime0, ElementProvider_IComponentData<Velocity>.Runtime runtime1)
     *     {
     *         Invoke(ref runtime0.For(indexInChunk), runtime1.For(indexInChunk);
     *     }
     *
     *     public void Schedule(EntityManager entityManager, EntityQuery entityQuery)
     *     {
     *          WrappedCodeGeneratedJobForEach<NewJob,
     *             ElementProvider_IComponentData<Position>, ElementProvider_IComponentData<Position>.Runtime,
     *             ElementProvider_IComponentData<Position>, ElementProvider_IComponentData<Position>.Runtime> wrapper;
     *          wrapper.wrappedUserJob = this;
     *          wrapper.Initialize(entityManager, readonly0: false, readonly1: true);
     *          wrapper.Schedule(entityQuery);  //<-- wrapper is an IJobChunk, and this is a regular IJobChunk schedule call
     *     }
     *  }
     *
     *
     *
     *
     */
[assembly: InternalsVisibleTo("Unity.Entities.CodeGen.Tests")]

namespace Unity.Entities.CodeGen
{
    internal class LambdaJobsPostProcessor : EntitiesILPostProcessor
    {
        private const string OnCreateForCompilerName = nameof(ComponentSystemBase.OnCreateForCompiler);

        protected override bool PostProcessImpl()
        {
            var mainModuleTypes = AssemblyDefinition.MainModule.GetAllTypes().Where(TypeDefinitionExtensions.IsComponentSystem).ToArray();

            bool madeChange = false;

            foreach (var systemType in mainModuleTypes)
            {
                InjectOnCreateForCompiler(systemType);
                madeChange = true;
            }
            
            foreach (var m in mainModuleTypes.SelectMany(m => m.Methods).ToList())
            {
                LambdaJobDescriptionConstruction[] lambdaJobDescriptionConstructions;
                try
                {
                    lambdaJobDescriptionConstructions = LambdaJobDescriptionConstruction.FindIn(m).ToArray();
                    foreach (var description in lambdaJobDescriptionConstructions)
                    {
                        madeChange = true;
                        Rewrite(m, description);
                    }
                }
                catch (PostProcessException ppe)
                {
                    AddDiagnostic(ppe.ToDiagnosticMessage(m));
                }
            }

            return madeChange;
        }

        private static MethodDefinition InjectOnCreateForCompiler(TypeDefinition typeDefinition)
        {
            // Turns out it's not trivial to inject some code that has to be run OnCreate of the system.
            // We cannot just create an OnCreate() method, because there might be a deriving class that also implements it.
            // That child method is probably not calling base.OnCreate(), but even when it is (!!) the c# compiler bakes base.OnCreate()
            // into a direct reference to whatever is the first baseclass to have OnCreate() at the time of compilation.  So if we go
            // and inject an OnCreate() in this class later on,  the child's base.OnCreate() call will actually bypass it.
            //
            // Instead what we do is add OnCreateForCompiler,  hide it from intellisense, give you an error if wanna be that guy that goes
            // and implement it anyway,  and then we inject a OnCreateForCompiler method into each and every ComponentSystem.  The reason we have to emit it in
            // each and every system, and not just the ones where we have something to inject,  is that when we emit these method, we need
            // to also emit base.OnCreateForCompiler().  However, when we are processing an user system type,  we do not know yet if its baseclass
            // also needs an OnCreateForCompiler().   So we play it safe, and assume it does.  So every OnCreateForCompiler() that we emit,
            // will assume its basetype also has an implementation and invoke that.
            
            if (typeDefinition.Name == nameof(ComponentSystemBase) && typeDefinition.Namespace == "Unity.Entities")
                return null;
            
            var name = OnCreateForCompilerName;

            var pre_existing_method = typeDefinition.Methods.FirstOrDefault(m => m.Name == name);
            if (pre_existing_method != null)
                UserError.DC0026($"It's not allowed to implement {OnCreateForCompilerName}'", pre_existing_method).Throw();

            var typeSystemVoid = typeDefinition.Module.TypeSystem.Void;
            var newMethod = new MethodDefinition(name,MethodAttributes.FamORAssem | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeSystemVoid);
            typeDefinition.Methods.Add(newMethod);

            var ilProcessor = newMethod.Body.GetILProcessor();
            ilProcessor.Emit(OpCodes.Ldarg_0);
            ilProcessor.Emit(OpCodes.Call, new MethodReference(name, typeSystemVoid, typeDefinition.BaseType) { HasThis = true});
            ilProcessor.Emit(OpCodes.Ret);
            return newMethod;
        }
        
        public static MethodDefinition GetOrMakeOnCreateForCompilerMethod(TypeDefinition userSystemType)
        {
            return userSystemType.Methods.SingleOrDefault(m => m.Name == OnCreateForCompilerName) ?? InjectOnCreateForCompiler(userSystemType);
        }

        private static bool keepUnmodifiedVersionAroundForDebugging = false;

        public static JobStructForLambdaJob Rewrite(MethodDefinition methodContainingLambdaJob, LambdaJobDescriptionConstruction lambdaJobDescriptionConstruction)
        {
            if (keepUnmodifiedVersionAroundForDebugging) CecilHelpers.CloneMethodForDiagnosingProblems(methodContainingLambdaJob);

            //in order to make it easier and safer to operate on IL, we're going to "Simplify" it.  This means transforming opcodes that have short variants, into the full one
            //so that when you analyze, you can assume (ldarg, 0) opcode + operand pair,  and don't have to go check for the shorthand version of ldarg_0, without an operand.
            //more importantly, it also rewrites branch.s (branch to a instruction that is closeby enough that the offset fits within a byte), to a regular branch instructions.
            //this is the only sane thing to do, because when we rewrite IL code, we add instructions, so it can happen that by adding instructions, what was possible to be a short
            //branch instruction, now is no longer a valid short branch target,  and cecil doesn't warn against that, it will just generate broken IL, and you'll spend a long time
            //figuring out what is going on.
            methodContainingLambdaJob.Body.SimplifyMacros();
            
            var methodLambdaWasEmittedAs = lambdaJobDescriptionConstruction.MethodLambdaWasEmittedAs;
            
            if (methodLambdaWasEmittedAs.DeclaringType.TypeReferenceEquals(methodContainingLambdaJob.DeclaringType) && !lambdaJobDescriptionConstruction.AllowReferenceTypes)
            {
                //sometimes roslyn emits the lambda as an instance method in the same type of the method that contains the lambda expression.
                //it does this only in the situation where the lambda captures a field _and_ does not capture any locals.  in this case
                //there's no displayclass being created.  We should figure out exactly what instruction caused this behaviour, and tell the user
                //she can't read a field like that.

                var illegalFieldRead = methodLambdaWasEmittedAs.Body.Instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldfld && i.Previous?.OpCode == OpCodes.Ldarg_0);
                if (illegalFieldRead != null)
                    UserError.DC0001(methodContainingLambdaJob, illegalFieldRead, (FieldReference) illegalFieldRead.Operand).Throw();

                var illegalInvocation = methodLambdaWasEmittedAs.Body.Instructions.FirstOrDefault(i => i.IsInvocation() && ((MethodReference)i.Operand).DeclaringType.TypeReferenceEquals(methodContainingLambdaJob.DeclaringType));
                if (illegalInvocation != null)
                    UserError.DC0002(methodContainingLambdaJob, illegalInvocation, (MethodReference)illegalInvocation.Operand).Throw();

                //this should never hit, but is here to make sure that in case we have a bug in detecting why roslyn emitted it like this, we can at least report an error, instead of silently generating invalid code.
                InternalCompilerError.DCICE001(methodContainingLambdaJob).Throw();
            }
 
            

            var moduleDefinition = methodContainingLambdaJob.Module;

            var body = methodContainingLambdaJob.Body;
            var ilProcessor = body.GetILProcessor();

            VariableDefinition displayClassVariable = null;
            if (lambdaJobDescriptionConstruction.DelegateProducingSequence.CapturesLocals)
            {
                bool allDelegatesAreGuaranteedNotToOutliveMethod = lambdaJobDescriptionConstruction.DisplayClass.IsValueType || CecilHelpers.AllDelegatesAreGuaranteedNotToOutliveMethodFor(methodContainingLambdaJob);


                displayClassVariable = body.Variables.Single(v => v.VariableType.TypeReferenceEquals(lambdaJobDescriptionConstruction.DisplayClass));

                //in this step we want to get rid of the heap allocation for the delegate. In order to make the rest of the code easier to reason about and write,
                //we'll make sure that while we do this, we don't change the total stackbehaviour. Because this used to push a delegate onto the evaluation stack,
                //we also have to write something to the evaluation stack.  Later in this method it will be popped, so it doesn't matter what it is really.  I use Ldc_I4_0,
                //as I found it introduced the most reasonable artifacts when the code is decompiled back into C#.
                lambdaJobDescriptionConstruction.DelegateProducingSequence.RewriteToKeepDisplayClassOnEvaluationStack();

                if (!lambdaJobDescriptionConstruction.DisplayClass.IsValueType && allDelegatesAreGuaranteedNotToOutliveMethod)
                {
                    CecilHelpers.PatchMethodThatUsedDisplayClassToTreatItAsAStruct(body, displayClassVariable, lambdaJobDescriptionConstruction.DisplayClass);
                    CecilHelpers.PatchDisplayClassToBeAStruct(lambdaJobDescriptionConstruction.DisplayClass);
                }
            }
            else
            {
                //if the lambda is not capturing, roslyn will recycle the delegate in a static field. not so great for us. let's nop out all that code.
                var instructionThatPushedDelegate = CecilHelpers.FindInstructionThatPushedArg(methodContainingLambdaJob, 1, lambdaJobDescriptionConstruction.WithCodeInvocationInstruction);

                var result = CecilHelpers.MatchesDelegateProducingPattern(methodContainingLambdaJob, instructionThatPushedDelegate, CecilHelpers.DelegateProducingPattern.MatchSide.Start);
                result?.RewriteToProduceSingleNullValue();
            }

            FieldDefinition entityQueryField = null;
            if (lambdaJobDescriptionConstruction.Kind != LambdaJobDescriptionKind.Job)
                entityQueryField = InjectAndInitializeEntityQueryField.InjectAndInitialize(methodContainingLambdaJob, lambdaJobDescriptionConstruction, methodLambdaWasEmittedAs.Parameters);
            
            
            var generatedJobStruct = JobStructForLambdaJob.CreateNewJobStruct(lambdaJobDescriptionConstruction);

            if (generatedJobStruct.RunWithoutJobSystemDelegateFieldNoBurst != null)
            {
                var constructorInfo = generatedJobStruct.ExecuteDelegateType.GetConstructors().First(c=>c.GetParameters().Length==2);
                
                var instructions = new List<Instruction>()
                {
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Ldftn, generatedJobStruct.RunWithoutJobSystemMethod),
                    Instruction.Create(OpCodes.Newobj, moduleDefinition.ImportReference(constructorInfo)),
                    Instruction.Create(OpCodes.Stsfld, generatedJobStruct.RunWithoutJobSystemDelegateFieldNoBurst)
                };
                if (generatedJobStruct.RunWithoutJobSystemDelegateFieldBurst != null)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldsfld, generatedJobStruct.RunWithoutJobSystemDelegateFieldNoBurst));

                    var methodInfo = typeof(InternalCompilerInterface)
                        .GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(m=>m.Name==nameof(InternalCompilerInterface.BurstCompile))
                        .Single(m=>m.GetParameters().First().ParameterType == generatedJobStruct.ExecuteDelegateType);

                    instructions.Add(Instruction.Create(OpCodes.Call, moduleDefinition.ImportReference(methodInfo)));
                    instructions.Add(    Instruction.Create(OpCodes.Stsfld, generatedJobStruct.RunWithoutJobSystemDelegateFieldBurst));
                }
                InjectAndInitializeEntityQueryField.InsertIntoOnCreateForCompilerMethod(methodContainingLambdaJob.DeclaringType,instructions.ToArray());
            }

            IEnumerable<Instruction> InstructionsToReplaceScheduleInvocationWith()
            {
                var newJobStructVariable = new VariableDefinition(generatedJobStruct.TypeDefinition);
                body.Variables.Add(newJobStructVariable);

                VariableDefinition tempStorageForJobHandle = null;
                if (lambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Schedule)
                {
                    tempStorageForJobHandle = new VariableDefinition(moduleDefinition.ImportReference(typeof(JobHandle)));
                    body.Variables.Add(tempStorageForJobHandle);

                    //since we're replacing the .Schedule() function on the description, the lambdajobdescription and the jobhandle argument to that function will be on the stack.
                    //we're going to need the jobhandle later when we call JobChunkExtensions.Schedule(), so lets stuff it in a variable.
                    yield return Instruction.Create(OpCodes.Stloc, tempStorageForJobHandle);
                }

                //pop the Description struct off the stack, its services are no longer required
                yield return Instruction.Create(OpCodes.Pop);

                yield return Instruction.Create(OpCodes.Ldloca, newJobStructVariable);
                yield return Instruction.Create(OpCodes.Initobj, generatedJobStruct.TypeDefinition);

                yield return Instruction.Create(OpCodes.Ldloca, newJobStructVariable);

                yield return Instruction.Create(OpCodes.Ldarg_0);
                if (lambdaJobDescriptionConstruction.DelegateProducingSequence.CapturesLocals)
                {
                    //only when the lambda is capturing, did we emit the ScheduleTimeInitialize method to take a displayclass argument
                    var opcode = methodLambdaWasEmittedAs.DeclaringType.IsValueType ? OpCodes.Ldloca : OpCodes.Ldloc;
                    yield return Instruction.Create(opcode, displayClassVariable);
                }

                yield return Instruction.Create(OpCodes.Call, generatedJobStruct.ScheduleTimeInitializeMethod);

                
                MethodInfo FindRunOrScheduleMethod()
                {
                    switch (lambdaJobDescriptionConstruction.Kind)
                    {
                        case LambdaJobDescriptionKind.Entities:
                        case LambdaJobDescriptionKind.Chunk:
                            if (lambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Schedule)
                                return typeof(JobChunkExtensions).GetMethod(nameof(JobChunkExtensions.Schedule));
                            return typeof(InternalCompilerInterface).GetMethod(nameof(InternalCompilerInterface.RunJobChunk));
                        case LambdaJobDescriptionKind.Job:
                            if (lambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Schedule)
                                return typeof(IJobExtensions).GetMethod(nameof(IJobExtensions.Schedule));
                            return typeof(InternalCompilerInterface).GetMethod(nameof(InternalCompilerInterface.RunIJob));
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                var runOrScheduleMethod = moduleDefinition
                    .ImportReference(FindRunOrScheduleMethod())
                    .MakeGenericInstanceMethod(generatedJobStruct.TypeDefinition);

                yield return Instruction.Create(runOrScheduleMethod.Parameters.First().ParameterType.IsByReference ? OpCodes.Ldloca : OpCodes.Ldloc, newJobStructVariable);

                switch (lambdaJobDescriptionConstruction.Kind)
                {
                    case LambdaJobDescriptionKind.Entities:
                    case LambdaJobDescriptionKind.Chunk:
                        yield return Instruction.Create(OpCodes.Ldarg_0);
                        yield return Instruction.Create(OpCodes.Ldfld, entityQueryField);
                        break;
                    case LambdaJobDescriptionKind.Job:
                        //job.Schedule() takes no entityQuery...
                        break;
                }

                if (lambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Schedule)
                    yield return Instruction.Create(OpCodes.Ldloc, tempStorageForJobHandle);

                if (lambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Run)
                {
                    if (!lambdaJobDescriptionConstruction.UsesBurst)
                        yield return Instruction.Create(OpCodes.Ldsfld, generatedJobStruct.RunWithoutJobSystemDelegateFieldNoBurst);
                    else
                    {
                        yield return Instruction.Create(OpCodes.Call, moduleDefinition.ImportReference(typeof(JobsUtility).GetMethod("get_"+nameof(JobsUtility.JobCompilerEnabled))));

                        var targetInstruction = Instruction.Create(OpCodes.Ldsfld, generatedJobStruct.RunWithoutJobSystemDelegateFieldBurst);
                        yield return Instruction.Create(OpCodes.Brtrue, targetInstruction);
                        yield return Instruction.Create(OpCodes.Ldsfld, generatedJobStruct.RunWithoutJobSystemDelegateFieldNoBurst);
                        var finalBranchDestination = Instruction.Create(OpCodes.Nop);
                        yield return Instruction.Create(OpCodes.Br, finalBranchDestination);
                        yield return targetInstruction;
                        yield return finalBranchDestination;
                    }
                }

                yield return Instruction.Create(OpCodes.Call, runOrScheduleMethod);
                
                
                if (lambdaJobDescriptionConstruction.ExecutionMode == ExecutionMode.Run && generatedJobStruct.WriteToDisplayClassMethod != null && lambdaJobDescriptionConstruction.DelegateProducingSequence.CapturesLocals)
                {
                    yield return Instruction.Create(OpCodes.Ldloca, newJobStructVariable);
                    
                    var opcode = methodLambdaWasEmittedAs.DeclaringType.IsValueType ? OpCodes.Ldloca : OpCodes.Ldloc;
                    yield return Instruction.Create(opcode, displayClassVariable);
                    yield return Instruction.Create(OpCodes.Call, generatedJobStruct.WriteToDisplayClassMethod);
                }
            }

            foreach (var invokedMethod in lambdaJobDescriptionConstruction.InvokedConstructionMethods)
            {
                bool invokedMethodServesNoPurposeAtRuntime =
                    invokedMethod.MethodName != nameof(LambdaJobQueryConstructionMethods.WithSharedComponentFilter);

                if (invokedMethodServesNoPurposeAtRuntime)
                {
                    CecilHelpers.EraseMethodInvocationFromInstructions(ilProcessor, invokedMethod.InstructionInvokingMethod);
                }
                else
                {
                    // Rewrite WithSharedComponentFilter calls as they need to modify EntityQuery dynamically
                    if (invokedMethod.MethodName ==
                        nameof(LambdaJobQueryConstructionMethods.WithSharedComponentFilter))
                    {

                        var setSharedComponentFilterOnQueryMethod
                            = moduleDefinition.ImportReference(
                                (lambdaJobDescriptionConstruction.Kind == LambdaJobDescriptionKind.Entities ? typeof(ForEachLambdaJobDescription_SetSharedComponent) : typeof(LambdaJobChunkDescription_SetSharedComponent)).GetMethod(
                                    nameof(LambdaJobChunkDescription_SetSharedComponent
                                        .SetSharedComponentFilterOnQuery)));
                        MethodReference genericSetSharedComponentFilterOnQueryMethod
                            = setSharedComponentFilterOnQueryMethod.MakeGenericInstanceMethod(invokedMethod.TypeArguments);

                        // Change invocation to invocation of helper method and add EntityQuery parameter to be modified
                        var setSharedComponentFilterOnQueryInstructions = new List<Instruction>
                        {
                            Instruction.Create(OpCodes.Ldarg_0),
                            Instruction.Create(OpCodes.Ldfld, entityQueryField),
                            Instruction.Create(OpCodes.Call, genericSetSharedComponentFilterOnQueryMethod)
                        };

                        ilProcessor.Replace(invokedMethod.InstructionInvokingMethod,
                            setSharedComponentFilterOnQueryInstructions);
					}
            	}
            }

            var scheduleInstructions = InstructionsToReplaceScheduleInvocationWith().ToList();
            ilProcessor.InsertAfter(lambdaJobDescriptionConstruction.ScheduleOrRunInvocationInstruction, scheduleInstructions);
            lambdaJobDescriptionConstruction.ScheduleOrRunInvocationInstruction.MakeNOP();

            var codegenInitializeMethod = GetOrMakeOnCreateForCompilerMethod(lambdaJobDescriptionConstruction.ContainingMethod.DeclaringType);

            return generatedJobStruct;
        }

    }
}
