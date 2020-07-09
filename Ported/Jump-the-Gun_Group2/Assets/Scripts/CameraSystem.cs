using Unity.Entities;
using Unity.Mathematics;

public class CameraSystem : SystemBase {

    float3 offset;

    protected override void OnCreate() {
        RequireSingletonForUpdate<PlayerTag>();
        offset = UnityEngine.Camera.main.transform.position;
    }

    protected override void OnUpdate() {
        var e = GetSingletonEntity<PlayerTag>();
        var pos = GetComponent<Position>(e).Value;
        pos.y = 0;
        UnityEngine.Camera.main.transform.position = pos + offset;
    }
}