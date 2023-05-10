using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct CameraControlSystem : ISystem, ISystemStartStop
{
    float minZoom;
    float maxZoom;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        
        Camera camera = Camera.main;

        float scrollInput = Input.mouseScrollDelta.y;

        if (scrollInput != 0)
        {
            float curZoom = camera.orthographicSize;
            curZoom -= scrollInput * 10;
            curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
            camera.orthographicSize = curZoom;
        }
    }

    public void OnStartRunning(ref SystemState state)
    {
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        
        Camera camera = Camera.main;
        camera.orthographic = true;
        camera.orthographicSize = globalSettings.MapSizeX / 2;
        camera.transform.position = new Vector3(globalSettings.MapSizeX / 2f, globalSettings.MapSizeY / 2f, -10);
        
        minZoom = globalSettings.MapSizeX / 30f;
        maxZoom = globalSettings.MapSizeX;
    }

    public void OnStopRunning(ref SystemState state)
    {
        
    }
}
