using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct UpdateBoneMatrixJob : IJobParallelFor
{
    [ReadOnly] public float BoneThickness;
    [ReadOnly] public NativeArray<float3> UpVectorsForMatrixCalculations;
    [ReadOnly] public NativeArray<float3> BoneTranslations;
    [ReadOnly] public int NumBoneTranslationsPerArm;
    
    [WriteOnly] public NativeArray<Matrix4x4> BoneMatrices;
    
    public void Execute(int index)
    {
        float3 delta = BoneTranslations[index + 1] - BoneTranslations[index];
        BoneMatrices[index] = 
            Matrix4x4.TRS(
                BoneTranslations[index] + delta * 0.5f, 
                quaternion.LookRotation(
                    delta, UpVectorsForMatrixCalculations[index / NumBoneTranslationsPerArm]),
                new float3(BoneThickness, BoneThickness, math.length(delta)));
    }
}