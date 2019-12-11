using Unity.Entities;
using UnityEngine;

public class InverseKinematicsDataCreator : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
    {
        var thumbJointPositionBuffers = dstManager.AddBuffer<ThumbJointPositionBuffer>(entity);
        thumbJointPositionBuffers.ResizeUninitialized(length: ArmSpawner.SpawnCount * ArmConstants.ChainCount);
        
        dstManager.AddBuffer<FingerJointPositionBuffer>(entity);
        dstManager.AddBuffer<ArmJointPositionBuffer>(entity);
        
        
    }
}