using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

// This system should run after the transform system has been updated, otherwise the camera
// will lag one frame behind the tank and will jitter.
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class CameraSystem : SystemBase
{
    [UnityEngine.Header("Camera Configs")]
    private Entity Target;
    private EntityQuery PlayerQuery;
    public float cameraYSpeed;

    public float cameraDampening; 
    public float cameraFollowHeight;
    public float maxTerrainHeight;
    public float followHeightOffset;
    public float followHeightOffsetMin; 
    public float ySpeed; 
    private float3 camVel; 
    public bool following; 


    protected override void OnCreate()
    {
        PlayerQuery = GetEntityQuery(typeof(Player));
        RequireForUpdate(PlayerQuery);
    }

    protected override void OnUpdate()
    {
        var player = PlayerQuery.ToEntityArray(Allocator.Temp);
        Target = player[0];
        var cameraTransform = CameraSingleton.Instance.transform;
        var playerTransform = GetComponent<LocalToWorld>(Target);

        cameraTransform.position = playerTransform.Position - 10.0f * playerTransform.Forward + new float3(0.0f, 5.0f, 0.0f);
        cameraTransform.LookAt(playerTransform.Position, new float3(0.0f, 1.0f, 0.0f));

        // controlling height offset
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.DownArrow))
            followHeightOffset += ySpeed * UnityEngine.Time.unscaledDeltaTime;
        else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.UpArrow))
            followHeightOffset -= ySpeed * UnityEngine.Time.unscaledDeltaTime;


        cameraTransform.position = Follow(playerTransform, cameraTransform);
    }

    [BurstCompile]
    private float3 Follow(LocalToWorld playerTransform, UnityEngine.Transform cameraTransform){
        followHeightOffset = math.max(followHeightOffset, followHeightOffsetMin);
        float height = maxTerrainHeight + followHeightOffset;
        float3 target = playerTransform.Position;
        target.y = height;

        var config =  SystemAPI.GetSingleton<Config>();
        target = ApplyCameraRotationOffset(target, cameraTransform, config.maxTerrainHeight);

        //float3 pos = smoothDamp(cameraTransform, target, camera.camVel, camera.cameraDampening, float.MaxValue, UnityEngine.Time.unscaledDeltaTime);
        float3 pos = math.lerp(cameraTransform.position, target, cameraDampening * UnityEngine.Time.unscaledDeltaTime);
        return pos; 

    }


    [BurstCompile]
    private float3 ApplyCameraRotationOffset(float3 pos, UnityEngine.Transform cameraTransform, float maxTerrainHeight){
        float zOff = (pos.y - maxTerrainHeight) / math.tan(cameraTransform.rotation.eulerAngles.x * (180/ math.PI));
        pos.z += zOff;
        return pos;
    }

    /*
    public void Follow(LocalToWorld playerTransform, CameraComponent camera, UnityEngine.Transform cameraTransform) {
            var config =  SystemAPI.GetSingleton<Config>();
			camera.following = true;
			float3 target = playerTransform.Position;
			var maxTerrainHeight = config.maxTerrainHeight;
			target.y = maxTerrainHeight + camera.followHeightOffset;
			cameraTransform.position = ApplyCameraRotationOffset(target, cameraTransform, camera.maxTerrainHeight);
			camera.camVel = new float3(0, 0, 0);
		}
    */


}