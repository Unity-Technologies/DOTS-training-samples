using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
public partial class CameraFollowSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CameraConfig>();
    }

    protected override void OnUpdate()
    {
        var entity = GetSingletonEntity<CameraFollow>();
        var entityTrans = EntityManager.GetComponentData<Translation>(entity);
        var entityPos = GetSmoothWorldPos(entityTrans.Value);
        
        var configEntity = GetSingletonEntity<CameraConfig>();
        var config = EntityManager.GetComponentData<CameraConfig>(configEntity);
        var cam = Camera.main.transform;

		config.ViewAngles.x += Input.GetAxis("Mouse X") * config.MouseSensitivity / Screen.height;
        config.ViewAngles.y -= Input.GetAxis("Mouse Y") * config.MouseSensitivity / Screen.height;
        config.ViewAngles.y = Mathf.Clamp(config.ViewAngles.y, 7f, 80f);
        config.ViewAngles.x -= Mathf.Floor(config.ViewAngles.x / 360f) * 360f;

        cam.rotation = Quaternion.Euler(config.ViewAngles.y, config.ViewAngles.x, 0f);
		cam.position = entityPos - cam.forward * config.ViewDist;

        EntityManager.SetComponentData(configEntity, config);
    }
    
	private Vector3 GetSmoothWorldPos(float3 value)
    {
		return new Vector3(value.x, 0f, value.z);
	}
}