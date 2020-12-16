using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FarmInitializeSystem))]
public class CameraSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var settings = GetSingleton<CommonSettings>();
        
        var target = GetSingletonEntity<CameraTarget>();
        var targetTranslation = GetComponent<Translation>(target);
        
        var camera = this.GetSingleton<GameObjectRefs>().Camera;
        
        var cameraPosition = camera.transform.position;
        camera.transform.position = math.lerp(cameraPosition, targetTranslation.Value + settings.CameraOffset, settings.CameraDamping);
        camera.transform.LookAt(math.lerp(cameraPosition, targetTranslation.Value, settings.CameraDamping));
    }
}
