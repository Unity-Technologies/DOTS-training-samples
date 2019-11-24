using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleManager : MonoBehaviour
{
    public Mesh particleMesh;
    public Material particleMaterial;
    public float spinRate;
    public float upwardSpeed;
    float3[] m_Points;
    Matrix4x4[] m_Matrices;
    MaterialPropertyBlock m_MatProps;
    float[] m_RadiusMults;
    static readonly int k_Color = Shader.PropertyToID("_Color");
    const int k_NumParticles = 1000;
    
    void Start()
    {
        m_Points = new float3[k_NumParticles];
        m_Matrices = new Matrix4x4[k_NumParticles];
        m_RadiusMults = new float[k_NumParticles];
        Vector4[] colors = new Vector4[k_NumParticles];

        for (int i = 0; i < k_NumParticles; i++)
        {
            float3 pos = new float3(Random.Range(-50f, 50f), Random.Range(0f, 50f), Random.Range(-50f, 50f));
            m_Points[i] = pos;
            m_Matrices[i] = Matrix4x4.TRS(m_Points[i], Quaternion.identity, Vector3.one * Random.Range(.2f, .7f));
            m_RadiusMults[i] = Random.value;
            colors[i] = Color.white * Random.Range(.3f, .7f);
        }

        m_MatProps = new MaterialPropertyBlock();
        m_MatProps.SetVectorArray(k_Color, colors);
    }

    void Update()
    {
        float time = Time.time;
        for (int i = 0; i < m_Points.Length; i++)
        {
            float3 tornadoPos = new float3(PointManager.tornadoX + PointManager.TornadoSway(m_Points[i].y, time), m_Points[i].y, PointManager.tornadoZ);
            float3 delta = (tornadoPos - m_Points[i]);
            float dist = math.length(delta);
            delta /= dist;
            float inForce = dist - math.clamp(tornadoPos.y / 50f, 0, 1) * 30f * m_RadiusMults[i] + 2f;
            m_Points[i] += new float3(-delta.z * spinRate + delta.x * inForce, upwardSpeed, delta.x * spinRate + delta.z * inForce) * Time.deltaTime;
            if (m_Points[i].y > 50f)
            {
                m_Points[i] = new float3(m_Points[i].x, 0f, m_Points[i].z);
            }

            Matrix4x4 matrix = m_Matrices[i];
            matrix.m03 = m_Points[i].x;
            matrix.m13 = m_Points[i].y;
            matrix.m23 = m_Points[i].z;
            m_Matrices[i] = matrix;
        }

        Graphics.DrawMeshInstanced(particleMesh, 0, particleMaterial, m_Matrices, m_Matrices.Length, m_MatProps);
    }
}
