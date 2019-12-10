using Unity.Entities;
using UnityEngine;

public class InverseKinematicsDataCreator : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
    {
        dstManager.AddBuffer<ThumbJointPositionBuffer>(entity);
        dstManager.AddBuffer<FingerJointPositionBuffer>(entity);
        dstManager.AddBuffer<ArmJointPositionBuffer>(entity);
        dstManager.AddBuffer<BoneMatrixBuffer>(entity);
    }
}