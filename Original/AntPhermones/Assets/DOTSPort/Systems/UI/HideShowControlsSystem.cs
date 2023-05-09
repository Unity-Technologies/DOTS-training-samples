using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct HideShowControlsSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UIDirectoryManaged>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var directory = SystemAPI.ManagedAPI.GetSingleton<UIDirectoryManaged>();
        if (Input.GetKeyDown(KeyCode.H) && directory != null)
        {
            directory.Canvas.enabled = !directory.Canvas.enabled;
        }
    }
}
