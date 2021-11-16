using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class PointManager : MonoBehaviour
{
    public Mesh barMesh;
    public Material barMaterial;

    public int buildingCount;
    public int groundDetailCount;
    public float breakResistance;
    [Range(0f, 1f)] public float damping;
    [Range(0f, 1f)] public float friction;

    Tornado[] tornados;
    MeshRenderer ground;
    Matrix4x4[][] matrices;
    MaterialPropertyBlock[] matProps;
    float3 groundExtents;
    int maxTornadoHeight;

    const float spacing = 2f;
    const int instancesPerBatch = 1023;

    internal void Awake()
    {
        Time.timeScale = 0f;

        ground = FindObjectOfType<Ground>().gameObject.GetComponent<MeshRenderer>();
        tornados = Resources.FindObjectsOfTypeAll<Tornado>();

        groundExtents = ground.bounds.extents;
        maxTornadoHeight = Mathf.FloorToInt(tornados.Max(t => t.height) * .9f / spacing);
    }

    internal void Start()
    {
        using (new DebugTimer("Generate buildings"))
            Generate();

        Time.timeScale = 1f;
    }

    internal void FixedUpdate()
    {
        using (new DebugTimer("Simulate"))
            Simulate();
    }

    internal void Update()
    {
        for (int i = 0; i < matrices.Length; i++)
            Graphics.DrawMeshInstanced(barMesh, 0, barMaterial, matrices[i], matrices[i].Length, matProps[i]);
    }

    void Generate()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var random = new Unity.Mathematics.Random(1234);

        // Create buildings
        const float spacing = 2f;
        var groundLimits = groundExtents;
        var buildingGroundLimits = groundLimits * 0.9f;

        for (int i = 0; i < buildingCount; i++)
        {
            int height = random.NextInt(4, maxTornadoHeight);
            var pos = random.NextFloat3(-buildingGroundLimits, buildingGroundLimits);
            for (int j = 0; j < height; j++)
            {
                var anchored = j == 0;
                var jspace = j * spacing;

                var point = em.CreateEntity();
                em.AddComponent<Point>(point);
                em.SetComponentData(point, new Point(pos.x + spacing, jspace, pos.z - spacing, anchored));

                point = em.CreateEntity();
                em.AddComponent<Point>(point);
                em.SetComponentData(point, new Point(pos.x - spacing, jspace, pos.z - spacing, anchored));

                point = em.CreateEntity();
                em.AddComponent<Point>(point);
                em.SetComponentData(point, new Point(pos.x + 0f, jspace, pos.z + spacing, anchored));
            }
        }

        // Create ground details
        for (int i = 0; i < groundDetailCount; i++)
        {
            var pos = random.NextFloat3(-groundLimits, groundLimits);

            var point = em.CreateEntity();
            em.AddComponent<Point>(point);
            em.SetComponentData(point, new Point(pos.x + random.NextFloat(-.2f, -.1f), pos.y + random.NextFloat(0f, 3f), pos.z + random.NextFloat(.1f, .2f), false));

            point = em.CreateEntity();
            em.AddComponent<Point>(point);
            em.SetComponentData(point, new Point(pos.x + random.NextFloat(.2f, .1f), pos.y + random.NextFloat(0f, .2f), pos.z + random.NextFloat(-.1f, -.2f), UnityEngine.Random.value < .1f));
        }

        // Create beams
        int batch = 0;
        var matricesList = new List<List<Matrix4x4>> { new List<Matrix4x4>() };
        int beamCount = 0;
        using (var pointQuery = em.CreateEntityQuery(typeof(Point)))
        using (var points = pointQuery.ToEntityArray(Allocator.TempJob))
        {
            Debug.Log($"{points.Length} points");
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    var a = em.GetComponentData<Point>(points[i]);
                    var b = em.GetComponentData<Point>(points[j]);
                    var delta = (b.position - a.position);
                    var length = math.length(delta);
                    if (length >= 5f || length <= .2f)
                        continue;

                    ++a.neighborCount;
                    ++b.neighborCount;

                    em.SetComponentData(points[i], a);
                    em.SetComponentData(points[j], b);

                    var beam = new Beam(points[i], points[j], random.NextFloat(.25f, .35f), length);

                    var barLength = math.length(delta);
                    var norm = delta / barLength;

                    var pos = (a.position + b.position) * .5f;
                    var rot = Quaternion.LookRotation(delta);
                    var scale = new Vector3(beam.thickness, beam.thickness, barLength);
                    var beamMatrix = Matrix4x4.TRS(pos, rot, scale);

                    beam.m1i = batch;
                    beam.m2i = matricesList[batch].Count;
                    matricesList[batch].Add(beamMatrix);
                    if (matricesList[batch].Count == instancesPerBatch)
                    {
                        batch++;
                        matricesList.Add(new List<Matrix4x4>());
                    }

                    var beamEntity = em.CreateEntity();
                    em.AddComponent<Beam>(beamEntity);
                    em.SetComponentData(beamEntity, beam);

                    beamCount++;
                }
            }
        }

        int pointCount = 0;
        using (var pointQuery = em.CreateEntityQuery(typeof(Point)))
        using (var points = pointQuery.ToEntityArray(Allocator.TempJob))
        {
            for (int i = 0; i < points.Length; i++)
            {
                var point = em.GetComponentData<Point>(points[i]);
                if (point.neighborCount == 0)
                    em.DestroyEntity(points[i]);
                else
                    pointCount++;
            }
        }

        matrices = new Matrix4x4[matricesList.Count][];
        for (int i = 0; i < matrices.Length; i++)
            matrices[i] = matricesList[i].ToArray();

        matProps = new MaterialPropertyBlock[beamCount];
        using (var bq = em.CreateEntityQuery(typeof(Beam)))
        using (var beams = bq.ToEntityArray(Allocator.TempJob))
        {
            var colors = new Vector4[instancesPerBatch];
            for (int i = 0; i < beamCount; i++)
            {
                var beamData = em.GetComponentData<Beam>(beams[i]);
                var point1 = em.GetComponentData<Point>(beamData.point1);
                var point2 = em.GetComponentData<Point>(beamData.point2);
                var delta = point2.position - point1.position;

                float upDot = math.acos(math.abs(math.dot(math.up(), math.normalize(delta)))) / math.PI;
                var color = Color.white * upDot * random.NextFloat(.7f, 1f);

                colors[i % instancesPerBatch] = color;
                if ((i + 1) % instancesPerBatch == 0 || i == beamCount - 1)
                {
                    var block = new MaterialPropertyBlock();
                    block.SetVectorArray("_Color", colors);
                    matProps[i / instancesPerBatch] = block;
                }
            }
        }

        Debug.Log($"Generated {pointCount} points and {beamCount} beams");
    }

    void Simulate()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var activeTornados = tornados.Where(t => t.simulate).ToArray();
        var random = new Unity.Mathematics.Random(1234);

        float invDamping = 1f - damping;
        using (var pointQuery = em.CreateEntityQuery(typeof(Point)))
        using (var points = pointQuery.ToEntityArray(Allocator.TempJob))
        {
            for (int i = 0; i < points.Length; i++)
            {
                var point = em.GetComponentData<Point>(points[i]);
                // TODO: Create AnchoredPointData and NonAchoredPointData
                if (point.anchor)
                    continue;

                var start = point.position;
                point.old.y += .01f;

                foreach (var tornado in activeTornados)
                {
                    // tornado force
                    float tdx = tornado.x + Tornado.Sway(point.position.y) - point.position.x;
                    float tdz = tornado.y - point.position.z;
                    float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
                    tdx /= tornadoDist;
                    tdz /= tornadoDist;
                    if (tornadoDist < tornado.maxForceDist)
                    {
                        float force = (1f - tornadoDist / tornado.maxForceDist);
                        float yFader = Mathf.Clamp01(1f - point.position.y / tornado.height);
                        force *= tornado.fader * tornado.force * random.NextFloat(-.3f, 1.3f);
                        float forceY = tornado.upForce;
                        point.old.y -= forceY * force;
                        float forceX = -tdz + tdx * tornado.inwardForce * yFader;
                        float forceZ = tdx + tdz * tornado.inwardForce * yFader;
                        point.old.x -= forceX * force;
                        point.old.z -= forceZ * force;

                        // TODO: ecb create affected beam
                    }
                }

                point.position += (point.position - point.old) * invDamping;
                point.old = start;

                if (point.position.y < 0f)
                {
                    point.position.y = 0f;
                    point.old.y = -point.old.y;
                    point.old.x += (point.position.x - point.position.x) * friction;
                    point.old.z += (point.position.z - point.position.z) * friction;
                }

                // Write back point data
                em.SetComponentData(points[i], point);
            }
        }

        using (var bq = em.CreateEntityQuery(typeof(Beam)))
        using (var beams = bq.ToEntityArray(Allocator.TempJob))
        {
            for (int i = 0; i < beams.Length; ++i)
            {
                var beam = em.GetComponentData<Beam>(beams[i]);
                var point1 = em.GetComponentData<Point>(beam.point1);
                var point2 = em.GetComponentData<Point>(beam.point2);

                var delta = point2.position - point1.position; // TODO: precompute delta and length
                var dist = math.length(delta);
                var extraDist = dist - beam.length;
                var norm = delta / dist;
                var push = norm * extraDist * .5f;
                var breaks = Mathf.Abs(extraDist) > breakResistance;
                var writeBackPoint1 = false;
                var writeBackPoint2 = false;
                var writeBackBeam = false;

                if (!point1.anchor && !point2.anchor)
                {
                    point1.position += push;
                    point2.position -= push;
                    writeBackPoint1 = writeBackPoint2 = true;
                }
                else if (point1.anchor)
                {
                    point2.position -= push * 2f;
                    writeBackPoint2 = true;
                }
                else if (point2.anchor)
                {
                    point1.position += push * 2f;
                    writeBackPoint1 = true;
                }

                var matrix = matrices[beam.m1i][beam.m2i];
                var translate = (point1.position + point2.position) * .5f;
                if (norm.x * beam.norm.x + norm.y * beam.norm.y + norm.z * beam.norm.z < .99f)
                {
                    // bar has rotated: expensive full-matrix computation
                    matrix = Matrix4x4.TRS(translate, Quaternion.LookRotation(delta), new Vector3(beam.thickness, beam.thickness, dist));
                    beam.norm = norm;
                }
                else
                {
                    // bar hasn't rotated: only update the position elements
                    matrix.m03 = translate.x;
                    matrix.m13 = translate.y;
                    matrix.m23 = translate.z;
                }

                matrices[beam.m1i][beam.m2i] = matrix;

                if (breaks)
                {
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;
                        writeBackPoint2 = true;

                        var newPoint = em.CreateEntity();
                        em.AddComponent<Point>(newPoint);
                        em.SetComponentData(newPoint, new Point(point2.position.x, point2.position.y, point2.position.z, false) { neighborCount = 1} );

                        beam.point2 = newPoint;
                        writeBackBeam = true;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;
                        writeBackPoint1 = true;

                        var newPoint = em.CreateEntity();
                        em.AddComponent<Point>(newPoint);
                        em.SetComponentData(newPoint, new Point(point1.position.x, point1.position.y, point1.position.z, false) { neighborCount = 1 });

                        beam.point1 = newPoint;
                        writeBackBeam = true;
                    }
                }

                if (writeBackPoint1)
                    em.SetComponentData(beam.point1, point1);
                if (writeBackPoint2)
                    em.SetComponentData(beam.point2, point2);
                if (writeBackBeam)
                    em.SetComponentData(beams[i], beam);
            }
        }
    }
}
