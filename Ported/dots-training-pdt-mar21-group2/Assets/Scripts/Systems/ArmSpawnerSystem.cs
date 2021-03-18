using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

public class ArmSpawnerSystem : SystemBase
{
    static Entity CreateJoint(ref EntityCommandBuffer ecb, in Entity jointBase, in Entity meshBase, float3 position,
        float length, float thickness)
    {
        // Joint without scaling
        var joint = ecb.Instantiate(jointBase);
        ecb.SetComponent(joint, new Translation {Value = position});
        ecb.AddComponent<ArmJoint>(joint);
        ecb.AddComponent<LocalToWorld>(joint); // Needed for children

        // Mesh with scaling and translation
        var mesh = ecb.Instantiate(meshBase);
        ecb.SetComponent(mesh, new Translation {Value = new float3(0.0f, length * 0.5f, 0.0f)});
        ecb.AddComponent(mesh, new NonUniformScale {Value = new float3(thickness, length, thickness)});
        ecb.AddComponent<LocalToWorld>(mesh); // Needed for parent
        ecb.AddComponent<LocalToParent>(mesh); // Needed for parent
        ecb.AddComponent(mesh, new Parent {Value = joint});

        return joint;
    }

    protected override void OnUpdate()
    {
        var worldBounds = GetSingleton<WorldBounds>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in ArmSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                // add margin to make sure arm won't try to grab for rock that will be recycled soon
                var margin = spawner.m_ArmJointLength * 4.0f;
                var armCount = (int) ((worldBounds.Width - margin) / spawner.m_ArmSeparation);

                for (uint i = 0; i < armCount; ++i)
                {
                    // Current position of arm, start at root
                    var rootTranslation = new float3(i * spawner.m_ArmSeparation, 0, 0);

                    // Entity of the arm
                    var instance = ecb.Instantiate(spawner.m_ArmPrefab);
                    ecb.SetComponent(instance, new Translation {Value = rootTranslation});

                    // Joint entities of the arm
                    var armScale = new NonUniformScale
                    {
                        Value = new float3(spawner.m_ArmJointThickness, spawner.m_ArmJointLength,
                            spawner.m_ArmJointThickness)
                    };

                    // Humerus
                    var relativeTranslation = float3.zero;
                    var humerus = CreateJoint(ref ecb, spawner.m_JointPrefab, spawner.m_JointBoxPrefab, relativeTranslation,
                        spawner.m_ArmJointLength, spawner.m_ArmJointThickness);
                    ecb.AddComponent<LocalToParent>(humerus); // Needed for parent
                    ecb.AddComponent(humerus, new Parent {Value = instance});

                    // Forearm
                    relativeTranslation = new float3(0.0f, spawner.m_ArmJointLength + spawner.m_ArmJointSpacing, 0.0f);
                    var forearm = CreateJoint(ref ecb, spawner.m_JointPrefab, spawner.m_JointBoxPrefab, relativeTranslation,
                        spawner.m_ArmJointLength, spawner.m_ArmJointThickness);
                    ecb.AddComponent<LocalToParent>(forearm); // Needed for parent
                    ecb.AddComponent(forearm, new Parent {Value = humerus});

                    // Fingers
                    var fingerJointLengths = new NativeArray<float>(4, Unity.Collections.Allocator.Temp);
                    fingerJointLengths[0] = spawner.m_Finger0JointLength;
                    fingerJointLengths[1] = spawner.m_Finger1JointLength;
                    fingerJointLengths[2] = spawner.m_Finger2JointLength;
                    fingerJointLengths[3] = spawner.m_Finger3JointLength;

                    var fingerJoints = new NativeArray<Entity>(15, Unity.Collections.Allocator.Temp);

                    float offset = -1.5f * spawner.m_FingerSpacing;
                    for (int j = 0; j < fingerJointLengths.Length; ++j)
                    {
                        relativeTranslation = new float3(offset, spawner.m_ArmJointLength + spawner.m_ArmJointSpacing, 0.0f);

                        // Joints per finger
                        for (int k = 0; k < 3; ++k)
                        {
                            var joint = CreateJoint(ref ecb, spawner.m_JointPrefab, spawner.m_JointBoxPrefab, relativeTranslation,
                                fingerJointLengths[j], spawner.m_FingerJointThickness);
                            ecb.AddComponent<LocalToParent>(joint); // Needed for parent
                            ecb.AddComponent(joint, new Parent {Value = k == 0 ? forearm : fingerJoints[j * 3 + k - 1]});
                            relativeTranslation = new float3(0.0f,  fingerJointLengths[j], 0.0f);
                            
                            fingerJoints[j * 3 + k] = joint;
                        }

                        offset += spawner.m_FingerJointSpacing + spawner.m_FingerSpacing;
                    }

                    // The thumb
                    relativeTranslation = new float3(0.5f * spawner.m_ArmJointThickness, spawner.m_ArmJointLength + spawner.m_ArmJointSpacing, 0.0f);
                    for (int j = 0; j < 3; ++j)
                    {
                        var joint = CreateJoint(ref ecb, spawner.m_JointPrefab, spawner.m_JointBoxPrefab, relativeTranslation,
                            spawner.m_ThumbJointLength, spawner.m_ThumbJointThickness);
                        if (j == 0)
                        {
                            ecb.AddComponent(joint, new Rotation {Value = quaternion.EulerZXY(Mathf.PI * -0.5f, 0.0f, Mathf.PI * -0.5f)});
                        }
                        ecb.AddComponent<LocalToParent>(joint); // Needed for parent
                        ecb.AddComponent(joint, new Parent {Value = j == 0 ? forearm : fingerJoints[12 + j - 1]});
                        relativeTranslation = new float3(0.0f, spawner.m_ThumbJointLength, 0.0f);

                        fingerJoints[12 + j] = joint;
                    }

                    // Add the joints with the arm component
                    var arm = new Arm
                    {
                        m_Humerus = humerus,
                        m_Forearm = forearm,
                        m_Finger0Joint0 = fingerJoints[0],
                        m_Finger0Joint1 = fingerJoints[1],
                        m_Finger0Joint2 = fingerJoints[2],
                        m_Finger1Joint0 = fingerJoints[3],
                        m_Finger1Joint1 = fingerJoints[4],
                        m_Finger1Joint2 = fingerJoints[5],
                        m_Finger2Joint0 = fingerJoints[6],
                        m_Finger2Joint1 = fingerJoints[7],
                        m_Finger2Joint2 = fingerJoints[8],
                        m_Finger3Joint0 = fingerJoints[9],
                        m_Finger3Joint1 = fingerJoints[10],
                        m_Finger3Joint2 = fingerJoints[11],
                        m_ThumbJoint0 = fingerJoints[12],
                        m_ThumbJoint1 = fingerJoints[13],
                        m_ThumbJoint2 = fingerJoints[14],
                    };
                    ecb.AddComponent(instance, arm);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}