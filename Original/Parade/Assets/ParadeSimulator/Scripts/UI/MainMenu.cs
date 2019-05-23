using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles core logic for MainMenu scene and associated prefab.
/// </summary>
public class MainMenu : MonoBehaviour {

    private static MainMenu _instance;
    public static MainMenu Instance {
        get { return _instance; }
    }

    private Text testResultsLabel = null;
    private Text currentTestNameLabel = null;
    private Button saveButton = null;
    private Button clearButton = null;
    private Toggle autosaveToggle = null;
    private bool initialized = false;

    private bool currentResultsSaved = false;
    private bool resultsCleared = false;

    void Awake()
    {

        if (_instance != null)
        {
            Debug.Log("MainMenu:: Duplicate instance of MainMenu, deleting second instance.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            initialize();
        }

    }

    private void initialize()
    {

        if (!initialized)
        {

            initialized = true;

            testResultsLabel = gameObject.transform.Find("MenuCanvas/TestResultsLabel").GetComponent<Text>();
            currentTestNameLabel = gameObject.transform.Find("MenuCanvas/TestSelector/CurrentTestNameLabel").GetComponent<Text>();
            saveButton = gameObject.transform.Find("MenuCanvas/SaveResultsButton").GetComponent<Button>();
            clearButton = gameObject.transform.Find("MenuCanvas/ClearResultsButton").GetComponent<Button>();
            autosaveToggle = gameObject.transform.Find("MenuCanvas/AutosaveToggle").GetComponent<Toggle>();

            if (!testResultsLabel || !currentTestNameLabel || !saveButton || !clearButton || !autosaveToggle)
            {
                Debug.LogError("MainMenu:: One or more UI components is missing! Menu will not function properly.");
            }

            autosaveToggle.onValueChanged.AddListener(delegate {
                ToggleValueChanged(autosaveToggle);
            });

            saveButton.interactable = false;

        }

    }

    private void Start()
    {
        currentTestNameLabel.text = TestManager.Instance.getCurrentTestName();
    }

    public void startTest()
    {
        TestManager.Instance.startNewTest();
    }

    public void selectNextTest()
    {
        currentTestNameLabel.text = TestManager.Instance.nextTest();
    }

    public void selectPreviousTest()
    {
        currentTestNameLabel.text = TestManager.Instance.previousTest();
    }

    public void refreshTestResults()
    {
        testResultsLabel.text = "Latest Test Results:\n" + TestManager.Instance.getLastTestResultsToDisplay();
    }

    public void saveTestResults()
    {

        if (currentResultsSaved == false)
        {

            TestManager.Instance.saveLatestResultToFile();
            currentResultsSaved = true;
            saveButton.interactable = false;
            saveButton.GetComponentInChildren<Text>().text = "Saved.";

        }

    }

    public void clearTestResults()
    {

        if(resultsCleared == false)
        {

            TestManager.Instance.clearOldTestResults();
            resultsCleared = true;
            clearButton.interactable = false;
            clearButton.GetComponentInChildren<Text>().text = "Cleared.";

        }
        
    }

    public void endDemo()
    {
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    private void ToggleValueChanged(Toggle myToggle)
    {
        TestManager.Instance.Autosave = myToggle.isOn;
    }

    public void enableAutoSave()
    {

        saveButton.interactable = false;
        autosaveToggle.isOn = true;

    }

    public void disableAutoSave()
    {

        saveButton.interactable = true;
        autosaveToggle.isOn = false;

    }

}
