using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraSystem : SystemBase
{
    private bool _Initialized = false;
    
    protected override void OnCreate()
    {
        EntityManager.CreateEntity(typeof(CameraData));
    }
    
    protected override void OnUpdate()
    {
        // Need to do this here instead of OnCreate to ensure GameObjectRefs has time to be created
        if (!TryInitCameraDistance())
            return;

        if (!TryGetSingletonEntity<Player>(out var playerEntity))
            return;
        
        // Apply input
        var cameraEntity = GetSingletonEntity<CameraData>();
        var cameraData = GetComponent<CameraData>(cameraEntity);
        cameraData.Distance += Input.GetAxis("Vertical") * Time.DeltaTime * 5f;
        SetComponent(cameraEntity, cameraData);

        // Compute new camera position
        var targetPos = GetComponent<Translation>(playerEntity).Value;
        targetPos.y = 0;
        var refs = this.GetSingleton<GameObjectRefs>();
        refs.Camera.transform.position = targetPos + (refs.CameraOffset * cameraData.Distance);
    }

    private bool TryInitCameraDistance()
    {
        if (_Initialized)
            return true;

        if (!HasSingleton<GameObjectRefs>())
            return false;
        
        var cameraEntity = GetSingletonEntity<CameraData>();
        var camera = GetComponent<CameraData>(cameraEntity);
        camera.Distance = this.GetSingleton<GameObjectRefs>().CameraInitialDistance;
        SetComponent(cameraEntity, camera);

        _Initialized = true;
        return true;
    }
}
