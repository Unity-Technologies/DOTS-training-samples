using System.Runtime.InteropServices.ComTypes;
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

        var gcfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();
        var rnd = new Random(1234);
        Entities
            .WithNativeDisableContainerSafetyRestriction(gcfe)
            .WithStoreEntityQueryInField(ref barAssignColorEntitiesQuery)
            .WithAll<BarAssignColor>()
            .ForEach((in DynamicBuffer<Connection> connections,
                in DynamicBuffer<Joint> joints,
                in DynamicBuffer<Bar> bars) =>
            {
                for (var i = 0; i < connections.Length; i++)
                {
                    var connection = connections[i];
                    var bar = bars[i];
                    var join1Pos = joints[connection.J1].Value;
                    var join2Pos = joints[connection.J2].Value;
                    var forward = math.normalize(join2Pos - join1Pos);

                    var upDot = math.acos(math.abs(math.dot(forward, new float3(0, 1, 0))))/math.PI;
                    var color = gcfe[bar];
                    color.Value = new float4(1) * rnd.NextFloat(0.7f, 1.0f) * upDot;
                    gcfe[bar] = color;
                }
            }).ScheduleParallel();
    }
}