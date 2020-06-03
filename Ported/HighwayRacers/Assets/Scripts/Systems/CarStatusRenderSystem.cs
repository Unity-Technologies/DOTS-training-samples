using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class CarStatusRenderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        CarSpawner spawner = GetSingleton<CarSpawner>();
        var carPrefab = spawner.CarPrefab;
        var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(carPrefab);
        renderMesh.material = CarStatusDisplayManager.Instance.BlockedStatusMaterial;
        
        var query = GetEntityQuery(ComponentType.ReadOnly<BlockedState>());

        EntityManager.SetSharedComponentData<RenderMesh>(query, renderMesh);

        // var blockedStatusMaterial = CarStatusDisplayManager.Instance.BlockedStatusMaterial;
        // var defaultSpeedStatusMaterial = CarStatusDisplayManager.Instance.DefaultSpeedStatusMaterial;
        // var accelerationStatusMaterial = CarStatusDisplayManager.Instance.AccelerationStatusMaterial;

        Entities.WithoutBurst()
            .ForEach((in CarProperties translation, in Speed speed) =>
            {
            }).Run();
    }
}
