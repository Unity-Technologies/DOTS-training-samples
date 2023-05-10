using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct CameraInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        
        Camera camera = Camera.main;
        camera.orthographic = true;
        camera.orthographicSize = globalSettings.MapSizeX;
        camera.transform.position = new Vector3(globalSettings.MapSizeX / 2f, globalSettings.MapSizeY / 2f, -10);
    }
}
