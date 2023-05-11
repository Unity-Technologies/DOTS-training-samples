using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial struct CameraControlSystem : ISystem, ISystemStartStop
{
    Vector3 m_MousePressPosition;
    Vector3 m_CameraPressPosition;
    float m_MinZoom;
    float m_MaxZoom;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
        //state.RequireForUpdate<CameraMoveByMouseExecution>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Camera camera = Camera.main;

        float scrollInput = Input.mouseScrollDelta.y;

        if (scrollInput != 0)
        {
            float curZoom = camera.orthographicSize;
            curZoom -= scrollInput * 10;
            curZoom = Mathf.Clamp(curZoom, m_MinZoom, m_MaxZoom);
            camera.orthographicSize = curZoom;
        }

      //  if (Input.GetMouseButtonDown(0))
      //  {
      //      m_MousePressPosition = Input.mousePosition;
      //      m_CameraPressPosition = camera.transform.position;
      //  }
      //  
      //  if (Input.GetMouseButton(0))
      //  {
      //      var delta = m_MousePressPosition - Input.mousePosition;
      //      camera.transform.position = m_CameraPressPosition + (delta * (camera.orthographicSize / (128 * 4)));
      //  }
    }

    public void OnStartRunning(ref SystemState state)
    {
        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        
        Camera camera = Camera.main;
        camera.orthographic = true;
        camera.orthographicSize = globalSettings.MapSizeX / 2;
        camera.transform.position = new Vector3(globalSettings.MapSizeX / 2f, globalSettings.MapSizeY / 2f, -10);
        
        m_MinZoom = globalSettings.MapSizeX / 30f;
        m_MaxZoom = globalSettings.MapSizeX;
    }

    public void OnStopRunning(ref SystemState state)
    {
        
    }
}
