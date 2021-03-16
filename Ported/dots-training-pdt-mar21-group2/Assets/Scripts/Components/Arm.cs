using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Arm : IComponentData
{
    public Entity m_Humerus;
    public Entity m_Forearm;
    public Entity m_ThumbJoint0;
    public Entity m_ThumbJoint1;
    public Entity m_ThumbJoint2;
    public Entity m_Finger0Joint0;
    public Entity m_Finger0Joint1;
    public Entity m_Finger0Joint2;
    public Entity m_Finger1Joint0;
    public Entity m_Finger1Joint1;
    public Entity m_Finger1Joint2;
    public Entity m_Finger2Joint0;
    public Entity m_Finger2Joint1;
    public Entity m_Finger2Joint2;
    public Entity m_Finger3Joint0;
    public Entity m_Finger3Joint1;
    public Entity m_Finger3Joint2;
}
