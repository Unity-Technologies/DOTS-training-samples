using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Assertions.Must;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(BarConstraintSolverSystem))]
public partial class BarBreakSystem : SystemBase
{
    const float k_BreakStrength = 0.55f; // break when above 5% change
    EntityCommandBufferSystem m_CommandBufferSystem;
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();
        Entities 
            .ForEach((Entity entity, 
                int entityInQueryIndex,
                ref DynamicBuffer<Connection> connections,
                ref DynamicBuffer<Joint> joints,
                ref DynamicBuffer<JointNeighbours> neighbours) =>
            {
                int nextIndex = joints.Length;
                for (int i = 0; i < connections.Length; i++)
                {
                    var connection = connections[i];
                    if (connection.OriginalLength == 0)
                    {
                        continue; // already broken
                    }
                    var joint1 = joints[connection.J1];
                    var joint1Pos = joint1.Value;
                    var joint2 = joints[connection.J2];
                    var joint2Pos = joint2.Value;

                    var delta = joint1Pos - joint2Pos;
                    var length = math.length(delta);
                    var extraDist = math.abs(length - connection.OriginalLength);
                    if (extraDist > k_BreakStrength)
                    {
                        var neighbourCountJ1 = neighbours[connection.J1];
                        var neighbourCountJ2 = neighbours[connection.J2];
                        if (neighbourCountJ2.Value > 1)
                        {
                            neighbourCountJ2.Value--;
                            neighbours[connection.J2] = neighbourCountJ2;
                            connection.J2 = nextIndex;
                            nextIndex++;
                            parallelWriter.AppendToBuffer(entityInQueryIndex, entity, joint2);
                            parallelWriter.AppendToBuffer(entityInQueryIndex, entity, new JointNeighbours
                            {
                                Value = 1,
                            });
                        }
                        else if (neighbourCountJ1.Value > 1)
                        {
                            neighbourCountJ1.Value--;
                            neighbours[connection.J1] = neighbourCountJ1;
                            connection.J1 = nextIndex;
                            nextIndex++;
                            parallelWriter.AppendToBuffer(entityInQueryIndex, entity, joint1);
                            parallelWriter.AppendToBuffer(entityInQueryIndex, entity, new JointNeighbours
                            {
                                Value = 1,
                            });
                        }
                        connections[i] = connection;
                    }
                }
            }).ScheduleParallel();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
