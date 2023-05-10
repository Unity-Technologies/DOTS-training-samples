using Unity.Entities;
using UnityEngine;

public partial struct TimeScaleSystem : ISystem, ISystemStartStop
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UIDirectoryManaged>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        float timeScale = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            timeScale = 1f;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            timeScale = 2f;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            timeScale = 3f;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            timeScale = 4f;
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            timeScale = 5f;
        } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            timeScale = 6f;
        } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            timeScale = 7f;
        } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            timeScale = 8f;
        } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            timeScale = 9f;
        }

        if (timeScale >= 0) // TimeScale has been changed
        {
            Time.timeScale = timeScale;
            RefreshUI();
        }
    }

    // Only needs for refreshing UI at start
    public void OnStartRunning(ref SystemState state)
    {
        RefreshUI();
    }

    public void OnStopRunning(ref SystemState state)
    {
        // Empty, we don't need this method but ISystemStartStop needs
    }

    void RefreshUI()
    {
        var directory = SystemAPI.ManagedAPI.GetSingleton<UIDirectoryManaged>();
        
        if (directory == null)
            return;
        
        directory.TextCurrentTimeScale.text = $"TimeScale: {Time.timeScale}";
    }
}
