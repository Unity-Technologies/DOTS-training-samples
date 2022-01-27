using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class JointAndBarSimulationSystem : SystemBase
{
    const float k_BreakStrength = 0.55f; // break when above 5% change
	float m_TornadoForceFader;
    EntityCommandBufferSystem m_CommandBufferSystem;
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var tornado = GetSingletonEntity<Tornado>();
        var parameters = GetComponent<TornadoSimulationParameters>(tornado);
        var tornadoDimensions = GetComponent<TornadoDimensions>(tornado);
        var tornadoPos = GetComponent<Translation>(tornado).Value;
        var dt = Time.fixedDeltaTime;
        var time = (float) Time.ElapsedTime;
        m_TornadoForceFader = math.clamp(m_TornadoForceFader + dt / 10.0f, 0, 1);
        var tornadoFader = m_TornadoForceFader;
        var random = new Random(1234);

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();
        Entities
            .ForEach((
                Entity entity,
                int entityInQueryIndex,
                ref DynamicBuffer<Connection> connections,
                ref DynamicBuffer<Joint> joints,
                ref DynamicBuffer<JointNeighbours> neighbours) =>
            {
                // Step 1: Apply forces to every joint (gravity and tornado)
		        for (var i = 0; i < joints.Length; i++)
		        {
			        var joint = joints[i];

			        //TODO (perf): Maybe separate anchored joints in another buffer? (probably doesn't matter a whole lot)
			        if (joint.IsAnchored)
			        {
				        continue;
			        }

					var start = joint.Value;

					var jointPos = joint.Value;
					var jointOldPos = joint.OldPos;

					jointOldPos.y += parameters.Gravity * dt;

					//TODO: We could use float2, or two floats, but whatever
					var td = new float3(
						tornadoPos.x + TornadoSway(jointPos.y, time) - jointPos.x,
						0,
						tornadoPos.z - jointPos.z
					);

					var tornadoXZDist = math.sqrt(td.x * td.x + td.z * td.z);
					td /= tornadoXZDist;

					if (tornadoXZDist < tornadoDimensions.TornadoRadius)
					{
						var forceScalar = (1.0f - tornadoXZDist / tornadoDimensions.TornadoRadius);
						var yFader = math.clamp(1f - jointPos.y / tornadoDimensions.TornadoHeight, 0, 1);
						forceScalar *= tornadoFader * parameters.TornadoForce * parameters.ForceMultiplyRange.RandomInRange(random);
						var force = new float3(
							-td.z - td.x * parameters.TornadoInwardForce*yFader,
							-parameters.TornadoUpForce,
							td.x - td.z * parameters.TornadoInwardForce*yFader
						);

						jointOldPos -= (force * forceScalar);
					}

					jointPos += (jointPos - jointOldPos) * (1f - parameters.Damping);
					jointOldPos = start;

					if (jointPos.y < 0f)
					{
						jointPos.y = 0;
						jointOldPos.y = -jointPos.y;
						jointOldPos.x += (jointPos.x - jointOldPos.x) * parameters.Friction;
						jointOldPos.z += (jointPos.z - jointOldPos.z) * parameters.Friction;
					}

					joint.Value = jointPos;
					joint.OldPos = jointOldPos;
					joints[i] = joint;
		        }

                //Step 2: Constraint Resolution + Bar break
                int nextIndex = joints.Length;
                for (int i = 0; i < connections.Length; i++)
                {
                    //Step 2.1: Bar length constraint solver
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


                    // Step 2.2: Bar break check
                    extraDist = math.abs(extraDist);
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
    static float TornadoSway(float y, float time) {
		return math.sin(y / 5f + time/4f) * 3f;
	}
}
