using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.Entities.CodeGeneratedJobForEach;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using TypeReference = Mono.Cecil.TypeReference;

namespace Unity.Entities.CodeGen
{
    enum LambdaJobDescriptionKind
    {
        Entities,
        Job,
        Chunk
    }
    internal enum ExecutionMode
    {
        Schedule,
        Run
    }
    
    
    class LambdaJobDescriptionConstruction
    {
        private const string EntitiesGetterName = "get_" + nameof(JobComponentSystem.Entities);
        private const string JobGetterName = "get_" + nameof(JobComponentSystem.Job);

        public class InvokedConstructionMethod
        {
            public InvokedConstructionMethod(string methodName, TypeReference[] typeArguments, object[] arguments, Instruction instructionInvokingMethod)
            {
                MethodName = methodName;
                TypeArguments = typeArguments;
                Arguments = arguments;
                InstructionInvokingMethod = instructionInvokingMethod;
            }

            public string MethodName { get; }
            public object[] Arguments { get; }
            public Instruction InstructionInvokingMethod { get; }
            
            public TypeReference[] TypeArguments { get; }
        }
        
        public MethodDefinition ContainingMethod { get; set; }
        public Instruction WithCodeInvocationInstruction;
        public List<InvokedConstructionMethod> InvokedConstructionMethods = new List<InvokedConstructionMethod>();
        public string Name;
        public Instruction ScheduleOrRunInvocationInstruction { get; set; }
        public LambdaJobDescriptionKind Kind;

        public Instruction ChainInitiatingInstruction { get; set; }
        public CecilHelpers.DelegateProducingSequence DelegateProducingSequence { get; set; }
        public MethodDefinition MethodLambdaWasEmittedAs => DelegateProducingSequence.MethodLambdaWasEmittedAs;

        public bool LambdaWasEmittedAsInstanceMethodOnContainingType => MethodLambdaWasEmittedAs.DeclaringType.TypeReferenceEquals(ContainingMethod.DeclaringType);
        
        public bool UsesBurst
        {
            get
            {
                if (InvokedConstructionMethods.Any(m => m.MethodName == nameof(LambdaJobDescriptionConstructionMethods.WithoutBurst)))
                    return false;

                var oldWithBurstMethod = InvokedConstructionMethods.FirstOrDefault(m =>m.MethodName == nameof(LambdaJobDescriptionConstructionMethods.WithBurst) && m.Arguments.Length == 1);
                if (oldWithBurstMethod != null)
                    return (int)oldWithBurstMethod.Arguments[0] == 1;

                return true;
            }
        }

        public ExecutionMode ExecutionMode =>
            ((MethodReference) ScheduleOrRunInvocationInstruction.Operand).Name == "Run"
                ? ExecutionMode.Run
                : ExecutionMode.Schedule;

        public bool AllowReferenceTypes => ExecutionMode == ExecutionMode.Run && !UsesBurst;

        public TypeDefinition DisplayClass
        {
            get
            {
                if (!DelegateProducingSequence.CapturesLocals)
                    throw new ArgumentException("Cannot ask for DisplayClass for a jobdescription that doesnt capture locals");
                return MethodLambdaWasEmittedAs.DeclaringType;
            }
        }
        
        public override string ToString()
        {
            var analysis = this;
            var sb = new StringBuilder();
            foreach (var m in analysis.InvokedConstructionMethods)
            {
                sb.Append($"{m.MethodName} ");

                foreach (var tp in m.TypeArguments)
                    sb.Append($"<{tp.Name}> ");

                foreach (var i in m.Arguments)
                    sb.Append(i + " ");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static IEnumerable<LambdaJobDescriptionConstruction> FindIn(MethodDefinition method)
        {
            var body = method.Body;

            if (body == null)
                yield break;

            var lambdaJobStatementStartingInstructions = body.Instructions.Where(i =>
            {
                if (i.OpCode != OpCodes.Call && i.OpCode != OpCodes.Callvirt)
                    return false;
                var mr = (MethodReference) i.Operand;

                if (mr.Name == EntitiesGetterName && mr.ReturnType.Name == nameof(ForEachLambdaJobDescription))
                    return true;
                if (mr.Name == JobGetterName && mr.DeclaringType.Name == nameof(JobComponentSystem))
                    return true;
#if ENABLE_DOTS_COMPILER_CHUNKS
                if (mr.Name == "get_" + nameof(JobComponentSystem.Chunks) && mr.DeclaringType.Name == nameof(JobComponentSystem))
                    return true;
#endif
                return false;
            }).ToList();

            int counter = 0;

            foreach (var lambdaJobStatementStartingInstruction in lambdaJobStatementStartingInstructions)
            {
                LambdaJobDescriptionConstruction result = default;
                result = AnalyzeLambdaJobStatement(method, lambdaJobStatementStartingInstruction, counter++);
                yield return result;
            }
        }

        static LambdaJobDescriptionConstruction AnalyzeLambdaJobStatement(MethodDefinition method, Instruction getEntitiesOrJobInstruction, int lambdaNumber)
        {
            List<InvokedConstructionMethod> modifiers = new List<InvokedConstructionMethod>();

            Instruction cursor = getEntitiesOrJobInstruction;
            var expectedPreviousMethodPushingDescription = getEntitiesOrJobInstruction;
            while (true)
            {
                cursor = FindNextConstructionMethod(method, cursor);

                var mr = cursor?.Operand as MethodReference;

                if (mr.Name == nameof(LambdaJobDescriptionConstructionMethods.Schedule) || mr.Name == nameof(LambdaJobDescriptionConstructionMethods.Run))
                {
                    var withNameModifier = modifiers.FirstOrDefault(m => m.MethodName == nameof(LambdaJobDescriptionConstructionMethods.WithName));
                    var name = withNameModifier?.Arguments.OfType<string>().Single() ?? $"{method.DeclaringType.Name}_{method.Name}_LambdaJob{lambdaNumber}";



                    LambdaJobDescriptionKind FindLambdaDescriptionKind()
                    {
                        switch (((MethodReference) getEntitiesOrJobInstruction.Operand).Name)
                        {
                            case EntitiesGetterName:
                                return LambdaJobDescriptionKind.Entities;
                            case JobGetterName:
                                return LambdaJobDescriptionKind.Job;
#if ENABLE_DOTS_COMPILER_CHUNKS
                            case "get_" + nameof(JobComponentSystem.Chunks):
                                return LambdaJobDescriptionKind.Chunk;
#endif
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    if (modifiers.All(m => m.MethodName != nameof(LambdaForEachDescriptionConstructionMethods.ForEach) && m.MethodName != nameof(LambdaSimpleJobDescriptionConstructionMethods.WithCode)))
                    {
                        DiagnosticMessage MakeDiagnosticMessage()
                        {
                            switch (FindLambdaDescriptionKind())
                            {
                                case LambdaJobDescriptionKind.Entities:
                                    return UserError.DC0006(method, getEntitiesOrJobInstruction);
                                case LambdaJobDescriptionKind.Job:
                                    return UserError.DC0017(method, getEntitiesOrJobInstruction);
                                case LambdaJobDescriptionKind.Chunk:
                                    return UserError.DC0018(method, getEntitiesOrJobInstruction);
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        MakeDiagnosticMessage().Throw();
                    }

                    var withCodeInvocationInstruction = modifiers
                        .Single(m => m.MethodName == nameof(LambdaForEachDescriptionConstructionMethods.ForEach) || m.MethodName == nameof(LambdaSimpleJobDescriptionConstructionMethods.WithCode))
                        .InstructionInvokingMethod;
                    return new LambdaJobDescriptionConstruction()
                    {
                        Kind = FindLambdaDescriptionKind(),
                        InvokedConstructionMethods = modifiers,
                        WithCodeInvocationInstruction = withCodeInvocationInstruction,
                        ScheduleOrRunInvocationInstruction = cursor,
                        Name = name,
                        ChainInitiatingInstruction = getEntitiesOrJobInstruction,
                        ContainingMethod = method,
                        DelegateProducingSequence = AnalyzeForEachInvocationInstruction(method, withCodeInvocationInstruction)
                    };
                }

                var instructions = mr.Parameters.Skip(1)
                    .Select(p => OperandObjectFor(CecilHelpers.FindInstructionThatPushedArg(method, p.Index, cursor))).ToArray();

                var invokedConstructionMethod = new InvokedConstructionMethod(mr.Name,
                    (mr as GenericInstanceMethod)?.GenericArguments.ToArray() ?? Array.Empty<TypeReference>(),
                    instructions, cursor);

                var allowDynamicValue = method.Module.ImportReference(typeof(AllowDynamicValueAttribute));
                for (int i = 0; i != invokedConstructionMethod.Arguments.Length; i++)
                {
                    if (invokedConstructionMethod.Arguments[i] != null)
                        continue;

                    var inbovokedForEachMethod = mr.Resolve();
                    var methodDefinitionParameter = inbovokedForEachMethod.Parameters[i + 1];

                    if (!methodDefinitionParameter.CustomAttributes.Any(c =>c.AttributeType.TypeReferenceEquals(allowDynamicValue)))
                        UserError.DC0008(method, cursor, mr).Throw();
                }

                if (modifiers.Any(m => m.MethodName == mr.Name) && !HasAllowMultipleAttribute(mr.Resolve()))
                    UserError.DC0009(method, cursor, mr).Throw();

                var findInstructionThatPushedArg = CecilHelpers.FindInstructionThatPushedArg(method, 0, cursor);
                if (cursor == null || findInstructionThatPushedArg != expectedPreviousMethodPushingDescription)
                    UserError.DC0007(method, cursor).Throw();

                expectedPreviousMethodPushingDescription = cursor;
                modifiers.Add(invokedConstructionMethod);
            }
        }


        private static Instruction FindNextConstructionMethod(MethodDefinition method, Instruction instruction)
        {
            var cursor = instruction;
            //the description object should be on the stack, and nothing on top of it.
            int stackDepth = 1;
            while (cursor.Next != null)
            {
                cursor = cursor.Next;

                var result = CecilHelpers.MatchesDelegateProducingPattern(method, cursor,CecilHelpers.DelegateProducingPattern.MatchSide.Start);
                if (result != null)
                {
                    cursor = result.Instructions.Last();
                    stackDepth += 1;
                    continue;
                }

                if (CecilHelpers.IsUnsupportedBranch(cursor))
                    UserError.DC0010(method, cursor).Throw();

                if (cursor.OpCode == OpCodes.Call && cursor.Operand is MethodReference mr && IsLambdaJobDescriptionConstructionMethod(mr))
                    return cursor;

                stackDepth -= cursor.GetPopDelta();
                if (stackDepth < 1)
                    UserError.DC0011(method, cursor).Throw();

                stackDepth += cursor.GetPushDelta();
            }

            return null;
        }

        static bool HasAllowMultipleAttribute(MethodDefinition mr) => mr.HasCustomAttributes && mr.CustomAttributes.Any(c => c.AttributeType.Name == nameof(LambdaJobDescriptionConstructionMethods.AllowMultipleInvocationsAttribute));

        private static object OperandObjectFor(Instruction argumentPushingInstruction)
        {
            var opCode = argumentPushingInstruction.OpCode;

            if (opCode == OpCodes.Ldstr)
                return (string) argumentPushingInstruction.Operand;
            if (opCode == OpCodes.Ldc_I4)
                return (int) argumentPushingInstruction.Operand;
            if (opCode == OpCodes.Ldc_I4_0)
                return 0;
            if (opCode == OpCodes.Ldc_I4_1)
                return 1;
            if (opCode == OpCodes.Ldc_I4_2)
                return 2;
            if (opCode == OpCodes.Ldc_I4_3)
                return 3;
            if (opCode == OpCodes.Ldc_I4_4)
                return 4;
            if (opCode == OpCodes.Ldc_I4_5)
                return 5;
            if (opCode == OpCodes.Ldc_I4_6)
                return 6;
            if (opCode == OpCodes.Ldc_I4_7)
                return 7;
            if (opCode == OpCodes.Ldc_I4_8)
                return 8;
            if (opCode == OpCodes.Ldfld)
                return argumentPushingInstruction.Operand;
            return null;
        }

        static bool IsLambdaJobDescriptionConstructionMethod(MethodReference mr) =>mr.DeclaringType.Name.EndsWith("ConstructionMethods") &&(mr.DeclaringType.Namespace == "Unity.Entities" || mr.DeclaringType.Namespace == "");

        public static CecilHelpers.DelegateProducingSequence AnalyzeForEachInvocationInstruction(MethodDefinition methodToAnalyze, Instruction withCodeInvocationInstruction)
        {
            var delegatePushingInstruction = CecilHelpers.FindInstructionThatPushedArg(methodToAnalyze, 1, withCodeInvocationInstruction);

            var result = CecilHelpers.MatchesDelegateProducingPattern(methodToAnalyze, delegatePushingInstruction, CecilHelpers.DelegateProducingPattern.MatchSide.Start);
            if (result == null)
                InternalCompilerError.DCICE002(methodToAnalyze, delegatePushingInstruction).Throw();

            return result;
        }
    }
}
