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

        var world = World.DefaultGameObjectInjectionWorld;
        var antRenderSystem = world.GetExistingSystem<AntSimulationRenderSystem>();
        var antSimulationSystem = world.GetExistingSystem<AntSimulationSystem>();

        if (antSimulationSystem != null)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                var simParams = antSimulationSystem.GetSingleton<AntSimulationParams>();
                simParams.addWallsToTexture ^= true;
                antSimulationSystem.SetSingleton(simParams);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                var simParams = antSimulationSystem.GetSingleton<AntSimulationParams>();
                simParams.renderAnts ^= true;
                antSimulationSystem.SetSingleton(simParams);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                var simParams = antSimulationSystem.GetSingleton<AntSimulationParams>();
                simParams.renderObstacles ^= true;
                antSimulationSystem.SetSingleton(simParams);
            }
        }
        
        if (Time.frameCount <= 1 || Time.frameCount % 30 == 0)
        {
            if (antRenderSystem == null || antSimulationSystem == null)
                statusText.text = "No stats, cannot find Systems!";
            else
                statusText.text = antRenderSystem.DumpStatusText() + "\n\n" + antSimulationSystem.DumpStatusText();
        }
    }
}
