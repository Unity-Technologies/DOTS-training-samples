using Unity.Entities;
using Unity.Mathematics;

public class CameraSystem : SystemBase {

    float3 offset;

    protected override void OnCreate() {
        RequireSingletonForUpdate<PlayerTag>();
        offset = math.float3(0.0f, 0.0f, 0.0f);
    }

    protected override void OnUpdate() {
        var e = GetSingletonEntity<PlayerTag>();
        var pos = GetComponent<Position>(e).Value;
        pos.y = 0;
        if(UnityEngine.Camera.main && math.length(offset) == 0.0f)
        {
            offset = UnityEngine.Camera.main.transform.position;
        }
        UnityEngine.Camera.main.transform.position = pos + offset;
    }
}