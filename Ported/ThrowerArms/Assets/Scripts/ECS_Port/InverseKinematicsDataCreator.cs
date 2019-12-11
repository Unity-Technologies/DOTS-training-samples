using Unity.Entities;
using UnityEngine;

public class InverseKinematicsDataCreator : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
    {
        var thumbJointPositionBuffers = dstManager.AddBuffer<ThumbJointPositionBuffer>(entity);
        thumbJointPositionBuffers.ResizeUninitialized(length: ArmSpawner.Count * ArmConstants.ChainCount);

        var fingerJointPositionBuffers = dstManager.AddBuffer<FingerJointPositionBuffer>(entity);
        fingerJointPositionBuffers.ResizeUninitialized(length: ArmSpawner.Count * FingerConstants.TotalChainCount);

        var armJointPositionBuffers = dstManager.AddBuffer<ArmJointPositionBuffer>(entity);
        armJointPositionBuffers.ResizeUninitialized(ArmSpawner.Count * ThumbConstants.ChainCount);
    }
}