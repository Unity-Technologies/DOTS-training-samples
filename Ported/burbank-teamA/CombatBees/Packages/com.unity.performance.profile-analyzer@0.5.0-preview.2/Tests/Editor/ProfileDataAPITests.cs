using NUnit.Framework;
using UnityEditor.Performance.ProfileAnalyzer;
using System.Collections.Generic;

public class ProfileDataAPITests : ProfileAnalyzerBaseTest
{
    [Test]
    public void ProfileData_AddMarkerName_AddsMarkerAndContainsName()
    {
        var data = new ProfileData();
        var markerNames = new List<string>()
                                {
                                    "Marker01",
                                    "Marker02",
                                    "Marker03",
                                    "Marker04"
                                };

        var markerList = new List<ProfileMarker>();
        for (int i = 0; i < 10; ++i)
        {
            var marker = new ProfileMarker()
            {
                msMarkerTotal = 0.5f,
                depth = i
            };

            int expectedIndex = i % markerNames.Count;
            data.AddMarkerName(markerNames[expectedIndex], marker);
            markerList.Add(marker);

            Assert.IsTrue(expectedIndex == marker.nameIndex, "Index mismatch at: " + i + " , " + marker.nameIndex); ;
        }
        
        for(int i = 0; i < markerList.Count; ++i)
        {
            var curName = data.GetMarkerName(markerList[i]);
            Assert.IsTrue(markerNames.Contains(curName));
        }
    }

    [Test]
    public void ProfileData_AddThreadName_AddsThreadAndContainsName()
    {
        var data = new ProfileData();
        var threadNames = new List<string>()
                                {
                                    "Thread01",
                                    "Thread02",
                                    "Thread03",
                                    "Thread04"
                                };

        var threadDict = new Dictionary<string, ProfileThread>();
        for (int i = 0; i < 10; ++i)
        {
            int expectedIndex = i % threadNames.Count;
            string threadName = threadNames[expectedIndex];
            ProfileThread thread;

            if(!threadDict.TryGetValue(threadName, out thread))
            {
                thread = new ProfileThread();
                threadDict.Add(threadName, thread);
            }

            var marker = new ProfileMarker()
            {
                msMarkerTotal = 0.5f,
                depth = i
            };

            thread.Add(marker);
            data.AddThreadName(threadName, thread);
            Assert.IsTrue(expectedIndex == thread.threadIndex, "Index mismatch at: " + i + " , " + thread.threadIndex); ;
        }

        foreach(var curThread in threadDict)
        {
            var curName = data.GetThreadName(curThread.Value);
            Assert.IsTrue(threadNames.Contains(curName));
        }
    }
}
