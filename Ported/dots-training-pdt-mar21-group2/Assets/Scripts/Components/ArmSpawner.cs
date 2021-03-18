using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct ArmSpawner : IComponentData
{
    public Entity m_ArmPrefab;
    public Entity m_JointPrefab;
    public Entity m_JointBoxPrefab;
    
    public float m_ArmJointThickness;

    public float m_FingerSpacing;
    public float m_FingerJointSpacing;
    public float m_ThumbJointThickness;
    public float m_ThumbJointLength;
    public float m_FingerJointThickness;
    public float m_Finger0JointLength;
    public float m_Finger1JointLength;
    public float m_Finger2JointLength;
    public float m_Finger3JointLength;
}