using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public Mesh particleMesh;
    public Material particleMaterial;
    public float spinRate;
    public float upwardSpeed;
    Vector3[] m_Points;
    Matrix4x4[] m_Matrices;
    MaterialPropertyBlock m_MatProps;
    float[] m_RadiusMults;

    void Start()
    {
        m_Points = new Vector3[1000];
        m_Matrices = new Matrix4x4[1000];
        m_RadiusMults = new float[1000];
        Vector4[] colors = new Vector4[1000];

        for (int i = 0; i < m_Points.Length; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-50f, 50f), Random.Range(0f, 50f), Random.Range(-50f, 50f));
            m_Points[i] = pos;
            m_Matrices[i] = Matrix4x4.TRS(m_Points[i], Quaternion.identity, Vector3.one * Random.Range(.2f, .7f));
            m_RadiusMults[i] = Random.value;
            colors[i] = Color.white * Random.Range(.3f, .7f);
        }

        m_MatProps = new MaterialPropertyBlock();
        m_MatProps.SetVectorArray("_Color", colors);
    }

    void Update()
    {
        for (int i = 0; i < m_Points.Length; i++)
        {
            Vector3 tornadoPos = new Vector3(PointManager.tornadoX + PointManager.TornadoSway(m_Points[i].y), m_Points[i].y, PointManager.tornadoZ);
            Vector3 delta = (tornadoPos - m_Points[i]);
            float dist = delta.magnitude;
            delta /= dist;
            float inForce = dist - Mathf.Clamp01(tornadoPos.y / 50f) * 30f * m_RadiusMults[i] + 2f;
            m_Points[i] += new Vector3(-delta.z * spinRate + delta.x * inForce, upwardSpeed, delta.x * spinRate + delta.z * inForce) * Time.deltaTime;
            if (m_Points[i].y > 50f)
            {
                m_Points[i] = new Vector3(m_Points[i].x, 0f, m_Points[i].z);
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
