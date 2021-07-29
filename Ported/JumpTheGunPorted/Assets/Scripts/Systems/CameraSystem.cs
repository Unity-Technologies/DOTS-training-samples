using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraSystem : SystemBase
{
    private bool _Initialized = false;
    
    protected override void OnCreate()
    {
        EntityManager.CreateEntity(typeof(CameraData));
        
        RequireSingletonForUpdate<CameraData>();
        RequireSingletonForUpdate<GameObjectRefs>();
        RequireSingletonForUpdate<Player>();
    }
    
    protected override void OnUpdate()
    {
        if (!_Initialized)
        {
            InitCameraDistance();
            _Initialized = true;
        }

        var playerEntity = GetSingletonEntity<Player>();
        
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

    private void InitCameraDistance()
    {
        var cameraEntity = GetSingletonEntity<CameraData>();
        var camera = GetComponent<CameraData>(cameraEntity);
        camera.Distance = this.GetSingleton<GameObjectRefs>().CameraInitialDistance;
        SetComponent(cameraEntity, camera);
    }
}
