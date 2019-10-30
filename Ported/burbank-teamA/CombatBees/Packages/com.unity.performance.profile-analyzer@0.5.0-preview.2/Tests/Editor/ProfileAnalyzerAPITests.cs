using NUnit.Framework;

public class ProfileAnalyzerAPITests : ProfileAnalyzerBaseTest
{
    [Test]
    public void ProfileAnalyzer_EmptyAnalysis_HasNoThreads()
    {
        var analyzer = m_setupData.analyzer;
        Assert.IsTrue(0 == analyzer.GetThreadNames().Count);
    }

    [Test]
    public void ProfileAnalyzer_EmptyAnalysis_HasNoProgress()
    {
        var analyzer = m_setupData.analyzer;
        Assert.IsTrue(0 == analyzer.GetProgress());
    }

    [Test]
    public void ProfileAnalyzer_EmptyAnalysis_ReturnsNullForAnalysis()
    {
        var analysis = GetAnalysisFromFrameData(null);
        Assert.IsNull(analysis);
    }

}
