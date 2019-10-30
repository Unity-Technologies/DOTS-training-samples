using NUnit.Framework;
using UnityEditor.Performance.ProfileAnalyzer;

public class MarkerDataAPITests : ProfileAnalyzerBaseTest
{
    [Test]
    public void MarkerData_ComputeBuckets_GeneratesExpectedValues()
    {
        var marker = new MarkerData("Test Marker");
        marker.presentOnFrameCount = 1;

        for (int i = 0; i < 20; ++i)
        {
            var frameTime = new FrameTime(1, 1f * i, 1);
            marker.frames.Add(frameTime);
        }

        marker.ComputeBuckets(0, 20);

        Assert.IsTrue(2 == marker.buckets[0]);

        for (int i = 1; i < marker.buckets.Length - 1; ++i)
        {
            Assert.IsTrue(1 == marker.buckets[i]);
        }

        Assert.IsTrue(0 == marker.buckets[19]);
    }
}

