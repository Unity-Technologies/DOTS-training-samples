using System;
using TMPro;
using Unity.Entities;
using UnityEngine;

public partial struct UIDirectoryInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TimeScaleControlsExecution>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var directory = GameObject.FindObjectOfType<UIDirectory>();
        if (directory == null)
        {
            throw new Exception($"GameObject with type {nameof(UIDirectory)} not found.");
        }

        var directoryManaged = new UIDirectoryManaged();
        directoryManaged.TextCurrentTimeScale = directory.TextCurrentTimeScale;
        directoryManaged.Canvas = directory.Canvas;

        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(entity, directoryManaged);
    }
}

public class UIDirectoryManaged: IComponentData
{
    public Canvas Canvas;
    public TextMeshProUGUI TextCurrentTimeScale;
    
    // Every IComponentData class must have a no-arg constructor.
    public UIDirectoryManaged()
    {
    }
}