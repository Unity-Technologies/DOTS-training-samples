using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KeyboardInput : MonoBehaviour
{
    static bool showText = true;
    public Text infoText;
    public Text statusText;
    ClientAntRenderSystem m_ClientAntRenderSystem;
    AntSimulationSystem m_AntSimulationSystem;

    void Start()
    {
        statusText.enabled = infoText.enabled = showText;
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

        m_AntSimulationSystem ??= FindSystemInAllWorlds<AntSimulationSystem>();
        m_ClientAntRenderSystem ??= FindSystemInAllWorlds<ClientAntRenderSystem>();

        if (m_AntSimulationSystem != null)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                var simParams = m_AntSimulationSystem.GetSingleton<AntSimulationParams>();
                simParams.addWallsToTexture ^= true;
                m_AntSimulationSystem.SetSingleton(simParams);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                var simParams = m_AntSimulationSystem.GetSingleton<AntSimulationParams>();
                simParams.renderAnts ^= true;
                m_AntSimulationSystem.SetSingleton(simParams);
            }
        }
        
        if (Time.frameCount <= 1 || Time.frameCount % 30 == 0)
        {
            var s = m_AntSimulationSystem == null ? "No Ant Simulation System in any World!" : m_AntSimulationSystem.DumpStatusText();
            s += "\n\n";
            s += m_ClientAntRenderSystem == null ? "No Ant Render System in any World!" : m_ClientAntRenderSystem.DumpStatusText();
            statusText.text = s;
        }
    }

    static T FindSystemInAllWorlds<T>() where T : ComponentSystemBase
    {
        foreach (var world in World.All)
        {
            var componentSystemBase = world.GetExistingSystem<T>();
            if (componentSystemBase != null)
                return componentSystemBase;
        }

        return default;
    }
}
