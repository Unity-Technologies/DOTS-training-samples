using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class AnimationSystem : SystemBase
{
    private float m_Rotation = 0.0f;
    private bool m_Reverse = false;
    
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        if (m_Reverse)
        {
            deltaTime *= -1.0f;
        }
        float rotationX = m_Rotation;
        var rotations = GetComponentDataFromEntity<Rotation>();
        Entities
            .ForEach((ref Arm arm) =>
            {
                var joints = new NativeArray<Entity>(11, Unity.Collections.Allocator.Temp);
                
                joints[0] = arm.m_ThumbJoint0;
                joints[1] = arm.m_ThumbJoint1;
                joints[2] = arm.m_ThumbJoint2;
                joints[3] = arm.m_Finger0Joint1;
                joints[4] = arm.m_Finger0Joint2;
                joints[5] = arm.m_Finger1Joint1;
                joints[6] = arm.m_Finger1Joint2;
                joints[7] = arm.m_Finger2Joint1;
                joints[8] = arm.m_Finger2Joint2;
                joints[9] = arm.m_Finger3Joint1;
                joints[10] = arm.m_Finger3Joint2;

                foreach (var joint in joints)
                {
                    var rotation = rotations[joint];
                    rotation.Value = math.mul(quaternion.RotateX(deltaTime), rotation.Value);
                    rotations[joint] = rotation;
                }
            }).Run();

        m_Rotation += deltaTime;
        if (m_Rotation < 0.0f || m_Rotation > Mathf.PI * 0.5f)
        {
            m_Reverse = !m_Reverse;
        }
    }
}
