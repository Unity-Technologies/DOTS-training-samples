using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CameraInteractionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var camera = this.GetSingleton<CameraRef>().Camera;
        var tornado = GetSingletonEntity<TornadoMovement>();
        camera.transform.LookAt(GetComponent<Translation>(tornado).Value);
    }
}
