using UnityEngine;
using System;
using System.IO;

/// <summary>
/// A very basic file IO class to handle writing Parade Demo test result to a csv file.
/// </summary>
public class FileWriter : MonoBehaviour {

    private string filePath = "";
    private string directoryPath = "";
    private string baseFileHeadings = "Date, Test Name, Environment, Test Duration, Frame Count, Average Frame Rate" + Environment.NewLine;
    private string dateFormat = "yyyy-MM-dd HH:mm:ss";

    private void Awake()
    {

        directoryPath = Application.persistentDataPath + "/" + ParadeConstants.ParadeResultsBasePath;
        filePath = directoryPath + "/" + ParadeConstants.ParadeResultsFilename;
        ensureSafeFileStructure();

    }

    /// <summary>
    /// Writes a pretty-printed .csv compatible string to data file
    /// </summary>
    /// <param name="test">The test whose results will be captured in the file change</param>
    public void WriteParadeTestResultToFile(ParadeTest test)
    {

        if (!File.Exists(filePath))
        {
            createBaseFile();
        }

        string result = "";

        result += test.TestDate.ToString(dateFormat) + ",";

        ParadeConfiguration testConfig = test.TestConfiguration;

        result += testConfig.ConfigName + ",";

#if UNITY_EDITOR
        result += "Editor" + ",";
#elif DEVELOPMENT_BUILD
        result += "Development Build" + ",";
#else
        result += "Production Build" + ",";
#endif

        result += test.ElapsedTestTime + ",";
        result += test.TestFrameCount + ",";
        result += (test.TestFrameCount / test.ElapsedTestTime) + "";

        result += Environment.NewLine;

        try
        {
            File.AppendAllText(filePath, result);
        }
        catch (Exception e)
        {
            Debug.LogError("FileWriter.WriteParadeTestResultToFile():: Exception ocurred: '" + e.Message + "'");
        }
        finally
        {
        }

    }

    /// <summary>
    /// Ensures that the file system can be written to. If it cannot be, then no file I/O will work.
    /// </summary>
    private void ensureSafeFileStructure()
    {

        try
        {

            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            if(File.Exists(filePath) == false)
            {
                createBaseFile();
            }

        }
        catch(Exception e)
        {
            Debug.LogError("FileWriter.ensureSafeFileStructure():: Exception ocurred: '" + e.Message + "'");
        }
        finally
        {
        }

    }

    /// <summary>
    /// Clears existing saved data and creates fresh file with column headings and no results data
    /// </summary>
    public void clearSavedData()
    {
        try
        {

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            createBaseFile();

        }
        catch(Exception e)
        {
            Debug.LogError("FileWriter.clearSavedData():: Exception ocurred: '" + e.Message + "'");
        }
        finally
        {
        }

    }

    /// <summary>
    /// Creates a blank csv results file with headings but no data.
    /// </summary>
    private void createBaseFile()
    {

        try
        {
            File.WriteAllText(filePath, baseFileHeadings);
        }
        catch(Exception e)
        {
            Debug.LogError("FileWriter.createBaseFile():: Exception ocurred: '" + e.Message + "'");
        }
        finally
        {
        }

    }

}

