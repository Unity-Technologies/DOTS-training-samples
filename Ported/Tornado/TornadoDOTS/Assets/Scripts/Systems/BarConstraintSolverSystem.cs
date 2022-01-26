using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(JointsSimulationSystem))]
public partial class BarConstraintSolverSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities 
            .ForEach((ref DynamicBuffer<Joint> joints, in DynamicBuffer<Connection> connections) =>
            {
                for (int i = 0; i < connections.Length; i++)
                {
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
                    else if (isJoint2Anchor)
                    {
                        joint1Pos += (push * 2);
                    }

                    joint1.Value = joint1Pos;
                    joint2.Value = joint2Pos;

                    joints[connection.J1] = joint1;
                    joints[connection.J2] = joint2;
                }
            }).ScheduleParallel();
    }
}