using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FarmerSpawnerSystem))]
public class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var target = GetSingletonEntity<CameraTarget>();
        var targetTranslation = GetComponent<Translation>(target);
        var targetVelocity = GetComponent<Velocity>(target);
        
        var settings = GetSingleton<Settings>();
        
        var camera = this.GetSingleton<GameObjectRefs>().Camera;
        
        var cameraPosition = camera.transform.position;
        
        camera.transform.position = Unity.Mathematics.math.lerp(cameraPosition, targetTranslation.Value + settings.CameraOffset, settings.CameraDamping);
        camera.transform.LookAt(Unity.Mathematics.math.lerp(cameraPosition, targetTranslation.Value, settings.CameraDamping));
    }
}
