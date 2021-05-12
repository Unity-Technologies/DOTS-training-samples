using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[UpdateInGroup(typeof(Unity.Entities.SimulationSystemGroup))]
[UpdateAfter(typeof(SpawnerSystem))]
public class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (TryGetSingleton<IsPaused>(out _))
            return;

        Camera mainCamera = Camera.main;

        //Update the camera tracking position
        float speed = 10.0f;
        var player = GetSingletonEntity<Player>();
        float time = (float)UnityEngine.Time.deltaTime;
        float step = speed * time; // calculate distance to move
        float originalHeight = mainCamera.transform.position.y;
        float3 targetPosition = GetComponent<Translation>(player).Value;
        float3 cameraPosition = Vector3.MoveTowards(mainCamera.transform.position, targetPosition, step);
        cameraPosition.y = originalHeight;
        mainCamera.transform.position = cameraPosition;
    }
}
