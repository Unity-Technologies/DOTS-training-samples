using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using Random = UnityEngine.Random;

public class PointManager : MonoBehaviour
{
    public Material barMaterial;
    public Mesh barMesh;
    [Range(0f, 1f)]
    public float damping;
    [Range(0f, 1f)]
    public float friction;
    public float breakResistance;
    [Range(0f, 1f)]
    public float tornadoForce;
    public float tornadoMaxForceDist;
    public float tornadoHeight;
    public float tornadoUpForce;
    public float tornadoInwardForce;

    Point[] m_Points;
    Bar[] m_Bars;
    int m_PointCount;

    bool m_Generating;
    public static float tornadoX;
    public static float tornadoZ;

    float m_TornadoFader;

    Matrix4x4[][] m_Matrices;
    MaterialPropertyBlock[] m_MatProps;

    Transform m_Cam;
    static readonly int k_Color = Shader.PropertyToID("_Color");

    const int k_InstancesPerBatch = 1023;

    void Awake()
    {
        Time.timeScale = 0f;
        Random.InitState(0);
    }

    void Start()
    {
        Generate();
        m_Cam = Camera.main.transform;
    }

    public static float TornadoSway(float y)
    {
        return Mathf.Sin(y / 5f + Time.time / 4f) * 3f;
    }

    void Generate()
    {
        m_Generating = true;

        List<Point> pointsList = new List<Point>();
        List<Bar> barsList = new List<Bar>();
        List<List<Matrix4x4>> matricesList = new List<List<Matrix4x4>>();
        matricesList.Add(new List<Matrix4x4>());

        using (new ProfilerMarker("CreateBuildings").Auto())
        {
            // buildings
            for (int i = 0; i < 35; i++)
            {
                int height = Random.Range(4, 12);
                Vector3 pos = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));
                float spacing = 2f;
                for (int j = 0; j < height; j++)
                {
                    {
                        var point = new Point();
                        point.pos = new float3(pos.x + spacing, j * spacing, pos.z - spacing);
                        point.old = point.pos;
                        point.anchor = j == 0;
                        pointsList.Add(point);
                    }
                    {
                        var point = new Point();
                        point.pos = new float3(pos.x - spacing, j * spacing, pos.z - spacing);
                        point.old = point.pos;
                        point.anchor = j == 0;
                        pointsList.Add(point);
                    }
                    {
                        var point = new Point();
                        point.pos = new float3(pos.x, j * spacing, pos.z + spacing);
                        point.old = point.pos;
                        point.anchor = j == 0;
                        pointsList.Add(point);
                    }
                }
            }
        }

        using (new ProfilerMarker("CreateGroundDetails").Auto())
        {
            // ground details
            for (int i = 0; i < 600; i++)
            {
                var pos = new float3(Random.Range(-55f, 55f), 0f, Random.Range(-55f, 55f));
                {
                    var point = new Point();
                    point.pos = pos + new float3(
                        Random.Range(-.2f, -.1f),
                        Random.Range(0f, 3f),
                        Random.Range(.1f, .2f)
                    );
                    point.old = point.pos;
                    pointsList.Add(point);
                }
                {
                    var point = new Point();
                    point.pos = pos + new float3(
                        Random.Range(.1f, .2f),
                        Random.Range(0, .2f),
                        Random.Range(-.1f, -.2f)
                    );
                    point.old = point.pos;
                    point.anchor = Random.value < .1f;

                    pointsList.Add(point);
                }
            }
        }

        using (new ProfilerMarker("CreateBars").Auto())
        {
            int batch = 0;

            for (int i = 0; i < pointsList.Count; i++)
            {
                for (int j = i + 1; j < pointsList.Count; j++)
                {
                    var pos1 = pointsList[i].pos;
                    var pos2 = pointsList[j].pos;
                    var delta = pos1 - pos2;
                    var l = math.length(delta);
                    if (l < 5f && l > .2f)
                    {

                        Bar bar = new Bar
                        {
                            point1 = i,
                            point2 = j,
                            length = l,
                            thickness = Random.Range(.25f, .35f)
                        };

                        var pos = (pos1 + pos2) / 2;
                        var rot = quaternion.LookRotation(delta, new float3(0, 1, 0));
                        var scale = new float3(bar.thickness, bar.thickness, bar.length);
                        bar.matrix = float4x4.TRS(pos, rot, scale);

                        var proj = math.dot(new float3(0, 1, 0), delta / l);
                        float upDot = math.acos(math.abs(proj)) / math.PI;
                        bar.color = Color.white * upDot * Random.Range(.7f, 1f);
                        
                        var p1 = pointsList[bar.point1];
                        p1.neighborCount++;
                        pointsList[bar.point1] = p1;
                        var p2 = pointsList[bar.point2];
                        p2.neighborCount++;
                        pointsList[bar.point2] = p2;

                        barsList.Add(bar);
                        matricesList[batch].Add(bar.matrix);
                        if (matricesList[batch].Count == k_InstancesPerBatch)
                        {
                            batch++;
                            matricesList.Add(new List<Matrix4x4>());
                        }
                    }
                }
            }
        }

        {
            var pointRemap = new int[pointsList.Count];
            m_Points = new Point[barsList.Count * 2];
            m_PointCount = 0;

            for (int i = 0; i < pointsList.Count; i++)
            {
                if (pointsList[i].neighborCount > 0)
                {
                    m_Points[m_PointCount] = pointsList[i];
                    pointRemap[i] = m_PointCount;
                    m_PointCount++;
                }
            }

            m_Bars = barsList.ToArray();
            for (int i = 0; i < m_Bars.Length; i++)
            {
                m_Bars[i].point1 = pointRemap[m_Bars[i].point1];
                m_Bars[i].point2 = pointRemap[m_Bars[i].point2];
            }
        }

        Debug.Log(m_PointCount + " points, room for " + m_Points.Length + " (" + barsList.Count + " bars)");

        m_Matrices = new Matrix4x4[matricesList.Count][];
        for (int i = 0; i < m_Matrices.Length; i++)
        {
            m_Matrices[i] = matricesList[i].ToArray();
        }

        m_MatProps = new MaterialPropertyBlock[barsList.Count];
        Vector4[] colors = new Vector4[k_InstancesPerBatch];
        for (int i = 0; i < barsList.Count; i++)
        {
            colors[i % k_InstancesPerBatch] = barsList[i].color;
            if ((i + 1) % k_InstancesPerBatch == 0 || i == barsList.Count - 1)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetVectorArray(k_Color, colors);
                m_MatProps[i / k_InstancesPerBatch] = block;
            }
        }

        matricesList = null;
        System.GC.Collect();
        m_Generating = false;
        Time.timeScale = 1f;
    }

    void FixedUpdate()
    {
        if (!m_Generating)
        {
            m_TornadoFader = Mathf.Clamp01(m_TornadoFader + Time.deltaTime / 10f);

            float invDamping = 1f - damping;
            for (int i = 0; i < m_PointCount; i++)
            {
                ref var point = ref m_Points[i];
                if (point.anchor == false)
                {
                    float3 start = point.pos;

                    point.old.y += .01f;

                    // tornado force
                    float tdx = tornadoX + TornadoSway(point.pos.y) - point.pos.x;
                    float tdz = tornadoZ - point.pos.z;
                    float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
                    tdx /= tornadoDist;
                    tdz /= tornadoDist;
                    if (tornadoDist < tornadoMaxForceDist)
                    {
                        float force = (1f - tornadoDist / tornadoMaxForceDist);
                        float yFader = Mathf.Clamp01(1f - point.pos.y / tornadoHeight);
                        force *= m_TornadoFader * tornadoForce * Random.Range(-.3f, 1.3f);
                        float forceY = tornadoUpForce;
                        float forceX = -tdz + tdx * tornadoInwardForce * yFader;
                        float forceZ = tdx + tdz * tornadoInwardForce * yFader;
                        point.old -= new float3(forceX, forceY, forceZ) * force;
                    }

                    point.pos += (point.pos - point.old) * invDamping;

                    point.old = start;
                    if (point.pos.y < 0f)
                    {
                        point.pos.y = 0f;
                        point.old.y = -point.old.y;
                        point.old.x += (point.pos.x - point.old.x) * friction;
                        point.old.z += (point.pos.z - point.old.z) * friction;
                    }
                }
            }

            for (int i = 0; i < m_Bars.Length; i++)
            {
                ref var bar = ref m_Bars[i];

                ref var point1 = ref m_Points[bar.point1];
                ref var point2 = ref m_Points[bar.point2];

                var delta = point2.pos - point1.pos;

                float dist = math.length(delta);
                float extraDist = dist - bar.length;

                var push = delta / dist * extraDist * .5f;
                if (!point1.anchor && !point2.anchor)
                {
                    point1.pos += push;
                    point2.pos -= push;
                }
                else if (point1.anchor)
                {
                    point2.pos -= push * 2f;
                }
                else if (point2.anchor)
                {
                    point1.pos += push * 2f;
                }

                if (math.dot(delta, bar.oldDelta) / dist < .99f)
                {
                    // bar has rotated: expensive full-matrix computation
                    bar.matrix = Matrix4x4.TRS(
                        (point1.pos + point2.pos) / 2,
                        Quaternion.LookRotation(delta),
                        new Vector3(bar.thickness, bar.thickness, bar.length));
                    bar.oldDelta = delta / dist;
                }
                else
                {
                    // bar hasn't rotated: only update the position elements
                    Matrix4x4 matrix = bar.matrix;
                    matrix.m03 = (point1.pos.x + point2.pos.x) * .5f;
                    matrix.m13 = (point1.pos.y + point2.pos.y) * .5f;
                    matrix.m23 = (point1.pos.z + point2.pos.z) * .5f;
                    bar.matrix = matrix;
                }

                if (Mathf.Abs(extraDist) > breakResistance)
                {
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;
                        Point newPoint = point2;
                        newPoint.neighborCount = 1;
                        m_Points[m_PointCount] = newPoint;
                        bar.point2 = m_PointCount;
                        m_PointCount++;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;
                        Point newPoint = point1;
                        newPoint.neighborCount = 1;
                        m_Points[m_PointCount] = newPoint;
                        bar.point1 = m_PointCount;
                        m_PointCount++;
                    }
                }

                m_Matrices[i / k_InstancesPerBatch][i % k_InstancesPerBatch] = bar.matrix;
            }
        }
    }

    void Update()
    {
        tornadoX = Mathf.Cos(Time.time / 6f) * 30f;
        tornadoZ = Mathf.Sin(Time.time / 6f * 1.618f) * 30f;
        m_Cam.position = new Vector3(tornadoX, 10f, tornadoZ) - m_Cam.forward * 60f;

        if (m_Matrices != null)
        {
            for (int i = 0; i < m_Matrices.Length; i++)
            {
                Graphics.DrawMeshInstanced(barMesh, 0, barMaterial, m_Matrices[i], m_Matrices[i].Length, m_MatProps[i]);
            }
        }
    }
}
