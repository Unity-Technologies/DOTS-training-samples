using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class CameraInteractionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var cameraRef = this.GetSingleton<CameraRef>();
        var camera = cameraRef.Camera;
        var tornado = GetSingletonEntity<TornadoMovement>();
        var tornadoPos = GetComponent<Translation>(tornado).Value;
        camera.transform.position = (Vector3) tornadoPos - camera.transform.forward * cameraRef.ArmLength;
    }
}
