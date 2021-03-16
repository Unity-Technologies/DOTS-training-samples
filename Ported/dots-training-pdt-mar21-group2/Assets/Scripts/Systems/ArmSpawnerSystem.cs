using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ArmSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in ArmSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (uint i = 0; i < spawner.m_ArmCount; ++i)
                {
                    // Current position of arm, start at root
                    var translation = new float3(i * spawner.m_ArmSeparation, 0, 0);

                    // Entity of the arm
                    var instance = ecb.Instantiate(spawner.m_ArmPrefab);
                    ecb.SetComponent(instance, new Translation {Value = translation});

                    // Joint entities of the arm
                    var armScale = new NonUniformScale
                    {
                        Value = new float3(spawner.m_ArmJointThickness, spawner.m_ArmJointLength,
                            spawner.m_ArmJointThickness)
                    };
                    
                    // Humerus
                    var humerusTranslation = new Translation
                    {
                        Value = new float3(translation.x, translation.y + 0.5f * spawner.m_ArmJointLength,
                            translation.z)
                    };
                    var humerus = ecb.Instantiate(spawner.m_JointPrefab);
                    ecb.SetComponent(humerus, humerusTranslation);
                    ecb.AddComponent(humerus, armScale);
                    ecb.AddComponent<ArmJoint>(humerus);
                    translation.y += spawner.m_ArmJointLength + spawner.m_ArmJointSpacing;

                    // Forearm
                    var forearmTranslation = new Translation
                    {
                        Value = new float3(translation.x, translation.y + 0.5f * spawner.m_ArmJointLength,
                            translation.z)
                    };
                    var forearm = ecb.Instantiate(spawner.m_JointPrefab);
                    ecb.SetComponent(forearm, forearmTranslation);
                    ecb.AddComponent(forearm, armScale);
                    ecb.AddComponent<ArmJoint>(forearm);
                    translation.y += spawner.m_ArmJointLength + spawner.m_ArmJointSpacing;

                    // Fingers
                    var fingerJointLengths = new float[]
                    {
                        spawner.m_Finger0JointLength,
                        spawner.m_Finger1JointLength,
                        spawner.m_Finger2JointLength,
                        spawner.m_Finger3JointLength,
                    };
                    var fingerJoints = new Entity[15];

                    float offset = -1.5f * spawner.m_FingerSpacing;
                    var fingerScale = new float3(spawner.m_FingerJointThickness, 0.0f, spawner.m_FingerJointThickness);
                    
                    for (uint j = 0; j < fingerJointLengths.Length; ++j)
                    {
                        var fingerTranslation = new float3();
                        fingerTranslation.x = translation.x + offset;
                        fingerTranslation.y = translation.y;
                        fingerTranslation.z = translation.z;
                    
                        fingerScale.y = fingerJointLengths[j];
                    
                        // Joints per finger
                        for (uint k = 0; k < 3; ++k)
                        {
                            var joint = ecb.Instantiate(spawner.m_JointPrefab);
                            ecb.SetComponent(joint,
                                new Translation
                                {
                                    Value = new float3(fingerTranslation.x,
                                        fingerTranslation.y + 0.5f * fingerJointLengths[j], fingerTranslation.z)
                                });
                            ecb.AddComponent(joint, new NonUniformScale {Value = fingerScale});
                            ecb.AddComponent<ArmJoint>(joint);
                    
                            fingerJoints[j * 3 + k] = joint;
                            fingerTranslation.y += fingerJointLengths[j];
                        }
                    
                        offset += spawner.m_FingerJointSpacing + spawner.m_FingerSpacing;
                    }
                    
                    // The thumb
                    float3 thumbTranslation;
                    thumbTranslation.x = translation.x - spawner.m_FingerSpacing;
                    thumbTranslation.y = translation.y;
                    thumbTranslation.z = translation.z;
                    var thumbScale = new float3(spawner.m_ThumbJointLength, spawner.m_ThumbJointThickness, spawner.m_ThumbJointThickness);
                    for (uint j = 0; j < 3; ++j)
                    {
                        var joint = ecb.Instantiate(spawner.m_JointPrefab);
                        ecb.SetComponent(joint,
                            new Translation
                            {
                                Value = new float3(thumbTranslation.x - 0.5f * spawner.m_ThumbJointLength,
                                    thumbTranslation.y, thumbTranslation.z)
                            });
                        ecb.AddComponent(joint, new NonUniformScale {Value = thumbScale});
                        ecb.AddComponent<ArmJoint>(joint);
                    
                        fingerJoints[12 + j] = joint;
                        thumbTranslation.x -= spawner.m_ThumbJointLength;
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