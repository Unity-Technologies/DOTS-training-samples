using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Capsule
{
    public  float3 vertexA;
    public  float radiusA;
    public  float3 vertexB;
    public float radiusB;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class VertexClothGarment : MonoBehaviour, IConvertGameObjectToEntity
{
    public float4[] m_SphereColliders;
    public Capsule[] m_CapsuleColliders;
    public float4[] m_PlaneColliders;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        
    }
}
