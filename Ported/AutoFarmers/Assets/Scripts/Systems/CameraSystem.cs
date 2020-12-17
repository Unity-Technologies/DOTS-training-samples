using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FarmInitializeSystem))]
public class CameraSystem : SystemBase
{
    private static bool k_SnapCameraOnFirstUpdate = true;
    
    protected override void OnUpdate()
    {
        var settings = GetSingleton<CommonSettings>();
        
        var target = GetSingletonEntity<CameraTarget>();
        var targetTranslation = GetComponent<Translation>(target);
        
        var camera = this.GetSingleton<GameObjectRefs>().Camera;
        
        var cameraPosition = new float3(camera.transform.position);
        
        var damping = settings.CameraDamping;
        if (k_SnapCameraOnFirstUpdate)
        {
            k_SnapCameraOnFirstUpdate = false;
            damping = 1f;
        }

        camera.transform.position = math.lerp(cameraPosition, targetTranslation.Value + settings.CameraOffset, damping);
        camera.transform.LookAt(math.lerp(cameraPosition, targetTranslation.Value, damping));
    }
}
