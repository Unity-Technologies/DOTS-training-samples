using System;
using System.Reflection;
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;
#if BURST_TESTS_ONLY
using Burst.Compiler.IL.Jit;
#endif

namespace Burst.Compiler.IL.Tests
{
    /// <summary>
    /// Tests related to usage of partial managed objects (e.g loading null or storing null
    /// reference to a struct, typically used by NativeArray DisposeSentinel)
    /// </summary>
    internal class PartialManaged
    {
#if BURST_TESTS_ONLY || ENABLE_UNITY_COLLECTIONS_CHECKS
        [TestCompiler()]
        public static int TestWriteNullReference()
        {
            var element = new Element();
            WriteNullReference(out element.Reference);
            return element.Value;
        }

        private static void WriteNullReference(out DisposeSentinel reference)
        {
            reference = null;
        }

        private struct Element
        {
#pragma warning disable 0649
            public int Value;
            public DisposeSentinel Reference;
#pragma warning restore 0649

        }
#endif

        [TestCompiler()]
        public static void AssignNullToLocalVariableClass()
        {
            MyClass x = null;
#pragma warning disable 0219
            MyClass value = x;
#pragma warning restore 0219
        }

#if BURST_TESTS_ONLY
        [Test]
        public void TestThrowExceptionOnNonExistingMethod()
        {
            MethodInfo info = typeof(PartialManaged).GetMethod("NonExistingMethod");
            JitCompiler compiler = new JitCompiler();
            Assert.Throws<ArgumentNullException>(() => compiler.CompileMethod(info));
        }
#endif

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_InstructionLdstrNotSupported)]
        public static int GetIndexOfCharFomString()
        {
            return "abc".IndexOf('b');
        }

        struct StructWithManaged
        {
#pragma warning disable 0649
            public MyClass myClassValue;
            public string stringValue;
            public object objectValue;
            public float[] arrayValue;

            public int value;
#pragma warning restore 0649
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_LoadingFieldWithManagedTypeNotSupported)]
        public static int AccessClassFromStruct()
        {
            var val = new StructWithManaged();
            val.myClassValue.value = val.value;
            return val.myClassValue.value;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_InstructionLdstrNotSupported)]
        public static void AccessStringFromStruct()
        {
            var val = new StructWithManaged();
#pragma warning disable 0219
            var p = val.stringValue = "abc";
#pragma warning restore 0219
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_LoadingFieldWithManagedTypeNotSupported)]
        public static void AccessObjectFromStruct()
        {
            var val = new StructWithManaged();
#pragma warning disable 0219
            var p = val.objectValue;
            p = new object();
#pragma warning restore 0219
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_LoadingFieldWithManagedTypeNotSupported)]
        public static void AccessArrayFromStruct()
        {
            var val = new StructWithManaged();
            var p = val.arrayValue;
            p[0] = val.value;
        }

        [TestCompiler]
        public static int GetValueFromStructWithClassField()
        {
            var val = new StructWithManaged();
            val.value = 5;

            return val.value;
        }

        [TestCompiler(ExpectCompilerException = true, ExpectedDiagnosticId = DiagnosticId.ERR_InstructionNewobjWithManagedTypeNotSupported)]
        public static void NewMyClass()
        {
#pragma warning disable 0219
            var value = new MyClass();
#pragma warning restore 0219
        }

        private class MyClass
        {
            public int value;
        }
    }
}
