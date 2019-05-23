using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private int _currentTimeIndex = 0;
    public float[] TimeScales = new float[] { 1.0f };

    [Header("Top HUD")]
    public Text NumberOfCarsText;
    public Text PeakNumberOfCarsText;
    public Button SwitchCameraButton;
    public Button ToggelTimeScaleButton;
    public Text ToggelTimeScaleButtonText;

    public Text QuadStatusText;
    public Button NextQuadButton;
    public Button PreviousQuadButton;

    [Header("Bottom HUD")]
    public Text TrafficStatusText;
    public static UIManager Instance { get; private set; }

    #region Unity Callbacks
    private void Awake()
    {
        Instance = this;
        SetTimeScale();
    }

    private void OnEnable()
    {
        SwitchCameraButton.onClick.AddListener(SwitchCamera);
        NextQuadButton.onClick.AddListener(OnNextQuadButtonClick);
        ToggelTimeScaleButton.onClick.AddListener(OnToggleTimeClick);
        PreviousQuadButton.onClick.AddListener(OnPreviousQuadButtonClick);
    }

    private void OnDisable()
    {
        SwitchCameraButton.onClick.RemoveListener(SwitchCamera);
        NextQuadButton.onClick.AddListener(OnNextQuadButtonClick);
        ToggelTimeScaleButton.onClick.RemoveListener(OnToggleTimeClick);
        PreviousQuadButton.onClick.AddListener(OnPreviousQuadButtonClick);
    }

    private void OnToggleTimeClick()
    {
        _currentTimeIndex++;
        _currentTimeIndex %= TimeScales.Length;
        SetTimeScale();
    }

    private void SetTimeScale()
    {
        Time.timeScale = TimeScales[_currentTimeIndex];
        ToggelTimeScaleButtonText.text = string.Format("Time scale: {0:0.0}", Time.timeScale);
    }

    private void SwitchCamera()
    {
        CameraController.Instance.SwitchCamera();
    }

    private void OnNextQuadButtonClick()
    {
        GameManager.Instance.GoToNextQuad();
        SetQuadStatusText();
    }

    private void OnPreviousQuadButtonClick()
    {
        GameManager.Instance.GoToPreviousQuad();
        SetQuadStatusText();
    }
    #endregion

    #region Public API
    public void SetCarCountText(string activeCarsText, string peakText)
    {
        NumberOfCarsText.text = activeCarsText;
        PeakNumberOfCarsText.text = peakText;
    }

    public void SetQuadButtons(int quadCount)
    {
        NextQuadButton.interactable = quadCount > 0;
        PreviousQuadButton.interactable = quadCount > 0;
        SetQuadStatusText();
    }

    private void SetQuadStatusText()
    {
        QuadStatusText.text = string.Format("Quad {0} of {1}", GameManager.Instance.ActiveSpawnManagerIndex + 1, GameManager.Instance.QuadCount);
    }
    #endregion
}
