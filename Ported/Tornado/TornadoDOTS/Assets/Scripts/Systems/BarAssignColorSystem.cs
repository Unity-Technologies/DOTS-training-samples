using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public partial class BarAssignColorSystem : SystemBase
{
    private EntityQuery barAssignColorEntitiesQuery;
    private EntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = commandBufferSystem.CreateCommandBuffer();
        
        ecb.RemoveComponentForEntityQuery<BarAssignColor>(barAssignColorEntitiesQuery);

        var gcfe = GetComponentDataFromEntity<Translation>(true);
        var rnd = new Random(1234);
        Entities
            .WithReadOnly(gcfe)
            .WithStoreEntityQueryInField(ref barAssignColorEntitiesQuery)
            .WithAll<BarAssignColor>()
            .ForEach((ref URPMaterialPropertyBaseColor color, in BarConnection connection) =>
            {
                var join1Pos = gcfe[connection.Joint1].Value;
                var join2Pos = gcfe[connection.Joint2].Value;
                var forward = math.normalize(join2Pos - join1Pos);

                var upDot = math.acos(math.abs(math.dot(forward, new float3(0, 1, 0))))/math.PI;
                color.Value = new float4(1) * rnd.NextFloat(0.7f, 1.0f) * upDot;
            }).ScheduleParallel();
    }
}