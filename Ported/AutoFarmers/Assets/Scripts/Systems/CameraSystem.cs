using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class CameraSystem : SystemBase
{
    private bool m_IsFirstUpdate = true;
    private float2 m_ViewAngles = float2.zero;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CameraTarget>();
    }
    protected override void OnUpdate()
    {
        var camera = this.GetSingleton<GameObjectRefs>().Camera;
        var settings = GetSingleton<CommonSettings>();
        var target = GetSingletonEntity<CameraTarget>();

        if (m_IsFirstUpdate)
        {
            m_IsFirstUpdate = false;
            m_ViewAngles = settings.CameraViewAngle;
            camera.transform.rotation = Quaternion.Euler(m_ViewAngles.y, m_ViewAngles.x, 0f);
        }

        // Port of the original camera behavior.
        var targetPosition = GetComponent<Translation>(target).Value;
        m_ViewAngles.x += Input.GetAxis("Mouse X") * settings.CameraMouseSensitivity / Screen.height;
        m_ViewAngles.y -= Input.GetAxis("Mouse Y") * settings.CameraMouseSensitivity / Screen.height;
        m_ViewAngles.y = Mathf.Clamp(m_ViewAngles.y,7f,80f);
        m_ViewAngles.x -= Mathf.Floor(m_ViewAngles.x / 360f) * 360f;
        
        camera.transform.rotation = Quaternion.Euler(m_ViewAngles.y,m_ViewAngles.x,0f);
        camera.transform.position = targetPosition - new float3(camera.transform.forward) * settings.CameraViewDistance;
    }
}
