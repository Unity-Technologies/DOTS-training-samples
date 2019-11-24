using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleManager : MonoBehaviour
{
    public Mesh particleMesh;
    public Material particleMaterial;
    public float spinRate;
    public float upwardSpeed;
    NativeArray<float3> m_Points;
    Matrix4x4[] m_Matrices;
    MaterialPropertyBlock m_MatProps;
    NativeArray<float> m_RadiusMults;
    static readonly int k_Color = Shader.PropertyToID("_Color");
    const int k_NumParticles = 1000;
    
    void Start()
    {
        m_Points = new NativeArray<float3>(k_NumParticles, Allocator.Persistent);
        m_Matrices = new Matrix4x4[k_NumParticles];
        m_RadiusMults = new NativeArray<float>(k_NumParticles, Allocator.Persistent);
        Vector4[] colors = new Vector4[k_NumParticles];

        for (int i = 0; i < k_NumParticles; i++)
        {
            float3 pos = new float3(Random.Range(-50f, 50f), Random.Range(0f, 50f), Random.Range(-50f, 50f));
            m_Points[i] = pos;
            m_Matrices[i] = float4x4.TRS(m_Points[i], quaternion.identity, new float3(1) * Random.Range(.2f, .7f));
            m_RadiusMults[i] = Random.value;
            colors[i] = Color.white * Random.Range(.3f, .7f);
        }

        m_MatProps = new MaterialPropertyBlock();
        m_MatProps.SetVectorArray(k_Color, colors);
    }

    void OnDestroy()
    {
        m_RadiusMults.Dispose();
        m_Points.Dispose();
    }

    [BurstCompile]
    unsafe struct UpdateParticles : IJobParallelFor
    {
        public NativeArray<float3> Points;
        public NativeArray<float> RadiusMults;
        [NativeDisableUnsafePtrRestriction]
        public Matrix4x4* Matrices;
        public float TornadoX;
        public float TornadoZ;
        public float UpwardSpeed;
        public float SpinRate;
        public float Time;
        public float DeltaTime;
        
        public void Execute(int i)
        {
            float3 tornadoPos = new float3(
                TornadoX + PointManager.TornadoSway(Points[i].y, Time),
                Points[i].y,
                TornadoZ);
            float3 delta = (tornadoPos - Points[i]);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - math.clamp(tornadoPos.y / 50f, 0, 1) * 30f * RadiusMults[i] + 2f;
            Points[i] += new float3(
                -delta.z * SpinRate + delta.x * inForce,
                UpwardSpeed,
                delta.x * SpinRate + delta.z * inForce) * DeltaTime;
            if (Points[i].y > 50f)
            {
                Points[i] = new float3(Points[i].x, 0f, Points[i].z);
            }

            Matrices[i].m03 = Points[i].x;
            Matrices[i].m13 = Points[i].y;
            Matrices[i].m23 = Points[i].z;
        }
    }
    
    unsafe void Update()
    {
        fixed (Matrix4x4* pMatrices = m_Matrices)
        {
            new UpdateParticles
            {
                Time = Time.time,
                DeltaTime = Time.deltaTime,
                Matrices = pMatrices,
                Points = m_Points,
                RadiusMults = m_RadiusMults,
                SpinRate = spinRate,
                TornadoX = PointManager.tornadoX,
                TornadoZ = PointManager.tornadoZ,
                UpwardSpeed = upwardSpeed
            }.Schedule(m_Points.Length, 16).Complete();
        }
        Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, m_Matrices, m_Matrices.Length, m_MatProps);
    }
}
