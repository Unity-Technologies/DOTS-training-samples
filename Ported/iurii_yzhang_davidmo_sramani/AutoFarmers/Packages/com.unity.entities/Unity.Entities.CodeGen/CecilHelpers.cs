using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Unity.Entities.CodeGen
{
    static class CecilHelpers
    {
        public static Instruction MakeInstruction(OpCode opcode, object operand)
        {
            if (operand is Instruction[] instructions)
                return Instruction.Create(opcode, instructions);
            switch (operand)
            {
                case null:
                    return Instruction.Create(opcode);
                case FieldReference o:
                    return Instruction.Create(opcode, o);
                case MethodReference o:
                    return Instruction.Create(opcode, o);
                case VariableDefinition o:
                    return Instruction.Create(opcode, o);
                case ParameterDefinition o:
                    return Instruction.Create(opcode, o);
                case GenericInstanceType o:
                    return Instruction.Create(opcode, o);
                case TypeReference o:
                    return Instruction.Create(opcode, o);
                case CallSite o:
                    return Instruction.Create(opcode, o);
                case int o:
                    return Instruction.Create(opcode, o);
                case float o:
                    return Instruction.Create(opcode, o);
                case double o:
                    return Instruction.Create(opcode, o);
                case sbyte o:
                    return Instruction.Create(opcode, o);
                case byte o:
                    return Instruction.Create(opcode, o);
                case long o:
                    return Instruction.Create(opcode, o);
                case uint o:
                    return Instruction.Create(opcode, o);
                case string o:
                    return Instruction.Create(opcode, o);
                case Instruction o:
                    return Instruction.Create(opcode, o);
                default:
                    throw new NotSupportedException("Unknown operand: " + operand.GetType());
            }
        }

        public static SequencePoint FindBestSequencePointFor(MethodDefinition method, Instruction instruction)
        {
            var sequencePoints = method.DebugInformation?.GetSequencePointMapping().Values.OrderBy(s => s.Offset).ToList();

            for (int i = 0; i != sequencePoints.Count-1; i++)
            {
                if (sequencePoints[i].Offset < instruction.Offset &&
                    sequencePoints[i + 1].Offset > instruction.Offset)
                    return sequencePoints[i];
            }

            return sequencePoints.FirstOrDefault();
        }

        public static (MethodDefinition[], Dictionary<FieldDefinition, FieldDefinition> oldFieldtoNewField) CloneClosureExecuteMethodAndItsLocalFunctions(
            IEnumerable<MethodDefinition> methodsToClone, TypeDefinition targetType,
            string newMethodName)
        {
            Dictionary<FieldDefinition, FieldDefinition> oldFieldtoNewField = new Dictionary<FieldDefinition, FieldDefinition>();
            var displayClassFieldsUsedByMethod =
                methodsToClone.SelectMany(method=>method.Body.Instructions)
                    .Select(i => i.Operand)
                    .OfType<FieldReference>()
                    .Where(fr => fr.DeclaringType.TypeReferenceEquals(methodsToClone.First().DeclaringType))
                    .Select(fr => fr.Resolve())
                    .Distinct()
                    .ToArray();

            foreach (var field in displayClassFieldsUsedByMethod)
            {
                var newField = new FieldDefinition(field.Name, field.Attributes, field.FieldType);
                targetType.Fields.Add(newField);
                oldFieldtoNewField.Add(field,newField);
            }

            var executeMethod = methodsToClone.First();

            if (executeMethod.HasGenericParameters)
                throw new ArgumentException();

            var clonedMethods = methodsToClone.ToDictionary(m => m, m =>
                {
                    var clonedMethod = new MethodDefinition(m == executeMethod ? newMethodName : m.Name, MethodAttributes.Public, m.ReturnType)
                            {HasThis = m.HasThis, DeclaringType = targetType};
                    targetType.Methods.Add(clonedMethod);
                    return clonedMethod;
                }
            );

            foreach (var methodToClone in methodsToClone)
            {
                var methodDefinition = clonedMethods[methodToClone];

                foreach (var lambdaParameter in methodToClone.Parameters)
                {
                    var executeParameter = new ParameterDefinition(lambdaParameter.Name, lambdaParameter.Attributes,
                        lambdaParameter.ParameterType);
                    foreach (var ca in lambdaParameter.CustomAttributes)
                        executeParameter.CustomAttributes.Add(ca);

                    methodDefinition.Parameters.Add(executeParameter);
                }

                var ilProcessor = methodDefinition.Body.GetILProcessor();

                var oldVarToNewVar = new Dictionary<VariableDefinition, VariableDefinition>();
                foreach (var vd in methodToClone.Body.Variables)
                {
                    var newVd = new VariableDefinition(vd.VariableType);
                    methodDefinition.Body.Variables.Add(newVd);
                    oldVarToNewVar.Add(vd, newVd);
                }

                var oldToNewInstructions = new Dictionary<Instruction, Instruction>();
                Instruction previous = null;
                foreach (var instruction in methodToClone.Body.Instructions)
                {
                    instruction.Previous = previous;
                    if (previous != null)
                        previous.Next = instruction;

                    var clonedOperand = instruction.Operand;
                    if (clonedOperand is FieldReference fr)
                    {
                        if (oldFieldtoNewField.TryGetValue(fr.Resolve(), out var replacement))
                            clonedOperand = replacement;
                    }

                    if (clonedOperand is VariableDefinition vd)
                    {
                        if (oldVarToNewVar.TryGetValue(vd, out var replacement))
                            clonedOperand = replacement;
                    }

                    if (clonedOperand is MethodReference mr)
                    {
                        var targetThatWeAreCloning = methodsToClone.FirstOrDefault(m => m.FullName == mr.FullName);
                        if (targetThatWeAreCloning != null)
                        {
                            var replacement = clonedMethods[targetThatWeAreCloning];
                            clonedOperand = replacement;
                        }
                    }

                    var newInstruction = MakeInstruction(instruction.OpCode, clonedOperand);
                    oldToNewInstructions.Add(instruction, newInstruction);
                    ilProcessor.Append(newInstruction);

                    previous = instruction;
                }

                var oldDebugInfo = methodToClone.DebugInformation;
                var newDebugInfo = methodDefinition.DebugInformation;
                foreach (var seq in oldDebugInfo.SequencePoints)
                    newDebugInfo.SequencePoints.Add(seq);

                // For all instructions that point to another instruction (like branches), make sure we patch those instructions to the new ones too.
                foreach (var newInstruction in oldToNewInstructions.Values)
                {
                    if (newInstruction.Operand is Instruction oldInstruction)
                        newInstruction.Operand = oldToNewInstructions[oldInstruction];
                    else if (newInstruction.Operand is Instruction[] instructions)
                        newInstruction.Operand = instructions.Select(i => oldToNewInstructions[i]).ToArray();
                }
            }

            return (clonedMethods.Values.ToArray(), oldFieldtoNewField);
        }


        public static void EraseMethodInvocationFromInstructions(ILProcessor ilProcessor, Instruction callInstruction)
        {
            var argumentPushingInstructions = new List<Instruction>();
            int succesfullyEraseArguments = 0;

            if (!(callInstruction.Operand is MethodReference methodReference))
                return;

            bool isMethodThatReturnsItsFirstArgument = !methodReference.HasThis && (methodReference.Parameters.FirstOrDefault()?.ParameterType.TypeReferenceEquals(methodReference.ReturnType) ?? false);

            var parametersCount = methodReference.Parameters.Count + (methodReference.HasThis ? 1 : 0);
            for (int i = 0; i != parametersCount; i++)
            {
                if (isMethodThatReturnsItsFirstArgument && i == 0)
                    continue;

                var instructionThatPushedArg = FindInstructionThatPushedArg(ilProcessor.Body.Method, i, callInstruction);
                if (instructionThatPushedArg == null)
                    continue;

                if (InstructionExtensions.IsInvocation(instructionThatPushedArg))
                    continue;

                var pushDelta = InstructionExtensions.GetPushDelta(instructionThatPushedArg);
                var popDelta = InstructionExtensions.GetPopDelta(instructionThatPushedArg);

                if (pushDelta == 1 && popDelta == 0)
                {
                    argumentPushingInstructions.Add(instructionThatPushedArg);
                    succesfullyEraseArguments++;
                    continue;
                }

                if (pushDelta == 1 && popDelta == 1)
                {
                    var whoPushedThat = FindInstructionThatPushedArg(ilProcessor.Body.Method, 0, instructionThatPushedArg);
                    if (InstructionExtensions.GetPopDelta(whoPushedThat) == 0 && InstructionExtensions.GetPushDelta(whoPushedThat) == 1)
                    {
                        argumentPushingInstructions.Add(instructionThatPushedArg);
                        argumentPushingInstructions.Add(whoPushedThat);
                        succesfullyEraseArguments++;
                        continue;
                    }
                }
            }


            foreach (var i in argumentPushingInstructions)
                i.MakeNOP();

            //we're going to remove the invocation. While we do this we want to remain stack neutral. The stackbehaviour going in is that the jobdescription itself will be on the stack,
            //plus any arguments the method might have.  After the function, it will put the same jobdescription on the stack as the return value.
            //we're going to pop all the arguments, but leave the jobdescription itself on the stack, since that the behaviour of the original method.
            var parametersToErase = parametersCount - (isMethodThatReturnsItsFirstArgument ? 1 : 0);
            var popInstructions = Enumerable.Repeat(Instruction.Create(OpCodes.Pop), parametersToErase - succesfullyEraseArguments);
            ilProcessor.InsertBefore(callInstruction, popInstructions);

            //instead of removing the call instruction, we'll replace it with a NOP opcode. This is safer as this instruction
            //might be the target of a branch instruction that we don't want to become invalid.
            callInstruction.MakeNOP();

            if (!methodReference.ReturnType.TypeReferenceEquals(methodReference.Module.TypeSystem.Void) && !isMethodThatReturnsItsFirstArgument)
            {
                callInstruction.OpCode = OpCodes.Ldnull;
            }
        }

        public class DelegateProducingSequence
        {
            public bool CapturesLocals;
            public MethodDefinition MethodLambdaWasEmittedAs;
            public MethodDefinition OriginalLambdaContainingMethod;
            public Instruction[] Instructions;

            public void RewriteToProduceSingleNullValue()
            {
                if (CapturesLocals)
                    throw new ArgumentException($"Cannot {nameof(RewriteToProduceSingleNullValue)} when {nameof(CapturesLocals)} is true");

                foreach (var i in Instructions)
                    i.MakeNOP();
                Instructions.Last().OpCode = OpCodes.Ldnull;
            }


            public void RewriteToKeepDisplayClassOnEvaluationStack()
            {
                if (!CapturesLocals)
                    throw new ArgumentException($"Cannot {nameof(RewriteToKeepDisplayClassOnEvaluationStack)} when {nameof(CapturesLocals)} is false");

                var ldFtn = Instructions.Single(i => i.OpCode == OpCodes.Ldftn);
                var ldLoc_that_loads_displayclass = ldFtn.Previous;
                if (!ldLoc_that_loads_displayclass.IsLoadLocal(out _) && !ldLoc_that_loads_displayclass.IsLoadLocalAddress(out _))
                    throw new PostProcessException("ldftn opcode was not preceeded with ldloc opcode", OriginalLambdaContainingMethod, ldFtn);
                foreach (var i in Instructions)
                {
                    if (i != ldLoc_that_loads_displayclass)
                        i.MakeNOP();
                }
            }
        }

        public class DelegateProducingPattern
        {
            public Func<Instruction, bool>[] InstructionMatchers;
            public bool CapturesLocal;
            public bool CapturesField;

            public enum MatchSide
            {
                Start,
                End
            }

            public DelegateProducingSequence Match(MethodDefinition containingMethod, Instruction i, MatchSide side)
            {
                DelegateProducingSequence MakeResult(Instruction[] instructions)
                {
                    return new DelegateProducingSequence()
                    {
                        Instructions = instructions, CapturesLocals = CapturesLocal,
                        OriginalLambdaContainingMethod = containingMethod,
                        MethodLambdaWasEmittedAs = FindLambdaMethod(instructions)
                    };
                }

                switch (side)
                {
                    case MatchSide.Start:
                    {
                        return IsStartOfSequence(i, InstructionMatchers, out var instructions) ? MakeResult(instructions.ToArray()) : null;
                    }
                    case MatchSide.End:
                    {
                        return IsEndOfSequence(i, InstructionMatchers, out var instructions) ? MakeResult(instructions.ToArray()) : null;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(side), side, null);
                }
            }

            private MethodDefinition FindLambdaMethod(Instruction[] instructions)
            {
                var instruction = instructions.FirstOrDefault(i => i.OpCode == OpCodes.Ldftn);
                if (instruction == null)
                    throw new ArgumentException("Instruction array did not have ldftn opcode. Instruction array way: "+instructions.Select(i=>i.ToString()).SeparateBy(Environment.NewLine));
                return ((MethodReference) instruction.Operand).Resolve();
            }
        }

        private static DelegateProducingPattern[] s_DelegateProducingPatterns;
        private static DelegateProducingPattern[] GetDelegateProducingPatterns()
        {
            if (s_DelegateProducingPatterns != null)
                return s_DelegateProducingPatterns;

            //roslyn will has a variety of ways it will emit IL code when it converts a lambda expression to a delegate object. This variety depends on several factors:
            //
            //which things the lambda expression captures.  Possible things to capture:
            //- locals
            //- parameters.  when captured, these will be copied into a local, and then the local is captured. going forward, I'll pretend parameter capture doesn't exist, and refer only to locals.
            //- instance fields
            //
            // Example of local capture:
            // https://sharplab.io/#v2:C4LglgNgPgAgTARgLACgYGYAE9MGFMDeqmJ2WMALJgLIAUAlIcaS2AHbCYC2AngDIB7AMYBDCJgC8mdAG5mLEgFEAHgFMhAV2CraDSQD48AtgGcBEVQDoA6gCcw22r0GiI9enJQsAvvOxUVdS0dGAQ4TBFGAl8UbyA==
            // Emitted IL: the expression turns into a method on the displayclass.  debug/release only differs through some NOP's sprinkled in between.
            //
            // Example of field capture:
            // https://sharplab.io/#v2:D4AQTAjAsAUCDMACciDCiDetE8QSwDsAXRAWwE8AxPAUwBsATRAXkXgG5tcFkAWRALIAKAJSYuuXAFEAHjQDGAVyI0holgD40AewIBnbXRoA6AOoAnPCqEVq9BiJGcYkgL4S+iWQuWqQEMEQAQzEMdxhXIA=
            // Emitted IL: lambda expression gets emitted as an instancemethod on the same type.    (we do not support this kind of capture, and give a compile error about it).  debug/release only differs through NOPs
            //
            // Example of local _and_ field capture
            // https://sharplab.io/#v2:D4AQTAjAsAUCDMACciDCiDetE8QSwDsAXRAWwE8AxPAUwBsATRAXkXgG5tcFkAWRALIAKAJSYuuXIRIUAMgHsAxgEM6LRAFZOMSZICiADxqKArkRpDRLAHxp5BAM7y6NAHQB1AE55zQitXomAGoycgUVOhERbUkAXwk+RENjMwsQCDBEZTEMeJhYoA==
            // Emitted IL: emitted like local capture, and the instance of the method's declaring type, gets stored in the displayclass. (we do not support this kind of capture, and give a compile error about it).  debug/release only differs through NOPs
            //
            // Because creating a new delegate is a heap allocation, roslyn tries to be smart, and in situations where it can cache the delegate, it will. The first example of this is the surprisingly simple looking:
            //
            // Example of not capturing anything
            // https://sharplab.io/#v2:CYLg1APgAgTAjAWAFBQMwAJboMLoN7LrqFGYZQAs6AsgBQCU+JppAogB4CmAxgK4AunWg3QBeAHw4A9gDsAzlIA2nAHQB1AE4BLQbQr16AbmboAvicroOPAUKhwY6AIaM85pKaA=
            // Emitted IL: method gets emitted as an instance method on a compiler generated type. the type is a singleton, and the only instance is stored in a static field on the same type.
            // The generated delegate object is cached on a static field in the same type. debug/release only differs through NOP.
            // Sidenote: Roslyn used to emit this simply as a static method, which is a lot simpler, but CoreCLR is faster at invoking instance delegates than static ones, so roslyn changed to this method.
            //
            //
            // Another dimension of "roslyn will choose a different codegen strategy" is wether or not the lambda expression is used more than once. If it's possible that it might be used more than once (like in a forloop)
            // and it wasn't possible to permanently cache the delegate, it will attempt to cache the delegate for this method invocation, so we get 1 allocation, instead of the number of loop iterations.
            //
            // Example of local capture inside a loop
            // https://sharplab.io/#v2:D4AQTAjAsAUCDMACciDCiDetE9wSwDsAXRAWwE8AzPAUwBsATAbm1wWQBZEBZACgEpMrXPmJlydAPYBjAIZ1EAXkTwWMESKzqNOSpIBOiXoRJ5FABiaI8AQkWrrAakf9hO3AFEAHjWkBXIhpeASUAPjRJAgBnSToaADoAdX08QN4KKTk6fn41d0QAXzci7RwQLm9fAKCQCDBEWUEMEoKgA==
            // Emitted IL: like regular local capture, but it will cache the delegate in a field in the displayclass.
            // Sidenote: using a forloop does cause the caching codegen to kick in, but using a while(true) loop does not.
            //
            // the extra attempt at caching only happens for "also captures locals" lambdas. lambdas that only capture fields are never cached.
            //
            // About local functions: While local functions can make the codegen look more complicated, in essence they don't affect the big picture. a local function in a lambda expression has access to every variable the lambda expression has access to
            // this means that "what things the lambda expression captures" gets augmented by whatever its local functions capture, and for the rest normal rules are being followed.
            //
            // Now that we have written an sherlock holmes essay on how roslyn emits code today, what do we do with it. The only scenarios we support is "captures only locals", and "captures nothing".
            // When nothing is captured, we can just NOP out the entire IL sequence that was responsible for making the delegate, as we only need to know what the target method was that our lambda expression ended up at.
            // When locals are captured, we have to do work:
            //   - in all of these scenarios, the lambda expression will be emitted as an instance metho on the displayclass.
            //   - in some of these scnearios the delegate will be cached on the displayclass, in other cases not.
            //
            // the second case is just a "check if we stored this already, if yes use that, if not, create it, then store" wrapper around the first case.
            // the first case (and thus also the second case) uses a "load displayclass on the stack, ldftn our executemethod, newobj our delegate type" sequence. We need to replace that with
            // create our own jobstruct, populate its fields from the displayclass. _not_ create the delegate, not try to cache the delegate, and then schedule/run our jobstruct.

            var notCapturingPattern = new DelegateProducingPattern()
            {
                InstructionMatchers = new Func<Instruction, bool>[]
                {
                    i => i.OpCode == OpCodes.Ldsfld,
                    i => i.OpCode == OpCodes.Dup,
                    i => i.IsBranch(),
                    i => i.OpCode == OpCodes.Pop,
                    i => i.OpCode == OpCodes.Ldsfld,
                    i => i.OpCode == OpCodes.Ldftn,
                    i => i.OpCode == OpCodes.Newobj,
                    i => i.OpCode == OpCodes.Dup,
                    i => i.OpCode == OpCodes.Stsfld,
                },
                CapturesLocal = false
            };

            var capturingLocal_InvokedOnce = new DelegateProducingPattern()
            {
                InstructionMatchers = new Func<Instruction, bool>[]
                {
                    i => i.IsLoadLocal(out _) || i.IsLoadLocalAddress(out _),
                    i => i.OpCode == OpCodes.Ldftn,
                    i => i.OpCode == OpCodes.Newobj,
                },
                CapturesLocal = true
            };

            var capturingLocal_ExcetedMoreThanOnce = new DelegateProducingPattern()
            {
                InstructionMatchers = new Func<Instruction, bool>[]
                {
                    i => i.IsLoadLocal(out _),
                    i => i.OpCode == OpCodes.Ldfld,
                    i => i.OpCode == OpCodes.Dup,
                    i => i.IsBranch(),
                    i => i.OpCode == OpCodes.Pop,
                    i => i.IsLoadLocal(out _),
                    i => i.IsLoadLocal(out _),
                    i => i.OpCode == OpCodes.Ldftn,
                    i => i.OpCode == OpCodes.Newobj,
                    i => i.OpCode == OpCodes.Dup,
                    i => i.IsStoreLocal(out _),
                    i => i.OpCode == OpCodes.Stfld,
                    i => i.IsLoadLocal(out _),
                },
                CapturesLocal = true,
            };

            var capturingOnlyFieldPattern = new DelegateProducingPattern()
            {
                InstructionMatchers = new Func<Instruction, bool>[]
                {
                    i => (i.OpCode == OpCodes.Ldarg_0) || (i.OpCode == OpCodes.Ldarg && ((ParameterDefinition)i.Operand).Index == -1),
                    i => i.OpCode == OpCodes.Ldftn,
                    i => i.OpCode == OpCodes.Newobj,
                },
                CapturesLocal = false,
                CapturesField = true
            };


            s_DelegateProducingPatterns = new[] {notCapturingPattern, capturingLocal_InvokedOnce, capturingLocal_ExcetedMoreThanOnce, capturingOnlyFieldPattern};
            return s_DelegateProducingPatterns;
        }

        internal static bool IsStartOfSequence(Instruction instruction, Func<Instruction, bool>[] pattern, out List<Instruction> instructions)
        {
            Instruction cursor = instruction;
            instructions = null;

            var results = new List<Instruction>(50);
            int patternIndex = 0;
            while(true)
            {
                if (cursor == null)
                    return false;
                results.Add(cursor);
                if (cursor.OpCode != OpCodes.Nop)
                {
                    if (!pattern[patternIndex++].Invoke(cursor))
                        return false;

                    if (patternIndex == pattern.Length)
                        break; //match!
                }

                cursor = cursor.Next;
            }

            instructions = results;
            return true;
        }

        internal static bool IsEndOfSequence(Instruction instruction, Func<Instruction, bool>[] pattern,out List<Instruction> instructions)
        {
            Instruction cursor = instruction;
            instructions = null;

            var result = new List<Instruction>(50);
            int patternIndex = pattern.Length-1;
            while(true)
            {
                if (cursor == null)
                    return false;
                result.Add(cursor);
                if (cursor.OpCode != OpCodes.Nop)
                {
                    if (!pattern[patternIndex--].Invoke(cursor))
                        return false;

                    if (patternIndex == -1)
                        break; //match!
                }
                cursor = cursor.Previous;
            }

            result.Reverse();
            instructions = result;
            return true;
        }

        public static DelegateProducingSequence MatchesDelegateProducingPattern(MethodDefinition containingMethod, Instruction instruction, DelegateProducingPattern.MatchSide matchSide)
        {
            return GetDelegateProducingPatterns().Select(pattern => pattern.Match(containingMethod, instruction, matchSide)).FirstOrDefault(result => result != null);
        }

        public static IEnumerable<MethodDefinition> FindUsedInstanceMethodsOnSameType(MethodDefinition method, HashSet<string> foundSoFar = null)
        {
            foundSoFar = foundSoFar ?? new HashSet<string>();

            var usedInThisMethod = method.Body.Instructions.Where(i=>i.IsInvocation()).Select(i => i.Operand).OfType<MethodReference>().Where(mr => mr.DeclaringType.TypeReferenceEquals(method.DeclaringType));

            foreach (var usedMethod in usedInThisMethod)
            {
                if (foundSoFar.Contains(usedMethod.FullName))
                    continue;
                foundSoFar.Add(usedMethod.FullName);
                var usedMethodResolved = usedMethod.Resolve();
                yield return usedMethodResolved;

                foreach (var used in FindUsedInstanceMethodsOnSameType(usedMethodResolved, foundSoFar))
                    yield return used;
            }
        }

        static readonly string _universalDelegatesNamespace = nameof(Unity) + "." + nameof(Unity.Entities) + "." + nameof(Unity.Entities.UniversalDelegates);

        public static bool AllDelegatesAreGuaranteedNotToOutliveMethodFor(MethodDefinition methodToAnalyze)
        {
            //in order to make lambda jobs be able to not allocate GC memory, we want to change the DisplayClass that stores the variables from a class to a struct.
            //This is only safe if the only delegates that are used in the methods are the ones for lambda jobs, because we know that those will not leak.  If any other
            //delegates are used, we cannot guarantee this, and we will keep the displayclass as a class, which results in a heap allocation for every invocation of the method.

            foreach (var instruction in methodToAnalyze.Body.Instructions)
            {
                //we'll find all occurrences of delegates by scanning all constructor invocations.
                if (instruction.OpCode != OpCodes.Newobj)
                    continue;
                var mr = (MethodReference) instruction.Operand;

                //to avoid a potentially expensive resolve, we'll first try to rule out this instruction as delegate creating by doing some pattern checks:

                //all delegate creation constructors take two arguments.
                if (mr.Parameters.Count != 2)
                    continue;

                //if this delegate is one of our UniversalDelegates we'll assume we're cool. This is not waterproof, as you could imagine a situation where someone
                //makes an instance of our delegate manually, and intentionally leaks that. We'll consider that scenario near-malice for now, and assume that the UniversalDelegates
                //are exclusively used as arguments for lambda jobs that do not leak.
                if (mr.DeclaringType.Namespace == _universalDelegatesNamespace)
                    continue;

                if (mr.DeclaringType.Name == typeof(LambdaJobChunkDescriptionConstructionMethods.JobChunkDelegate).Name && mr.DeclaringType.DeclaringType?.Name == nameof(LambdaJobChunkDescriptionConstructionMethods))
                    continue;

                //ok, it walks like a delegate constructor invocation, let's see if it talks like one:
                var constructedType = mr.DeclaringType.Resolve();
                if (constructedType.BaseType.Name == nameof(MulticastDelegate))
                    return false;
            }

            return true;
        }

        public static void PatchDisplayClassToBeAStruct(TypeDefinition displayClass)
        {
            displayClass.BaseType = displayClass.Module.ImportReference(typeof(ValueType));
            displayClass.IsClass = false;

            //we have to kill the body of the default constructor, as it invokes the base class constructor, which makes no sense for a valuetype
            var constructorDefinition = displayClass.Methods.Single(m => m.IsConstructor);
            constructorDefinition.Body = new MethodBody(constructorDefinition);
            constructorDefinition.Body.GetILProcessor().Emit(OpCodes.Ret);
        }

        public static void PatchMethodThatUsedDisplayClassToTreatItAsAStruct(MethodBody body, VariableDefinition displayClassVariable, TypeReference displayClassTypeReference)
        {
            var instructions = body.Instructions.ToArray();
            var ilProcessor = body.GetILProcessor();
            
            foreach (var instruction in instructions)
            {
                //we will replace all LdLoc of our displayclass to LdLoca_S of our displayclass variable that now lives on the stack.
                if (instruction.IsLoadLocal(out int loadIndex) && displayClassVariable.Index == loadIndex)
                {
                    instruction.OpCode = OpCodes.Ldloca_S;
                    instruction.Operand = body.Variables[loadIndex];
                }

                //Roselyn should never double assign the DisplayClass to a variable, throw if we somehow detect this.
                if (instruction.IsStoreLocal(out int storeIndex) && displayClassVariable.Index == storeIndex)
                {
                    InternalCompilerError.DCICE003(body.Method, instruction).Throw();
                }

                bool IsInstructionNewObjOfDisplayClass(Instruction thisInstruction)
                {
                    return thisInstruction.OpCode.Code == Code.Newobj && ((MethodReference) thisInstruction.Operand).DeclaringType.TypeReferenceEquals(displayClassTypeReference);
                }

                //we need to replace the creation of the displayclass object on the heap, with a initobj of the displayclass on the stack.
                //the final sequence will be ldloca, initobj (which will replace newobj, stloc.1.
                if (IsInstructionNewObjOfDisplayClass(instruction))
                {
                    // Remove next stloc.1 instruction so that we can replace both of these with the instructions below
                    if (!instruction.Next.IsStoreLocal(out _))
                        InternalCompilerError.DCICE004(body.Method, instruction).Throw();
                    instruction.Next.MakeNOP();
                    ilProcessor.Replace(instruction, new[]
                    {
                        Instruction.Create(OpCodes.Ldloca, displayClassVariable),
                        Instruction.Create(OpCodes.Initobj, displayClassTypeReference),
                    });
                }
            }
        }

        public static void CloneMethodForDiagnosingProblems(MethodDefinition methodToAnalyze)
        {
            var cloneName = methodToAnalyze.Name + "_Unmodified";
            if (methodToAnalyze.DeclaringType.Methods.Any(m => m.Name == cloneName))
                return;

            var clonedMethod = new MethodDefinition(cloneName, methodToAnalyze.Attributes, methodToAnalyze.ReturnType);
            foreach (var parameter in methodToAnalyze.Parameters)
                clonedMethod.Parameters.Add(parameter);
            foreach (var v in methodToAnalyze.Body.Variables)
                clonedMethod.Body.Variables.Add(new VariableDefinition(v.VariableType));
            var p = clonedMethod.Body.GetILProcessor();
            var oldToNew = new Dictionary<Instruction, Instruction>();
            foreach (var i in methodToAnalyze.Body.Instructions)
            {
                var newInstruction = CecilHelpers.MakeInstruction(i.OpCode, i.Operand);
                oldToNew.Add(i, newInstruction);
                p.Append(newInstruction);
            }

            foreach (var i in oldToNew.Values)
            {
                if (i.Operand is Instruction operand)
                {
                    if (oldToNew.TryGetValue(operand, out var replacement))
                        i.Operand = replacement;
                }
            }

            methodToAnalyze.DeclaringType.Methods.Add(clonedMethod);
        }

        public static Instruction FindInstructionThatPushedArg(MethodDefinition containingMethod, int argNumber,
            Instruction callInstructionsWhoseArgumentsWeWantToFind)
        {
            containingMethod.Body.EnsurePreviousAndNextAreSet();

            var cursor = callInstructionsWhoseArgumentsWeWantToFind.Previous;

            int stackSlotWhoseWriteWeAreLookingFor = argNumber;
            int stackSlotWhereNextPushWouldBeWrittenTo = InstructionExtensions.GetPopDelta(callInstructionsWhoseArgumentsWeWantToFind);

            var seenInstructions = new HashSet<Instruction>() {callInstructionsWhoseArgumentsWeWantToFind, cursor};

            while (cursor != null)
            {
                var pushAmount = cursor.GetPushDelta();
                var popAmount = cursor.GetPopDelta();

                var result = CecilHelpers.MatchesDelegateProducingPattern(containingMethod, cursor, CecilHelpers.DelegateProducingPattern.MatchSide.End);
                if (result != null)
                {
                    //so we are crawling backwards through isntructions.  if we find a "this is roslyn caching a delegate" sequence,
                    //we're going to pretend it is a single instruction, that pushes the delegate on the stack, and pops nothing.
                    cursor = result.Instructions.First();
                    pushAmount = 1;
                    popAmount = 0;
                } else if (cursor.IsBranch())
                {
                    var target = (Instruction) cursor.Operand;
                    if (!seenInstructions.Contains(target))
                    {
                        if (IsUnsupportedBranch(cursor))
                            UserError.DC0010(containingMethod, cursor).Throw();
                    }
                }


                for (int i = 0; i != pushAmount; i++)
                {
                    stackSlotWhereNextPushWouldBeWrittenTo--;
                    if (stackSlotWhereNextPushWouldBeWrittenTo == stackSlotWhoseWriteWeAreLookingFor)
                        return cursor;
                }

                for (int i = 0; i != popAmount; i++)
                {
                    stackSlotWhereNextPushWouldBeWrittenTo++;
                }

                cursor = cursor.Previous;
                seenInstructions.Add(cursor);
            }

            return null;
        }

        public static bool IsUnsupportedBranch(Instruction cursor)
        {
            if (cursor.OpCode.FlowControl == FlowControl.Next)
                return false;

            if (cursor.OpCode.FlowControl == FlowControl.Call)
                return false;

            return true;
        }

        public static MethodDefinition AddMethodImplementingInterfaceMethod(ModuleDefinition module, TypeDefinition type, System.Reflection.MethodInfo interfaceMethod)
        {
            var interfaceMethodReference = module.ImportReference(interfaceMethod);
            var newMethod = new MethodDefinition(interfaceMethodReference.Name,
                MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final | MethodAttributes.Public |
                MethodAttributes.HideBySig, interfaceMethodReference.ReturnType);

            int index = 0;
            foreach (var pd in interfaceMethodReference.Parameters)
            {
                var pdName = pd.Name;
                if (pdName.Length == 0)
                    pdName = interfaceMethod.GetParameters()[index].Name;
                newMethod.Parameters.Add(new ParameterDefinition(pdName, pd.Attributes, module.ImportReference(pd.ParameterType)));
                index++;
            }

            type.Methods.Add(newMethod);
            return newMethod;
        }
    }
}
