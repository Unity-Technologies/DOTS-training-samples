using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[DisableAutoCreation]
public class CarStatusRenderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var blockedStatusMaterial = CarStatusDisplayManager.Instance.BlockedStatusMaterial;
        var defaultSpeedStatusMaterial = CarStatusDisplayManager.Instance.DefaultSpeedStatusMaterial;
        var accelerationStatusMaterial = CarStatusDisplayManager.Instance.AccelerationStatusMaterial;

        Entities.WithoutBurst()
            .ForEach((RenderMesh renderMesh, in CarProperties translation, in Speed speed) =>
            {
                renderMesh.material = accelerationStatusMaterial;
            }).Run();
    }
}
