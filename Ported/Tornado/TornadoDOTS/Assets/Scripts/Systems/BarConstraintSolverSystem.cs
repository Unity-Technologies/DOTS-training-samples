using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(JointsSimulationSystem))]
public partial class BarConstraintSolverSystem : SystemBase
{
	private EntityCommandBufferSystem ecbSystem;
	protected override void OnCreate()
	{
		base.OnCreate();
		ecbSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
    {
        var breakDistance = GetSingleton<TornadoSimulationParameters>().BreakDistance;

        var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
	        .ForEach((Entity e,
		        int entityInQueryIndex,
		        ref DynamicBuffer<Joint> joints,
		        ref DynamicBuffer<Connection> connections) =>
	        {
		        var nextJointIndex = joints.Length;
                for (var i = 0; i < connections.Length; i++)
                {
                    //Distance constraint
                    var connection = connections[i];
                    var joint1 = joints[connection.J1];
                    var joint2 = joints[connection.J2];
                    
                    var joint1Pos = joint1.Value;
                    var joint2Pos = joint2.Value;
                    
                    var delta = joint2Pos - joint1Pos;
                    var dist = math.length(delta);
                    var extraDist = dist - connection.OriginalLength;

                    var push = (delta / dist * extraDist) * 0.5f;
                    var isJoint1Anchor = joint1.IsAnchored;
                    var isJoint2Anchor = joint2.IsAnchored;
                    if (!isJoint1Anchor && !isJoint2Anchor)
                    {
                        joint1Pos += push;
                        joint2Pos -= push;
                    }
                    else if (isJoint1Anchor)
                    {
                        joint2Pos -= (push * 2);
                    }
                    else
                    {
                        joint1Pos += (push * 2);
                    }

                    joint1.Value = joint1Pos;
                    joint2.Value = joint2Pos;

					//Update existing joints
                    joints[connection.J1] = joint1;
                    joints[connection.J2] = joint2;
                    
					if (math.abs(extraDist) > breakDistance)
					{
						if (joint2.NeighbourCount > 1)
						{
							joint2.NeighbourCount--;
							joints[connection.J2] = joint2;
							
							var newJoint = joint2;
							newJoint.NeighbourCount = 1;
							ecb.AppendToBuffer(entityInQueryIndex, e, newJoint);
							connection.J2 = nextJointIndex;
							nextJointIndex++;
						}
						else if (joint1.NeighbourCount > 1)
						{
							joint1.NeighbourCount--;
							joints[connection.J1] = joint1;
							
							var newJoint = joint1;
							newJoint.NeighbourCount = 1;
							ecb.AppendToBuffer(entityInQueryIndex, e, newJoint);
							connection.J1 = nextJointIndex;
							nextJointIndex++;
						}
					}
					
                    connections[i] = connection;
                }
            })
	        .ScheduleParallel();
        
        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}