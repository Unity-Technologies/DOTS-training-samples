using System;

namespace Unity.PerformanceTesting
{
    [Obsolete("PerformanceTest has been deprecated. Use Test and Performance attributes instead. [PerformanceTest] -> [Test, Performance]", true)]
    public class PerformanceTestAttribute: Attribute
    {

    }

    [Obsolete("PerformanceUnityTest has been deprecated. Use UnityTest and Performance attributes instead. [PerformanceUnityTest] -> [UnityTest, Performance]", true)]
    public class PerformanceUnityTestAttribute: Attribute
    {

    }
}
