using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InverseKinematicsDataCreator : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
    {
        var thumbJointPositionBuffers = dstManager.AddBuffer<ThumbJointPositionBuffer>(entity);
        thumbJointPositionBuffers.ResizeUninitialized(length: ArmSpawner.Count * ThumbConstants.ChainCount);
        
        for (int i = 0; i < thumbJointPositionBuffers.Length; i++)
        {
            thumbJointPositionBuffers[i] = float3.zero;
        }
        
        var thumbJointMatrixBuffers = dstManager.AddBuffer<ThumbJointMatrixBuffer>(entity);
        thumbJointMatrixBuffers.ResizeUninitialized(length: ArmSpawner.Count * ThumbConstants.ChainCount);
        
        for (int i = 0; i < thumbJointMatrixBuffers.Length; i++)
        {
            thumbJointMatrixBuffers[i] = Matrix4x4.zero;
        }

        var fingerJointPositionBuffers = dstManager.AddBuffer<FingerJointPositionBuffer>(entity);
        fingerJointPositionBuffers.ResizeUninitialized(length: ArmSpawner.Count * FingerConstants.TotalChainCount);
        
        for (int i = 0; i < fingerJointPositionBuffers.Length; i++)
        {
            fingerJointPositionBuffers[i] = float3.zero;
        }
        
        var fingerJointMatrixBuffers = dstManager.AddBuffer<FingerJointMatrixBuffer>(entity);
        fingerJointMatrixBuffers.ResizeUninitialized(length: ArmSpawner.Count * FingerConstants.TotalChainCount);
        
        for (int i = 0; i < fingerJointMatrixBuffers.Length; i++)
        {
            fingerJointMatrixBuffers[i] = Matrix4x4.zero;
        }
        
        var armJointPositionBuffers = dstManager.AddBuffer<ArmJointPositionBuffer>(entity);
        armJointPositionBuffers.ResizeUninitialized(ArmSpawner.Count * ArmConstants.ChainCount);
        
        for (int i = 0; i < armJointPositionBuffers.Length; i++)
        {
            armJointPositionBuffers[i] = float3.zero;
        }
        
        var armJointMatrixBuffers = dstManager.AddBuffer<ArmJointMatrixBuffer>(entity);
        armJointMatrixBuffers.ResizeUninitialized(ArmSpawner.Count * ArmConstants.ChainCount);
        
        for (int i = 0; i < armJointMatrixBuffers.Length; i++)
        {
            armJointMatrixBuffers[i] = Matrix4x4.zero;
        }

        var upVectorBufferForArmsAndFingers = dstManager.AddBuffer<UpVectorBufferForArmsAndFingers>(entity);
        upVectorBufferForArmsAndFingers.ResizeUninitialized(ArmSpawner.Count);
        
        for (int i = 0; i < upVectorBufferForArmsAndFingers.Length; i++)
        {
            upVectorBufferForArmsAndFingers[i] = float3.zero;
        }

        var upVectorBufferForThumbs = dstManager.AddBuffer<UpVectorBufferForThumbs>(entity);
        upVectorBufferForThumbs.ResizeUninitialized(ArmSpawner.Count);
        
        for (int i = 0; i < upVectorBufferForThumbs.Length; i++)
        {
            upVectorBufferForThumbs[i] = float3.zero;
        }

        
    }
}