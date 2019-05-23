using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Core logic for running Parade tests. Tracks current test case, and relays test parameters.
/// </summary>
public class TestManager : MonoBehaviour {

    private static TestManager _instance;
    public static TestManager Instance {
        get { return _instance; }
    }

    private ParadeConfiguration[] testConfigurations = null;
    private int testIndex = 0;
    private bool initialized = false;
    private ParadeTest latestParadeTest = null;

    private bool autosave = true;
    public bool Autosave {

        get { return autosave; }

        set {

            autosave = value;

            if (autosave)
            {
                if(MainMenu.Instance != null)
                {
                    MainMenu.Instance.enableAutoSave();
                }
            }
            else
            {
                if (MainMenu.Instance != null)
                {
                    MainMenu.Instance.disableAutoSave();
                }
            }

        }

    }

    void Awake()
    {

        if (_instance != null)
        {
            //Debug.Log("TestManager:: Duplicate instance of TestManager, deleting second instance.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            initialize();
        }

    }

    private void initialize()
    {

        if (!initialized)
        {

            initialized = true;
            loadAllConfigurationPresets();

        }

    }

    public void giveTestResults(float elapsedTime, int frameCount, bool aborted)
    {

        latestParadeTest.SetTestResults(elapsedTime, frameCount, aborted);
        SceneManager.LoadScene("MainMenu");

    }

    /// <summary>
    /// Returns raw string of last test result, including aborted tests.
    /// </summary>
    /// <returns>String to display</returns>
    public string getLastTestResultsToDisplay()
    {

        string latestResult = "None";

        if (latestParadeTest != null)
        {
            latestResult =  latestParadeTest.FetchResultDisplayString();
        }

        return latestResult;
        
    }

    public void saveLatestResultToFile()
    {

        if (latestParadeTest != null && latestParadeTest.hasUsefulData())
        {
            gameObject.GetComponent<FileWriter>().WriteParadeTestResultToFile(latestParadeTest);
        }

    }

    public void clearOldTestResults()
    {
        gameObject.GetComponent<FileWriter>().clearSavedData();
    }

    public void startNewTest()
    {
        SceneManager.LoadScene("Parade");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if(scene.name == "MainMenu")
        {

            if (autosave)
            {

                MainMenu.Instance.enableAutoSave();
                saveLatestResultToFile();

            }
            else
            {
                MainMenu.Instance.disableAutoSave();
            }

            MainMenu.Instance.refreshTestResults();

        }
        else if (scene.name == "Parade")
        {

            latestParadeTest = new ParadeTest(testConfigurations[testIndex]);
            CityStreamManager.Instance.ConfigurationPreset = testConfigurations[testIndex];
            CityStreamManager.Instance.beginTest();

        }

    }

    private void loadAllConfigurationPresets()
    {
        testConfigurations = Resources.LoadAll<ParadeConfiguration>("ConfigurationPresets/");
    }

    public string nextTest()
    {

        testIndex += 1;

        if(testIndex >= testConfigurations.Length)
        {
            testIndex = 0;
        }

        return testConfigurations[testIndex].ConfigName;

    }

    public string previousTest()
    {

        testIndex -= 1;

        if (testIndex < 0)
        {
            testIndex = testConfigurations.Length-1;
        }

        return testConfigurations[testIndex].ConfigName;

    }

    public string getCurrentTestName()
    {
        return testConfigurations[testIndex].ConfigName;
    }

}
