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
        
        var fingerJointPositionBuffers = dstManager.AddBuffer<FingerJointPositionBuffer>(entity);
        fingerJointPositionBuffers.ResizeUninitialized(length: ArmSpawner.Count * FingerConstants.TotalChainCount);
        
        for (int i = 0; i < fingerJointPositionBuffers.Length; i++)
        {
            fingerJointPositionBuffers[i] = float3.zero;
        }

        var armJointPositionBuffers = dstManager.AddBuffer<ArmJointPositionBuffer>(entity);
        armJointPositionBuffers.ResizeUninitialized(ArmSpawner.Count * ArmConstants.ChainCount);
        
        for (int i = 0; i < armJointPositionBuffers.Length; i++)
        {
            armJointPositionBuffers[i] = float3.zero;
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


        var boneCount = (ArmConstants.ChainCount - 1 + FingerConstants.TotalChainCount - FingerConstants.CountPerArm + ThumbConstants.ChainCount - 1);
        var armBoneBuffers = dstManager.AddBuffer<ArmBoneBuffer>(entity);
        armBoneBuffers.ResizeUninitialized(ArmSpawner.Count * boneCount);

        int currentIndex = 0;
        int jointIndex = 0;
        for (var i = 0; i < ArmSpawner.Count; i++)
        {
            for (int j = 0; j < ArmConstants.ChainCount - 1; j++)
                armBoneBuffers[currentIndex++] = new ArmBoneBuffer() { StartIndex = jointIndex++, EndIndex = jointIndex, Thickness = ArmConstants.BoneThickness, upIndex = i };
            jointIndex++;
        }
        for (var i = 0; i < ArmSpawner.Count; i++)
        {
            for (int k = 0; k < FingerConstants.CountPerArm; k++)
            {
                for (int j = 0; j < FingerConstants.PerFingerChainCount - 1; j++)
                    armBoneBuffers[currentIndex++] = new ArmBoneBuffer() { StartIndex = jointIndex++, EndIndex = jointIndex, Thickness = FingerConstants.BoneThickness, upIndex = i };
                jointIndex++;
            }
        }
        for (var i = 0; i < ArmSpawner.Count; i++)
        {
            for (int j = 0; j < ThumbConstants.ChainCount - 1; j++)
                armBoneBuffers[currentIndex++] = new ArmBoneBuffer() { StartIndex = jointIndex++, EndIndex = jointIndex, Thickness = ThumbConstants.BoneThickness, upIndex = i+ArmSpawner.Count };
            jointIndex++;
        }

        var armJointBuffers = dstManager.AddBuffer<ArmJointBuffer>(entity);
        armJointBuffers.ResizeUninitialized(jointIndex);

        var armUpVectorBuffers = dstManager.AddBuffer<ArmUpVectorBuffer>(entity);
        armUpVectorBuffers.ResizeUninitialized(ArmSpawner.Count * 2);

        var armMatrixBuffers = dstManager.AddBuffer<ArmJointMatrixBuffer>(entity);
        armMatrixBuffers.ResizeUninitialized(ArmSpawner.Count * boneCount);

    }
}

