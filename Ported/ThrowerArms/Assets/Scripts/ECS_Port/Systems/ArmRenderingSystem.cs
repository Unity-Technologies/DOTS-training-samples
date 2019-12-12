using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class ArmRenderingSystem : JobComponentSystem
{
    private EntityQuery _antQuery;
    private EntityQuery _armSharedRenderingQuery;
    private bool _allocatedBatchData;

    private readonly List<Matrix4x4[]> _matrices = new List<Matrix4x4[]>();
    private readonly List<Vector4[]> _colours = new List<Vector4[]>();
    private readonly List<GCHandle> _gcHandlesToFree = new List<GCHandle>();
    private readonly List<MaterialPropertyBlock> _materialPropertyBlocks = new List<MaterialPropertyBlock>();

    private EntityQuery m_positionBufferQuery;
    private EntityQuery m_matrixBufferQuery;
    private EntityQuery m_handUpBufferQuery;

    private List<Matrix4x4[]> matrices = new List<Matrix4x4[]>();

    private static int CountPerBatch = 1023;
    private int BoneCount;

    protected override void OnCreate()
    {
        base.OnCreate();

        m_positionBufferQuery = GetEntityQuery(ComponentType.ReadWrite<ArmJointPositionBuffer>());
        m_matrixBufferQuery = GetEntityQuery(ComponentType.ReadWrite<ArmJointMatrixBuffer>());
        m_handUpBufferQuery = GetEntityQuery(ComponentType.ReadWrite<UpVectorBufferForArmsAndFingers>());

        BoneCount = ArmSpawner.Count * (ArmConstants.ChainCount - 1 + FingerConstants.TotalChainCount - FingerConstants.CountPerArm + ThumbConstants.ChainCount - 1);
        var batchCount = ((BoneCount - 1) / CountPerBatch) +1;

        for (int i = 0; i < batchCount; i++)
            matrices.Add(new Matrix4x4[CountPerBatch]);

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var armJointPositionBuffer =
            EntityManager.GetBuffer<ArmJointPositionBuffer>(m_positionBufferQuery.GetSingletonEntity()).AsNativeArray();
        var fingerJointPositionBuffer =
            EntityManager.GetBuffer<FingerJointPositionBuffer>(m_positionBufferQuery.GetSingletonEntity()).AsNativeArray();
        var thumbJointPositionBuffer =
            EntityManager.GetBuffer<ThumbJointPositionBuffer>(m_positionBufferQuery.GetSingletonEntity()).AsNativeArray();
        var upVectorBufferForArmsAndFingers =
            EntityManager.GetBuffer<UpVectorBufferForArmsAndFingers>(m_handUpBufferQuery.GetSingletonEntity()).AsNativeArray();
        var upVectorBufferForThumbs =
            EntityManager.GetBuffer<UpVectorBufferForThumbs>(m_handUpBufferQuery.GetSingletonEntity()).AsNativeArray();


        var armJointBuffer =
            EntityManager.GetBuffer<ArmJointBuffer>(m_matrixBufferQuery.GetSingletonEntity()).AsNativeArray();
        var armUpVectorBuffer =
            EntityManager.GetBuffer<ArmUpVectorBuffer>(m_matrixBufferQuery.GetSingletonEntity()).AsNativeArray();

        var pos = 0;
        NativeArray<float3>.Copy(armJointPositionBuffer.Reinterpret<float3>(), 0, armJointBuffer.Reinterpret<float3>(), pos, armJointPositionBuffer.Length);
        pos += armJointPositionBuffer.Length;
        NativeArray<float3>.Copy( fingerJointPositionBuffer.Reinterpret<float3>(), 0, armJointBuffer.Reinterpret<float3>(), pos, fingerJointPositionBuffer.Length);
        pos += fingerJointPositionBuffer.Length;
        NativeArray<float3>.Copy(thumbJointPositionBuffer.Reinterpret<float3>(), 0, armJointBuffer.Reinterpret<float3>(), pos, thumbJointPositionBuffer.Length);

        pos = 0;
        NativeArray<float3>.Copy( upVectorBufferForArmsAndFingers.Reinterpret<float3>(), 0, armUpVectorBuffer.Reinterpret<float3>(), pos, upVectorBufferForArmsAndFingers.Length);
        pos += upVectorBufferForArmsAndFingers.Length;
        NativeArray<float3>.Copy( upVectorBufferForThumbs.Reinterpret<float3>(), 0, armUpVectorBuffer.Reinterpret<float3>(), pos, upVectorBufferForThumbs.Length);

        var armBoneBuffer =
            EntityManager.GetBuffer<ArmBoneBuffer>(m_matrixBufferQuery.GetSingletonEntity()).AsNativeArray();
        var armJointMatriceBuffer =
                    EntityManager.GetBuffer<ArmJointMatrixBuffer>(m_matrixBufferQuery.GetSingletonEntity()).AsNativeArray();

        JobHandle calculateArmMatrixJob =
            new UpdateBoneMatrixJob
            {
                BoneInfo = armBoneBuffer,
                JointPositions = armJointBuffer,
                UpVectors = armUpVectorBuffer,
                
                BoneMatrices = armJointMatriceBuffer.Reinterpret<Matrix4x4>()
            }.Schedule(
                armBoneBuffer.Length,
                innerloopBatchCount: 256,
                dependsOn: inputDeps);

        calculateArmMatrixJob.Complete();
        // Calc matrices
        int remaining = BoneCount;
        for (int i = 0; i < matrices.Count; i++)
        {
            int copyCount = math.min(remaining, CountPerBatch);

            NativeArray<Matrix4x4>.Copy(armJointMatriceBuffer.Reinterpret<Matrix4x4>(), CountPerBatch * i, matrices[i], 0, copyCount);
            remaining -= copyCount;
            Graphics.DrawMeshInstanced(ArmSpawner.SharedArmMesh, 0, ArmSpawner.SharedArmMaterial, matrices[i], copyCount);
        }

        return calculateArmMatrixJob;
    }
}
