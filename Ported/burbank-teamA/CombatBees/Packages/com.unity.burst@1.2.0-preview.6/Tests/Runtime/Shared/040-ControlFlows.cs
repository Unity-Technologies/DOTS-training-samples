using System;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Burst.Compiler.IL.Tests
{
    internal class ControlFlows
    {
        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticIds = new[] { DiagnosticId.ERR_TryConstructionNotSupported, DiagnosticId.ERR_InstructionLeaveNotSupported })]
        public static int TryCatch()
        {
            try
            {
                return default(int);
            }
            catch (InvalidOperationException)
            {
                return 1;
            }
        }

        [TestCompiler]
        public static int For()
        {
            var counter = 0;
            for (var i = 0; i < 10; i++)
                counter++;
            return counter;
        }

        [TestCompiler(10)]
        public static int ForBreak(int a)
        {
            int result = 0;
            for (int i = 0; i < a; i++)
            {
                if (i == 5)
                {
                    break;
                }

                result += 2;
            }
            return result;
        }

        [TestCompiler(10, 5)]
        public static int ForContinue(int a, int b)
        {
            int result = 0;
            for (int i = 0; i < a; i++)
            {
                if (i == b)
                {
                    continue;
                }

                result += i;
            }
            return result;
        }

        [TestCompiler]
        public static int ForBreak2()
        {
            int i = 0;
            while (true)
            {
                if (i == 5)
                {
                    break;
                }

                i++;
            }
            return i;
        }

        [TestCompiler(10)]
        public static float ForDynamicCondition(ref int b)
        {
            var counter = 0.0f;
            for (var i = 0; i < b; i++)
                counter++;
            return counter;
        }

        [TestCompiler(5, 5)]
        public static int ForNestedIf(int a, int b)
        {
            var counter = 0;
            for (var i = 0; i < a; i++)
                for (var i2 = 0; i != b; i++)
                {
                    counter += i;
                    counter += i2;
                }
            return counter;
        }

        [TestCompiler(5, 5)]
        public static int DoWhileNested(int a, int b)
        {
            var total = 0;
            var counter2 = 0;
            do
            {
                var counter1 = 0;
                do
                {
                    total++;
                    counter1++;
                } while (counter1 < a);
                counter2++;
            } while (counter2 < b);

            return total;
        }

        [TestCompiler(5)]
        public static int While(int a)
        {
            var i = 0;
            var counter = 0;
            while (i < a)
            {
                i++;
                counter += i;
            }
            return counter;
        }

        [TestCompiler(5)]
        public static int ForForIf(int a)
        {
            var counter = 0;
            for (var i = 0; i != a; i++)
                for (var j = 0; j < 4; j++)
                    if (j > 2)
                        counter = counter + i;
            return counter;
        }

        [TestCompiler(5)]
        public static int ForNestedComplex1(int a)
        {
            var x = 0;
            var y = 0;
            for (var i = 0; i < a; i++)
            {
                y = y + 1;
                for (var j = 0; j < 4; j++)
                {
                    if (y > 1)
                    {
                        x = x + i;
                        if (x > 2)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                y = y + 1;
                                if (y > 3)
                                {
                                    x = x + 1;
                                }
                                else if (x > 6)
                                {
                                    y = 1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        x--;
                    }
                    x++;
                }
                if (y > 2)
                {
                    x = x + 1;
                }
            }
            return x;
        }

        [TestCompiler(5)]
        public static int ForNestedComplex2(int a)
        {
            var x = 0;
            for (var i = 0; i < a; i++)
            {
                var insideLoop1 = 0;
                for (var j = 0; j < 4; j++)
                {
                    x = x + i;
                    if (x > 2)
                    {
                        insideLoop1++;
                        for (int k = 0; k < 3; k++)
                        {
                            if (insideLoop1 > 3)
                            {
                                x = x + 1;
                            }
                            else if (x > 6)
                            {
                                break;
                            }
                        }
                    }
                }
                if (insideLoop1 > 2)
                {
                    x = x + 1 + insideLoop1;
                }
            }
            return x;
        }


        [TestCompiler(5)]
        [TestCompiler(-5)]
        public static int IfReturn(int a)
        {
            if (a < 0)
                return 55;
            return 111;
        }

        [TestCompiler(5)]
        [TestCompiler(-5)]
        public static int IfElseReturn(int a)
        {
            int b = 0;
            if (a < 0)
            {
                b = 1;
            }
            else
            {
                b = 2;
            }
            return b;
        }

        [TestCompiler(5)]
        [TestCompiler(-5)]
        public static int IfElseReturnDynamic(int a)
        {
            int b;
            if (a < 0)
            {
                b = a;
            }
            else
            {
                b = a + 1;
            }
            return b;
        }


        [TestCompiler(10)]
        public static int WhileFunction(int a)
        {
            while (condition_helper(a))
            {
                a--;
            }
            return a;
        }

        [TestCompiler(10)]
        public static int WhileDynamic(ref int a)
        {
            while (a > 2)
            {
                a--;
            }
            return a;
        }


        [TestCompiler(5, 6, 7)]
        [TestCompiler(-5, -6, -7)]
        public static int IfDeep(int a, int b, int c)
        {
            int result = 0;
            if (a < 0)
            {
                if (b > 1)
                {
                    if (c < 2)
                    {
                        result = 55;
                    }
                    else
                    {
                        result = 66;
                    }
                }
                else
                {
                    result = 77;
                }
            }
            else
            {
                if (b < 0)
                {
                    if (c < -2)
                    {
                        result = 88;
                    }
                    else
                    {
                        result = 99;
                    }
                }
                else
                {
                    result = 100;
                }
            }
            return result;
        }

        [TestCompiler(5)]
        public static int CallRecursive(int n)
        {
            return InternalCallRecursive(n);
        }

        private static int InternalCallRecursive(int n)
        {
            if (n <= 1)
                return 1;
            return n * InternalCallRecursive(n - 1);
        }

        [TestCompiler(3f, 8f)]
        [TestCompiler(6f, 8f)]
        public static float IfCompareFloat(float a, float b)
        {
            if (a > 5f)
                return 10f;
            return b;
        }

        [TestCompiler(10)]
        [TestCompiler(0)]
        public static float TernaryCompareFloat(int input)
        {
            return input > 5 ? 2.5f : 1.2F;
        }

        [TestCompiler(0)]
        [TestCompiler(1)]
        public static int TernaryMask(int a)
        {
            return (a & 1) != 0 ? 5 : 4;
        }

        [TestCompiler(0)]
        [TestCompiler(1)]
        public static int IfElseMash(int a)
        {
            if ((a & 1) != 0)
                return 5;
            else
                return 4;
        }

        [TestCompiler(0)]
        public static int IfCallCondition(int a)
        {
            if (a > 0 && condition_helper(++a))
            {
                return a;
            }
            return -10 + a;
        }

        [TestCompiler(1)]
        [TestCompiler(0)]
        [TestCompiler(-1)]
        public static int IfIncrementCondition(int a)
        {
            if (a < 0 || condition_helper(++a))
            {
                return a;
            }
            return -10 + a;
        }


        private static bool condition_helper(int value)
        {
            return value > 2;
        }

        [TestCompiler(1, 8)]
        public static int IfWhileGotoForward(int a, int b)
        {
            if (a > 0)
            {
                while (a < 10)
                {
                    a++;
                    if (a == b)
                    {
                        a--;
                        goto TestLabel;
                    }
                }
                a++;
            }
            TestLabel:
            a--;
            return a;
        }

        [TestCompiler(1, 5)]
        public static int IfWhileGotoBackward(int a, int b)
        {
            RewindLabel:
            if (a > 0)
            {
                while (a < 10)
                {
                    a++;
                    if (a == b)
                    {
                        a++;
                        goto RewindLabel;
                    }
                }
                a++;
            }
            a--;
            return a;
        }

        [TestCompiler(-1, 0)]
        [TestCompiler(0, 0)]
        [TestCompiler(0, -1)]
        public static int IfAssignCondition(int a, int b)
        {
            int result = 0;
            if (++a > 0 && ++b > 0)
            {
                result = a + b;
            }
            else
            {
                result = a * 10 + b;
            }
            return result;
        }


        private static bool ProcessFirstInt(int a, out float b)
        {
            b = a + 1;
            return b < 10;
        }

        private static bool ProcessNextInt(int a, ref float b)
        {
            b = a + 2;
            return b < 20;
        }

        [TestCompiler(1, 10)]
        public static float ForWhileNestedCall(int a, int b)
        {
            float value = 0;
            for (int i = 0; i < b * 3; i++)
            {
                var flag = ProcessFirstInt(a, out value);
                int num2 = 0;
                while (flag && num2 < 2)
                {
                    bool flag2 = i == a;
                    if (flag2)
                    {
                        flag = ProcessNextInt(a + i, ref value);
                    }
                    else
                    {
                        value++;
                        flag = ProcessNextInt(a + b + i, ref value);
                    }
                    num2++;
                }
            }
            return value;
        }

#if BURST_TESTS_ONLY
        [TestCompiler(true)]
        [TestCompiler(false)]
        public static bool CheckDup(bool value)
        {
            return ILTestsHelper.CheckDupBeforeJump(value);
        }
#endif

        [TestCompiler(1)]
        public static int WhileIfContinue(int a)
        {
            while (a > 10)
            {
                if (a < 5)
                {
                    a++;
                    if (a == 8)
                    {
                        continue;
                    }
                }
                a++;
            }
            return a;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticIds = new[] { DiagnosticId.ERR_TryConstructionNotSupported, DiagnosticId.ERR_InstructionEndfinallyNotSupported, DiagnosticId.WRN_DisablingNoaliasStoringImplicitNativeContainerToField })]
        public static int ForEachDispose()
        {
            var array = new NativeArray<int>();
            foreach (var item in array)
            {
                return item;
            }

            return 0;
        }

        [TestCompiler(0)]
        [TestCompiler(1)]
        [TestCompiler(2)]
        [TestCompiler(3)]
        [TestCompiler(4)]
        public static int SwitchReturn(int a)
        {
            switch (a)
            {
                case 1:
                    return 100;
                case 2:
                    return 200;
                case 3:
                    return 300;
                case 10:
                    return 300;
                default:
                    return 1000;
            }
        }

        [TestCompiler(0)]
        [TestCompiler(1)]
        [TestCompiler(2)]
        [TestCompiler(3)]
        [TestCompiler(4)]
        public static int SwitchBreak(int a)
        {
            switch (a)
            {
                case 1:
                    return 100;
                case 2:
                    break;
                default:
                    return 1000;
            }

            return 200;
        }

        [TestCompiler((byte)0)]
        [TestCompiler((byte)1)]
        [TestCompiler((byte)2)]
        [TestCompiler((byte)3)]
        [TestCompiler((byte)4)]
        public static int SwitchBreakByte(byte a)
        {
            switch (a)
            {
                case 1:
                    return 100;
                case 2:
                    break;
                default:
                    return 1000;
            }

            return 200;
        }

        public static byte GetValueAsByte(int a)
        {
            return (byte)a;
        }

        [TestCompiler(0)]
        [TestCompiler(1)]
        [TestCompiler(2)]
        [TestCompiler(3)]
        public static byte SwitchByteReturnFromFunction(int a)
        {
            switch (GetValueAsByte(a))
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                default:
                    return 0;
            }
        }

        [TestCompiler(long.MaxValue)]
        [TestCompiler(long.MinValue)]
        [TestCompiler(0)]
        public static byte SwitchOnLong(long a)
        {
            switch (a)
            {
                case long.MaxValue:
                    return 1;
                case long.MinValue:
                    return 2;
                default:
                    return 0;
            }
        }

        public static byte TestSwitchByteReturn(NativeArray<byte> _results, int a)
        {
            if (_results.Length > a)
            {
                switch (_results[a])
                {
                    case 0:
                        return 1;
                    case 1:
                        return 2;
                    case 2:
                        return 3;
                    default:
                        return 0;
                }
            }
            return 99;
        }

        [TestCompiler(EnumSwitch.Case1)]
        [TestCompiler(EnumSwitch.Case2)]
        [TestCompiler(EnumSwitch.Case3)]
        public static int SwitchEnum(EnumSwitch a)
        {
            switch (a)
            {
                case EnumSwitch.Case1:
                    return 100;
                case EnumSwitch.Case3:
                    break;
                default:
                    return 1000;
            }

            return 200;
        }

        public enum EnumSwitch
        {
            Case1,

            Case2,

            Case3,
        }

        [TestCompiler(ExpectedException = typeof(InvalidOperationException))]
        [MonoOnly(".NET CLR does not support burst.abort correctly")]
        public static int ExceptionReachedReturn()
        {
            throw new InvalidOperationException("This is bad 1");
        }

        [TestCompiler(ExpectedException = typeof(InvalidOperationException))]
        [MonoOnly(".NET CLR does not support burst.abort correctly")]
        public static void ExceptionReached()
        {
            throw new InvalidOperationException("This is bad 2");
        }

        [TestCompiler(1)]
        [TestCompiler(2)]
        public static void ExceptionNotReached(int a)
        {
            if (a > 10)
            {
                throw new InvalidOperationException("This is bad 2");
            }
        }

        [TestCompiler(1)]
        [TestCompiler(2)]
        public static void ExceptionMultipleNotReached(int a)
        {
            if (a > 10)
            {
                if (a > 15)
                {
                    throw new InvalidOperationException("This is bad 2");
                }
                else
                {
                    if (a < 8)
                    {
                        throw new NotSupportedException();
                    }
                    else
                    {
                        a = a + 1;
                    }
                }
            }
        }


        [TestCompiler(1)]
        [TestCompiler(2)]
        public static int ExceptionNotReachedReturn(int a)
        {
            int b = a;
            if (a > 10)
            {
                b = 5;
                throw new InvalidOperationException("This is bad 2");
            }
            return b;
        }


        [TestCompiler(13)]
        [TestCompiler(1)]
        public static int ExceptionMultipleNotReachedReturn(int a)
        {
            if (a > 10)
            {
                if (a > 15)
                {
                    throw new InvalidOperationException("This is bad 2");
                }
                else
                {
                    if (a < 12)
                    {
                        throw new NotSupportedException();
                    }
                    else
                    {
                        a = a + 1;
                    }
                }
            }
            return a;
        }

        [TestCompiler]
        public static void TestInternalError()
        {
            var job = new InternalErrorVariableNotFound();
            job.Execute();
        }

        public struct InternalErrorVariableNotFound : IJob
        {
            public void Execute()
            {
                CausesError(3);
            }

            static int CausesError(int x)
            {
                int y = 0;
                while (y != 0 && y != 1)
                {
                    if (x > 0)
                        x = y++;
                }
                return y;
            }
        }
        
        [TestCompiler(true)]
        public static int TestPopNonInitialTrailingPush(bool x)
        {
            return (x ? 1 : -1) * math.min(16, 1);
        }

        [TestCompiler]
        // Check unsigned ternary comparison (Bxx_Un) opcodes
        public static ulong TestUnsignedTernary()
        {
            ulong a = 0;
            ulong b = ~0UL;
            ulong c = (a < b) ? 1UL : 0;
            ulong d = (a <= b) ? 1UL : 0;
            ulong e = (a > b) ? 0: 1UL;
            ulong f = (a >= b) ? 0: 1UL;

            return c + d + e + f;
        }

    }
}
