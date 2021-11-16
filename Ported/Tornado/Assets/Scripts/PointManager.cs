using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class PointManager : MonoBehaviour, IConvertGameObjectToEntity
{
    public Mesh barMesh;
    public Material barMaterial;
    public MeshRenderer ground;
    public Tornado[] tornados;

    public int buildingCount = 35;
    public int groundDetailCount = 600;

    public float breakResistance;
    [Range(0f, 1f)] public float damping;
    [Range(0f, 1f)] public float friction;

    Bar[] bars;
    Point[] points;
    int pointCount;

    Matrix4x4[][] matrices;
    MaterialPropertyBlock[] matProps;

    const int instancesPerBatch = 1023;

    internal void Awake()
    {
        Time.timeScale = 0f;
    }

    internal void Start()
    {
        using (new DebugTimer("Generate buildings"))
            Generate();
    }

    void Generate()
    {
        // buildings
        const float spacing = 2f;
        var pointsList = new List<Point>(buildingCount * 18);
        var groundLimits = ground.bounds.extents;
        var buildingGroundLimits = groundLimits * 0.9f;
        var maxTornadoHeight = Mathf.FloorToInt(tornados.Max(t => t.height) * .9f / spacing);
        
        for (int i = 0; i < buildingCount; i++)
        {
            int height = Random.Range(4, maxTornadoHeight);
            var pos = new Vector3(Random.Range(-buildingGroundLimits.x, buildingGroundLimits.x), 0f, Random.Range(-buildingGroundLimits.z, buildingGroundLimits.z));
            for (int j = 0; j < height; j++)
            {
                var anchored = j == 0;
                var jspace = j * spacing;
                pointsList.Add(new Point(pos.x + spacing, jspace, pos.z - spacing, anchored));
                pointsList.Add(new Point(pos.x - spacing, jspace, pos.z - spacing, anchored));
                pointsList.Add(new Point(pos.x + 0f, jspace, pos.z + spacing, anchored));
            }
        }

        // ground details
        for (int i = 0; i < groundDetailCount; i++)
        {
            var pos = new Vector3(Random.Range(-groundLimits.x, groundLimits.x), 0f, Random.Range(-groundLimits.z, groundLimits.z));
            pointsList.Add(new Point(pos.x + Random.Range(-.2f, -.1f), pos.y + Random.Range(0f, 3f), pos.z + Random.Range(.1f, .2f)));
            pointsList.Add(new Point(pos.x + Random.Range(.2f, .1f), pos.y + Random.Range(0f, .2f), pos.z + Random.Range(-.1f, -.2f), Random.value < .1f));
        }

        int batch = 0;
        var barsList = new List<Bar>();
        var matricesList = new List<List<Matrix4x4>> { new List<Matrix4x4>() };
        for (int i = 0; i < pointsList.Count; i++)
        {
            for (int j = i + 1; j < pointsList.Count; j++)
            {
                Bar bar = new Bar(pointsList[i], pointsList[j]);
                if (bar.length < 5f && bar.length > .2f)
                {
                    bar.point1.neighborCount++;
                    bar.point2.neighborCount++;

                    barsList.Add(bar);
                    matricesList[batch].Add(bar.matrix);
                    if (matricesList[batch].Count == instancesPerBatch)
                    {
                        batch++;
                        matricesList.Add(new List<Matrix4x4>());
                    }
                }
            }
        }
        
        pointCount = 0;
        points = new Point[barsList.Count * 2];
        for (int i = 0; i < pointsList.Count; i++)
        {
            if (pointsList[i].neighborCount > 0)
            {
                points[pointCount] = pointsList[i];
                pointCount++;
            }
        }
        
        Debug.Log(pointCount + " points, room for " + points.Length + " (" + barsList.Count + " bars)");

        bars = barsList.ToArray();

        matrices = new Matrix4x4[matricesList.Count][];
        for (int i = 0; i < matrices.Length; i++)
            matrices[i] = matricesList[i].ToArray();

        matProps = new MaterialPropertyBlock[barsList.Count];
        var colors = new Vector4[instancesPerBatch];
        for (int i = 0; i < barsList.Count; i++)
        {
            colors[i % instancesPerBatch] = barsList[i].color;
            if ((i + 1) % instancesPerBatch == 0 || i == barsList.Count - 1)
            {
                var block = new MaterialPropertyBlock();
                block.SetVectorArray("_Color", colors);
                matProps[i / instancesPerBatch] = block;
            }
        }

        Time.timeScale = 1f;
    }

    internal void FixedUpdate()
    {
        foreach (var tornado in tornados)
        {
            if (!tornado.simulate)
                continue;

            float invDamping = 1f - damping;
            for (int i = 0; i < pointCount; i++)
            {
                Point point = points[i];
                if (point.anchor != false)
                    continue;

                var start = point.position;
                point.oldPosition.y += .01f;

                // tornado force
                float tdx = tornado.x + Tornado.Sway(point.y) - point.x;
                float tdz = tornado.y - point.z;
                float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
                tdx /= tornadoDist;
                tdz /= tornadoDist;
                if (tornadoDist < tornado.maxForceDist)
                {
                    float force = (1f - tornadoDist / tornado.maxForceDist);
                    float yFader = Mathf.Clamp01(1f - point.y / tornado.height);
                    force *= tornado.fader * tornado.force * Random.Range(-.3f, 1.3f);
                    float forceY = tornado.upForce;
                    point.oldPosition.y -= forceY * force;
                    float forceX = -tdz + tdx * tornado.inwardForce * yFader;
                    float forceZ = tdx + tdz * tornado.inwardForce * yFader;
                    point.oldPosition.x -= forceX * force;
                    point.oldPosition.z -= forceZ * force;
                }

                point.Damp(invDamping);
                point.SetPrevious(start);

                if (point.y < 0f)
                {
                    point.y = 0f;
                    point.oldPosition.y = -point.oldPosition.y;
                    point.oldPosition.x += (point.x - point.oldPosition.x) * friction;
                    point.oldPosition.z += (point.z - point.oldPosition.z) * friction;
                }

                points[i] = point;
            }

            for (int i = 0; i < bars.Length; i++)
            {
                Bar bar = bars[i];

                Point point1 = bar.point1;
                Point point2 = bar.point2;

                var delta = point2.position - point1.position;
                var dist = delta.magnitude;
                var extraDist = dist - bar.length;
                var push = (delta / dist * extraDist) * .5f;

                if (point1.anchor == false && point2.anchor == false)
                {
                    point1.position += push;
                    point2.position -= push;
                }
                else if (point1.anchor)
                {
                    point2.position -= push * 2f;
                }
                else if (point2.anchor)
                {
                    point1.position += push * 2f;
                }

                if (delta.x / dist * bar.delta.x + delta.y / dist * bar.delta.y + delta.z / dist * bar.delta.z < .99f)
                {
                    // bar has rotated: expensive full-matrix computation
                    bar.matrix = Matrix4x4.TRS((point1.position + point2.position) * .5f,
                                           Quaternion.LookRotation(new Vector3(delta.x, delta.y, delta.z)),
                                           new Vector3(bar.thickness, bar.thickness, bar.length));
                    bar.delta = delta / dist;
                }
                else
                {
                    // bar hasn't rotated: only update the position elements
                    var matrix = bar.matrix;
                    var translate = (point1.position + point2.position) * .5f;
                    matrix.m03 = translate.x;
                    matrix.m13 = translate.y;
                    matrix.m23 = translate.z;
                    bar.matrix = matrix;
                }

                if (Mathf.Abs(extraDist) > breakResistance)
                {
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;
                        var newPoint = new Point(point2, 1);
                        points[pointCount++] = newPoint;
                        bar.point2 = newPoint;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;
                        var newPoint = new Point(point1, 1);
                        points[pointCount++] = newPoint;
                        bar.point1 = newPoint;
                    }
                }

                bar.min = new Vector3(Mathf.Min(point1.x, point2.x), Mathf.Min(point1.y, point2.y), Mathf.Min(point1.z, point2.z));
                bar.max = new Vector3(Mathf.Max(point1.x, point2.x), Mathf.Max(point1.y, point2.y), Mathf.Max(point1.z, point2.z));

                matrices[i / instancesPerBatch][i % instancesPerBatch] = bar.matrix;

//                 bar.point1 = point1;
//                 bar.point2 = point2;
            }
        }
    }

    internal void Update()
    {
        if (matrices == null)
            return;
        
        for (int i = 0; i < matrices.Length; i++)
            Graphics.DrawMeshInstanced(barMesh, 0, barMaterial, matrices[i], matrices[i].Length, matProps[i]);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        UnityEngine.Debug.Log("Convert points");
    }
}
