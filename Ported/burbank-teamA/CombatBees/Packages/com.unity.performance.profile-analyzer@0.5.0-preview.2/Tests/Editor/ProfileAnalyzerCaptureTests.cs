using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;

public class ProfileAnalyzerCaptureTests : ProfileAnalyzerBaseTest
{
    [UnityTest]
    public IEnumerator PlayMode_Capture_ContainsNoDuplicates()
    {
        StartProfiler();

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        StopProfiler();

        //analyze the data
        m_setupData.profileData = m_setupData.analyzer.PullFromProfiler(m_setupData.firstFrame, m_setupData.lastFrame);
        var analysis = GetAnalysisFromFrameData(m_setupData.profileData);
        
        var analysisMarkers = analysis.GetMarkers();
        var analysisMarkerDict = new Dictionary<string, int>();
        for (int i = 0; i < analysisMarkers.Count; ++i)
        {
            int count = 0;
            string curName = analysisMarkers[i].name;

            analysisMarkerDict.TryGetValue(curName, out count);

            analysisMarkerDict[curName] = count + 1;
        }

        Assert.IsTrue(0 != analysisMarkerDict.Count, "analysisMarkerSet count is zero!");

        foreach (var entry in analysisMarkerDict)
        {
            Assert.IsTrue(1 == entry.Value, "Duplicates found in analysis marker list: " + entry.Key);
        }
    }
}
