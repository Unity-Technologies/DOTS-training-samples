using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

public partial class BarAssignColorSystem : SystemBase
{
    EntityQuery m_BarAssignColorEntitiesQuery;
    EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        ecb.RemoveComponentForEntityQuery<BarAssignColor>(m_BarAssignColorEntitiesQuery);
        var gcfe = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>();
        
        var random = new Random(1234);
        Entities
            .WithNativeDisableContainerSafetyRestriction(gcfe)
            .WithStoreEntityQueryInField(ref m_BarAssignColorEntitiesQuery)
            .WithAll<BarAssignColor>()
            .ForEach((
                in DynamicBuffer<Connection> connections,
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
                    color.Value = new float4(1) * random.NextFloat(0.7f, 1.0f) * upDot;
                    gcfe[bar] = color;
                }
            }).ScheduleParallel();
    }
}