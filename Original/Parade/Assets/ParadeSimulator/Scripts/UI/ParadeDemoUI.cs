using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Very basic UI to show test progress during Parade testing.
/// </summary>
public class ParadeDemoUI : MonoBehaviour {

    private bool drawUI = true;
    private Text configNameLabel = null;
    private Text speedLabel = null;
    private Text currentBufferBlockLabel = null;
    private Text pressSpaceToStartLabel = null;
    CityStreamManager myCityStreamManager = null;
    CameraMover myCameraMover = null;

    void Start()
    {

        configNameLabel = gameObject.transform.Find("DemoCanvas/ConfigNameLabel").GetComponent<Text>();
        speedLabel = gameObject.transform.Find("DemoCanvas/SpeedLabel").GetComponent<Text>();
        currentBufferBlockLabel = gameObject.transform.Find("DemoCanvas/CurrentBufferBlockLabel").GetComponent<Text>();
        pressSpaceToStartLabel = gameObject.transform.Find("DemoCanvas/PressSpaceToStartLabel").GetComponent<Text>();

        if (!configNameLabel || !speedLabel || !currentBufferBlockLabel || !pressSpaceToStartLabel)
        {
            Debug.LogWarning("ParadeDemoUI:: Cannot find one or more UI components. UI will not draw properly.");
        }

        myCityStreamManager = CityStreamManager.Instance;
        myCameraMover = CameraMover.Instance;

        if (!myCityStreamManager || !myCameraMover)
        {
            Debug.LogError("ParadeDemoUI:: Cannot find CityStreamManager and/or CameraMover in this scene. Demo will not function correctly!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        if (myCityStreamManager.EnableTestUI)
        {

            drawUI = true;
            configNameLabel.enabled = true;
            speedLabel.enabled = true;
            currentBufferBlockLabel.enabled = true;
            pressSpaceToStartLabel.enabled = true;

            // UI Elements that don't change (will reduce "performance artifacts" due to unrequired UI updates)
            configNameLabel.text = "Test Name: '" + myCityStreamManager.CurrentConfigName + "'";

        }
        else
        {

            drawUI = false;
            configNameLabel.enabled = false;
            speedLabel.enabled = false;
            currentBufferBlockLabel.enabled = false;
            pressSpaceToStartLabel.enabled = false;

        }

    }
	
	void Update ()
    {

        if (myCityStreamManager.Started)
        {

            if (drawUI)
            {

                speedLabel.text = "Speed: " + myCityStreamManager.CityMovementSpeed;
                currentBufferBlockLabel.text = "Block: " + myCityStreamManager.CurrentCityBlock + " of " + ((myCityStreamManager.TestBlocksToTravel > 0) ? ""+myCityStreamManager.TestBlocksToTravel : "INFINITY");
                pressSpaceToStartLabel.text = "";

            }

        }

	}

}
