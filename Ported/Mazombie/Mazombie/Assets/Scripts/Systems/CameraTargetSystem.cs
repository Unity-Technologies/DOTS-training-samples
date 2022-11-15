using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
public partial class CameraTargetSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<Player>();
    }

    protected override void OnUpdate()
    {
        if (MainCameraTargetGameObject.TransformInstance == null)
            return;
        
        // Set the Camera Target gameobject's position to the player's position
        // This is needed since the Cinemachine's Virtual Cam can only follow a managed object
        Entity playerEntity = SystemAPI.GetSingletonEntity<Player>();
        LocalToWorld targetLocalToWorld = SystemAPI.GetComponent<LocalToWorld>(playerEntity);
        MainCameraTargetGameObject.TransformInstance.SetPositionAndRotation(targetLocalToWorld.Position, targetLocalToWorld.Rotation);
    }
}