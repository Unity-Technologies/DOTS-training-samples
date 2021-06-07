using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeyboardInput : MonoBehaviour
{
    static bool showText = true;
    public Text infoText;
    public Text statusText;

    AntManager m_AntManager;

    void Start()
    {
        statusText.enabled = infoText.enabled = showText;
        m_AntManager = FindObjectOfType<AntManager>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            showText = !showText;
            statusText.enabled = infoText.enabled = showText;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (m_AntManager != null && Input.GetKeyDown(KeyCode.V))
            m_AntManager.addWallsToTexture ^= true;

        if (m_AntManager != null && Input.GetKeyDown(KeyCode.A))
            m_AntManager.renderAnts ^= true;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Time.timeScale = 1f;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Time.timeScale = 2f;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            Time.timeScale = 3f;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            Time.timeScale = 4f;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            Time.timeScale = 8;
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            Time.timeScale = 15f;
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            Time.timeScale = 25f;
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            Time.timeScale = 50;
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            Time.timeScale = 100f;

        if (Time.frameCount <= 1 || Time.frameCount % 100 == 0)
        {
            if (m_AntManager == null)
                statusText.text = "No stats, cannot find AntManager!";
            else
                statusText.text = m_AntManager.DumpStatusText();
        }
    }
}
