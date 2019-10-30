using NUnit.Framework;
using System.Collections.Generic;

public class ProfileAnalyzerEmptyTests : ProfileAnalyzerBaseTest
{
    List<int> SelectRange(int startIndex, int endIndex)
    {
        List<int> list = new List<int>();
        for (int c = startIndex; c <= endIndex; c++)
        {
            list.Add(c);
        }

        return list;
    }

    [Test]
    public void ProfileAnalyzer_EmptyData_IsEmpty()
    {
        var analyzer = m_setupData.analyzer;
        var profileData = analyzer.PullFromProfiler(0, 300);
        var depthFilter = m_setupData.depthFilter;
        var threadFilters = m_setupData.threadFilters;

        int firstFrameIndex = profileData.OffsetToDisplayFrame(0);
        int lastFrameIndex  = profileData.OffsetToDisplayFrame(profileData.GetFrameCount() - 1);

        Assert.AreEqual(0, firstFrameIndex, "First Frame index not zero");
        Assert.AreEqual(300, lastFrameIndex, "Last Frame index is not 300");

        var analysis = analyzer.Analyze(profileData, SelectRange(firstFrameIndex, lastFrameIndex), threadFilters, depthFilter);
        var frameSummary = analysis.GetFrameSummary();

        Assert.AreEqual(0, analysis.GetThreads().Count);
        Assert.AreEqual(0, frameSummary.msTotal);
        Assert.AreEqual(0, frameSummary.first);
        Assert.AreEqual(300, frameSummary.last);
        Assert.AreEqual(301, frameSummary.count);
        Assert.AreEqual(0, frameSummary.msMean);
        Assert.AreEqual(0, frameSummary.msMedian);
        Assert.AreEqual(0, frameSummary.msLowerQuartile);
        Assert.AreEqual(0, frameSummary.msUpperQuartile);
        Assert.AreEqual(0, frameSummary.msMin);
        Assert.AreEqual(0, frameSummary.msMax);
        Assert.AreEqual(150, frameSummary.medianFrameIndex);
        Assert.AreEqual(0, frameSummary.minFrameIndex);
        Assert.AreEqual(0, frameSummary.maxFrameIndex);
        Assert.AreEqual(0, frameSummary.maxMarkerDepth);
        Assert.AreEqual(0, frameSummary.totalMarkers);
    }
}
