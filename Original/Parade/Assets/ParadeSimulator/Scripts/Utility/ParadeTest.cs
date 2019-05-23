using System;

/// <summary>
/// Basic container that houses all Parade Test data so it can be cleany passed to and from various objects.
/// </summary>
public class ParadeTest {

    private ParadeConfiguration testConfiguration;
    public ParadeConfiguration TestConfiguration {
        get { return testConfiguration; }
    }

    private float elapsedTestTime = 0.0f;
    public float ElapsedTestTime {
        get { return elapsedTestTime; }
    }

    private int testFrameCount = 0;
    public int TestFrameCount {
        get { return testFrameCount; }
    }

    private DateTime testDate;
    public DateTime TestDate {
        get { return testDate; }
    }

    private bool testAborted = false;

    public ParadeTest(ParadeConfiguration config)
    {
        testConfiguration = config;
    }

    public void SetTestResults(float elapsedTime, int frameCount, bool aborted)
    {

        testDate = DateTime.Now;
        elapsedTestTime = elapsedTime;
        testFrameCount = frameCount;
        testAborted = aborted;

    }

    public bool hasUsefulData()
    {
        return (testAborted == false);
    }

    public string FetchResultDisplayString()
    {

        string displayString = "";

        if (testAborted)
        {
            displayString = "Test '" + testConfiguration.ConfigName + "' Aborted.";
        }
        else
        {
            displayString = "Test '" + testConfiguration.ConfigName + "' Complete. Traveled " + testConfiguration.NumberOfTestBlocks + " blocks at speed " + testConfiguration.CameraSpeed + " in " + elapsedTestTime + "s(" + testFrameCount + " frames) =>Avg: " + (testFrameCount / elapsedTestTime) + "fps.";
        }

        return displayString;

    }

}
