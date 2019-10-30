using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.TestTools;

[TestFixture]

public class EditModeTest
{
    private bool jobCompilerStatusStorage;
    
    [SetUp]
    public void Setup()
    {
        jobCompilerStatusStorage = JobsUtility.JobCompilerEnabled;
    }
    
    [UnityTest]
    public IEnumerator CheckBurstJobEnabled()
    {
        JobsUtility.JobCompilerEnabled = true;

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        using (var jobTester = new BurstJobTester())
        {
            var result = jobTester.Calculate();
            Assert.AreNotEqual(0.0f, result);
        }
    }

#if UNITY_BURST_BUG_FUNCTION_POINTER_FIXED
    [UnityTest]
    public IEnumerator CheckBurstFunctionPointerException()
    {
        JobsUtility.JobCompilerEnabled = true;

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        using (var jobTester = new BurstJobTester())
        {
            var exception = Assert.Throws<InvalidOperationException>(() => jobTester.CheckFunctionPointer());
            StringAssert.Contains("Exception was thrown from a function compiled with Burst", exception.Message);
        }
    }
#endif

    [UnityTest]
    public IEnumerator CheckBurstJobDisabled()
    {
        JobsUtility.JobCompilerEnabled = false;
        
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        
        using (var jobTester = new BurstJobTester())
        {
            var result = jobTester.Calculate();
            Assert.AreEqual(0.0f, result);
        }
    }

    [TearDown]
    public void Restore()
    {
        JobsUtility.JobCompilerEnabled = jobCompilerStatusStorage;
    }
}
