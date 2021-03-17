using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;

public class ArmSpawnerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject m_ArmPrefab;
    [Range(0, 1000)] public uint m_ArmCount;
    public GameObject m_JointPrefab;
    public GameObject m_JointBoxPrefab;
    public float m_ArmSeparation = 1.0f;
    public float m_ArmJointLength = 1.0f;
    public float m_ArmJointThickness = 0.15f;
    public float m_ArmJointSpacing = 0.0f;
    public float m_FingerSpacing = 0.08f;
    public float m_FingerJointSpacing = 0.0f;
    public float m_ThumbJointThickness = 0.06f;
    public float m_ThumbJointLength = 0.13f;
    public float m_FingerJointThickness = 0.05f;
    public float m_Finger0JointLength = 0.2f;
    public float m_Finger1JointLength = 0.22f;
    public float m_Finger2JointLength = 0.2f;
    public float m_Finger3JointLength = 0.16f;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(m_ArmPrefab);
        referencedPrefabs.Add(m_JointPrefab);
        referencedPrefabs.Add(m_JointBoxPrefab);
    }

    // This function is required by IConvertGameObjectToEntity
    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        // GetPrimaryEntity fetches the entity that resulted from the conversion of
        // the given GameObject, but of course this GameObject needs to be part of
        // the conversion, that's why DeclareReferencedPrefabs is important here.
        dstManager.AddComponentData(entity, new ArmSpawner
        {
            m_ArmPrefab = conversionSystem.GetPrimaryEntity(m_ArmPrefab),
            m_ArmCount = m_ArmCount,
            m_JointPrefab = conversionSystem.GetPrimaryEntity(m_JointPrefab),
            m_JointBoxPrefab = conversionSystem.GetPrimaryEntity(m_JointBoxPrefab),
            m_ArmSeparation = m_ArmSeparation,
            m_ArmJointLength = m_ArmJointLength,
            m_ArmJointThickness = m_ArmJointThickness,
            m_ArmJointSpacing = m_ArmJointSpacing,
            m_FingerSpacing = m_FingerSpacing,
            m_FingerJointSpacing = m_FingerJointSpacing,
            m_ThumbJointThickness = m_ThumbJointThickness,
            m_ThumbJointLength = m_ThumbJointLength,
            m_FingerJointThickness = m_FingerJointThickness,
            m_Finger0JointLength = m_Finger0JointLength,
            m_Finger1JointLength = m_Finger1JointLength,
            m_Finger2JointLength = m_Finger2JointLength,
            m_Finger3JointLength = m_Finger3JointLength,
        });
    }
}