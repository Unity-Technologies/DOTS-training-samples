using System;
using NUnit.Framework;
using Unity.Burst;

[TestFixture, BurstCompile]
public class FunctionPointerTests
{
    [BurstCompile(CompileSynchronously = true)]
    private static T StaticFunctionNoArgsGenericReturnType<T>()
    {
        return default;
    }

    private delegate int DelegateNoArgsIntReturnType();

    [Test]
    public void TestCompileFunctionPointerNoArgsGenericReturnType()
    {
        Assert.Throws<InvalidOperationException>(
            () => BurstCompiler.CompileFunctionPointer<DelegateNoArgsIntReturnType>(StaticFunctionNoArgsGenericReturnType<int>),
            "The method `Int32 StaticFunctionNoArgsGenericReturnType[Int32]()` must be a non-generic method");
    }
}
