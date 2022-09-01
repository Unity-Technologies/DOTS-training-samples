using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
