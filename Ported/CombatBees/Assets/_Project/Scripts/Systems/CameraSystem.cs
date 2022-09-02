using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysUpdateSystem]
[UpdateAfter(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(EntityCameraSystem))]
public partial class GameObjectCameraSystem : SystemBase
{
    public Transform GameObjectCameraTransform;
    
    protected override void OnUpdate()
    {
        if (GameObjectCameraTransform != null && HasSingleton<EntityCamera>())
        {
            Entity cameraEntity = GetSingletonEntity<EntityCamera>();
            LocalToWorld cameraLtW = GetComponent<LocalToWorld>(cameraEntity);
            GameObjectCameraTransform.position = cameraLtW.Position;
            GameObjectCameraTransform.rotation = cameraLtW.Rotation;
        }
    }
}

[AlwaysUpdateSystem]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class EntityCameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Vector2 camRotateInput = default;
		if (Input.GetKey(KeyCode.Mouse1)) 
        {
			camRotateInput.x += Input.GetAxis("Mouse X");
			camRotateInput.y -= Input.GetAxis("Mouse Y");
		}
        float camZoomInput = 0f;
		camZoomInput -= Input.GetAxis("Mouse ScrollWheel");

        Entities
            .ForEach((Entity entity, ref EntityCamera entityCamera, ref Rotation rotation, ref Translation translation) => 
            {
                float3 center = float3.zero + (math.up() * entityCamera.CenterHeight);

                if(!entityCamera.IsInitialized)
                {
                    entityCamera.CurrentZoomDistance = 50f;
                    entityCamera.PitchAngle = 45f;
                    entityCamera.YawAngle = 45f;
                    entityCamera.IsInitialized = true;
                }

                entityCamera.PitchAngle += camRotateInput.y * entityCamera.RotationSpeed;
                entityCamera.YawAngle += camRotateInput.x * entityCamera.RotationSpeed;
                entityCamera.CurrentZoomDistance += camZoomInput * entityCamera.ZoomSpeed;
                
                entityCamera.PitchAngle = math.clamp(entityCamera.PitchAngle, -89f, 89f);
                entityCamera.CurrentZoomDistance = math.clamp(entityCamera.CurrentZoomDistance, entityCamera.MinZoomDistance, entityCamera.MaxZoomDistance);

                rotation.Value = quaternion.Euler(math.radians(entityCamera.PitchAngle), math.radians(entityCamera.YawAngle), 0f);
                translation.Value = center + (math.mul(rotation.Value, -math.forward()) * entityCamera.CurrentZoomDistance);

            }).Schedule();
    }
}
