using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public Material barMaterial;
    public Mesh barMesh;
    [Range(0f, 1f)]
    public float damping;
    [Range(0f, 1f)]
    public float friction;
    public float breakResistance;
    public float expForce;
    [Range(0f, 1f)]
    public float tornadoForce;
    public float tornadoMaxForceDist;
    public float tornadoHeight;
    public float tornadoUpForce;
    public float tornadoInwardForce;

    Point[] m_Points;
    Bar[] m_Bars;
    public int pointCount;

    bool m_Generating;
    public static float tornadoX;
    public static float tornadoZ;

    float m_TornadoFader = 0f;

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
        StartCoroutine(Generate());
        m_Cam = Camera.main.transform;
    }

    public static float TornadoSway(float y)
    {
        return Mathf.Sin(y / 5f + Time.time / 4f) * 3f;
    }

    IEnumerator Generate()
    {
        m_Generating = true;

        List<Point> pointsList = new List<Point>();
        List<Bar> barsList = new List<Bar>();
        List<List<Matrix4x4>> matricesList = new List<List<Matrix4x4>>();
        matricesList.Add(new List<Matrix4x4>());

        // buildings
        for (int i = 0; i < 35; i++)
        {
            int height = Random.Range(4, 12);
            Vector3 pos = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));
            float spacing = 2f;
            for (int j = 0; j < height; j++)
            {
                Point point = new Point();
                point.x = pos.x + spacing;
                point.y = j * spacing;
                point.z = pos.z - spacing;
                point.oldX = point.x;
                point.oldY = point.y;
                point.oldZ = point.z;
                if (j == 0)
                {
                    point.anchor = true;
                }

                pointsList.Add(point);
                point = new Point();
                point.x = pos.x - spacing;
                point.y = j * spacing;
                point.z = pos.z - spacing;
                point.oldX = point.x;
                point.oldY = point.y;
                point.oldZ = point.z;
                if (j == 0)
                {
                    point.anchor = true;
                }

                pointsList.Add(point);
                point = new Point();
                point.x = pos.x + 0f;
                point.y = j * spacing;
                point.z = pos.z + spacing;
                point.oldX = point.x;
                point.oldY = point.y;
                point.oldZ = point.z;
                if (j == 0)
                {
                    point.anchor = true;
                }

                pointsList.Add(point);
            }
        }

        // ground details
        for (int i = 0; i < 600; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-55f, 55f), 0f, Random.Range(-55f, 55f));
            Point point = new Point();
            point.x = pos.x + Random.Range(-.2f, -.1f);
            point.y = pos.y + Random.Range(0f, 3f);
            point.z = pos.z + Random.Range(.1f, .2f);
            point.oldX = point.x;
            point.oldY = point.y;
            point.oldZ = point.z;
            pointsList.Add(point);

            point = new Point();
            point.x = pos.x + Random.Range(.2f, .1f);
            point.y = pos.y + Random.Range(0f, .2f);
            point.z = pos.z + Random.Range(-.1f, -.2f);
            point.oldX = point.x;
            point.oldY = point.y;
            point.oldZ = point.z;
            if (Random.value < .1f)
            {
                point.anchor = true;
            }

            pointsList.Add(point);
        }

        int batch = 0;

        for (int i = 0; i < pointsList.Count; i++)
        {
            for (int j = i + 1; j < pointsList.Count; j++)
            {
                Bar bar = new Bar();
                bar.AssignPoints(pointsList[i], pointsList[j]);
                if (bar.length < 5f && bar.length > .2f)
                {
                    bar.point1.neighborCount++;
                    bar.point2.neighborCount++;

                    barsList.Add(bar);
                    matricesList[batch].Add(bar.matrix);
                    if (matricesList[batch].Count == k_InstancesPerBatch)
                    {
                        batch++;
                        matricesList.Add(new List<Matrix4x4>());
                    }

                    if (barsList.Count % 500 == 0)
                    {
                        yield return null;
                    }
                }
            }
        }

        m_Points = new Point[barsList.Count * 2];
        pointCount = 0;
        for (int i = 0; i < pointsList.Count; i++)
        {
            if (pointsList[i].neighborCount > 0)
            {
                m_Points[pointCount] = pointsList[i];
                pointCount++;
            }
        }

        Debug.Log(pointCount + " points, room for " + m_Points.Length + " (" + barsList.Count + " bars)");

        m_Bars = barsList.ToArray();

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

        pointsList = null;
        barsList = null;
        matricesList = null;
        System.GC.Collect();
        m_Generating = false;
        Time.timeScale = 1f;
    }

    void FixedUpdate()
    {
        if (m_Generating == false)
        {
            m_TornadoFader = Mathf.Clamp01(m_TornadoFader + Time.deltaTime / 10f);

            float invDamping = 1f - damping;
            for (int i = 0; i < pointCount; i++)
            {
                Point point = m_Points[i];
                if (point.anchor == false)
                {
                    float startX = point.x;
                    float startY = point.y;
                    float startZ = point.z;

                    point.oldY += .01f;

                    // tornado force
                    float tdx = tornadoX + TornadoSway(point.y) - point.x;
                    float tdz = tornadoZ - point.z;
                    float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
                    tdx /= tornadoDist;
                    tdz /= tornadoDist;
                    if (tornadoDist < tornadoMaxForceDist)
                    {
                        float force = (1f - tornadoDist / tornadoMaxForceDist);
                        float yFader = Mathf.Clamp01(1f - point.y / tornadoHeight);
                        force *= m_TornadoFader * tornadoForce * Random.Range(-.3f, 1.3f);
                        float forceY = tornadoUpForce;
                        point.oldY -= forceY * force;
                        float forceX = -tdz + tdx * tornadoInwardForce * yFader;
                        float forceZ = tdx + tdz * tornadoInwardForce * yFader;
                        point.oldX -= forceX * force;
                        point.oldZ -= forceZ * force;
                    }

                    point.x += (point.x - point.oldX) * invDamping;
                    point.y += (point.y - point.oldY) * invDamping;
                    point.z += (point.z - point.oldZ) * invDamping;

                    point.oldX = startX;
                    point.oldY = startY;
                    point.oldZ = startZ;
                    if (point.y < 0f)
                    {
                        point.y = 0f;
                        point.oldY = -point.oldY;
                        point.oldX += (point.x - point.oldX) * friction;
                        point.oldZ += (point.z - point.oldZ) * friction;
                    }
                }
            }

            for (int i = 0; i < m_Bars.Length; i++)
            {
                Bar bar = m_Bars[i];

                Point point1 = bar.point1;
                Point point2 = bar.point2;

                float dx = point2.x - point1.x;
                float dy = point2.y - point1.y;
                float dz = point2.z - point1.z;

                float dist = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
                float extraDist = dist - bar.length;

                float pushX = (dx / dist * extraDist) * .5f;
                float pushY = (dy / dist * extraDist) * .5f;
                float pushZ = (dz / dist * extraDist) * .5f;

                if (point1.anchor == false && point2.anchor == false)
                {
                    point1.x += pushX;
                    point1.y += pushY;
                    point1.z += pushZ;
                    point2.x -= pushX;
                    point2.y -= pushY;
                    point2.z -= pushZ;
                }
                else if (point1.anchor)
                {
                    point2.x -= pushX * 2f;
                    point2.y -= pushY * 2f;
                    point2.z -= pushZ * 2f;
                }
                else if (point2.anchor)
                {
                    point1.x += pushX * 2f;
                    point1.y += pushY * 2f;
                    point1.z += pushZ * 2f;
                }

                if (dx / dist * bar.oldDX + dy / dist * bar.oldDY + dz / dist * bar.oldDZ < .99f)
                {
                    // bar has rotated: expensive full-matrix computation
                    bar.matrix = Matrix4x4.TRS(new Vector3((point1.x + point2.x) * .5f, (point1.y + point2.y) * .5f, (point1.z + point2.z) * .5f),
                        Quaternion.LookRotation(new Vector3(dx, dy, dz)),
                        new Vector3(bar.thickness, bar.thickness, bar.length));
                    bar.oldDX = dx / dist;
                    bar.oldDY = dy / dist;
                    bar.oldDZ = dz / dist;
                }
                else
                {
                    // bar hasn't rotated: only update the position elements
                    Matrix4x4 matrix = bar.matrix;
                    matrix.m03 = (point1.x + point2.x) * .5f;
                    matrix.m13 = (point1.y + point2.y) * .5f;
                    matrix.m23 = (point1.z + point2.z) * .5f;
                    bar.matrix = matrix;
                }

                if (Mathf.Abs(extraDist) > breakResistance)
                {
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;
                        Point newPoint = new Point();
                        newPoint.CopyFrom(point2);
                        newPoint.neighborCount = 1;
                        m_Points[pointCount] = newPoint;
                        bar.point2 = newPoint;
                        pointCount++;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;
                        Point newPoint = new Point();
                        newPoint.CopyFrom(point1);
                        newPoint.neighborCount = 1;
                        m_Points[pointCount] = newPoint;
                        bar.point1 = newPoint;
                        pointCount++;
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
