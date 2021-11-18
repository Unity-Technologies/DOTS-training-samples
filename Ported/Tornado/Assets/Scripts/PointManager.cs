#define DEBUG_DOTS
#pragma warning disable 0649
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

static class PointTypes
{
    public static readonly ComponentType[] FixedPointTypes = new ComponentType[] { typeof(Point), typeof(PointDamping), typeof(AnchoredPoint) };
    public static readonly ComponentType[] DynamicPointTypes = new ComponentType[] { typeof(Point), typeof(PointDamping), typeof(DynamicPoint), typeof(AffectedPoint) };

    public static readonly EntityArchetype FixedPointArchType;
    public static readonly EntityArchetype DynamicPointArchType;

    static PointTypes()
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        FixedPointArchType = em.CreateArchetype(FixedPointTypes);
        DynamicPointArchType = em.CreateArchetype(DynamicPointTypes);
    }
}

class PointManager : MonoBehaviour
{
    const float spacing = 2f;
    const int instancesPerBatch = 1023;

    public Mesh barMesh = null;
    public Material barMaterial = null;

    [Range(0, 999)] public int buildingCount = 115;
    [Range(0, 10000)] public int groundDetailCount = 250;
    [Range(0f, 1f)] public float breakResistance = 0.55f;
    [Range(0f, 1f)] public float damping = 0.027f;
    [Range(0f, 1f)] public float friction = 0.369f;

    Tornado[] tornados;
    MeshRenderer ground;
    internal Matrix4x4[][] matrices;
    MaterialPropertyBlock[] matProps;
    float3 groundExtents;
    int maxTornadoHeight;


    internal void Awake()
    {
        Time.timeScale = 0f;

        ground = GetComponent<MeshRenderer>();
        tornados = Resources.FindObjectsOfTypeAll<Tornado>();

        groundExtents = ground.bounds.extents;
        maxTornadoHeight = Mathf.FloorToInt(tornados.Max(t => t.height) * .9f / spacing);
    }

    internal void Start()
    {
        #if DEBUG_DOTS
        using (new DebugTimer("Generate buildings", 500d))
        #endif
        { 
            Generate();
            Time.timeScale = 1f;
        }
    }

    internal void Update()
    {
        #if DEBUG_DOTS
        using (var t = new DebugTimer($"Rendering {matrices.Length} beam batches", 3d))
        #endif
        {
            for (int i = 0; i < matrices.Length; ++i)
                Graphics.DrawMeshInstanced(barMesh, 0, barMaterial, matrices[i], matrices[i].Length, matProps[i]);
        }
    }

    internal Entity CreatePoint(EntityManager em, in float3 pos, in bool anchored, in int neighborCount = 0)
    {
        var point = em.CreateEntity(anchored ? PointTypes.FixedPointArchType : PointTypes.DynamicPointArchType);
        em.SetComponentData(point, new Point(pos) { neighborCount = neighborCount });
        em.SetComponentData(point, new PointDamping(1f - damping, friction));
        return point;
    }

    void Generate()
    {
        var random = new Unity.Mathematics.Random(1234);
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Create buildings
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

                CreatePoint(em, new float3(pos.x + spacing, jspace, pos.z - spacing), anchored);
                CreatePoint(em, new float3(pos.x - spacing, jspace, pos.z - spacing), anchored);
                CreatePoint(em, new float3(pos.x + 0f, jspace, pos.z + spacing), anchored);
            }
        }

        // Create ground details
        for (int i = 0; i < groundDetailCount; i++)
        {
            var pos = random.NextFloat3(-groundLimits, groundLimits);

            CreatePoint(em, new float3(pos.x + random.NextFloat(-.2f, -.1f), pos.y + random.NextFloat(0f, 3f), pos.z + random.NextFloat(.1f, .2f)), false);
            CreatePoint(em, new float3(pos.x + random.NextFloat(.2f, .1f), pos.y + random.NextFloat(0f, .2f), pos.z + random.NextFloat(-.1f, -.2f)), random.NextFloat() < .1f);
        }

        // Create beams
        int beamCount = 0;
        int matrixBatchIndex = 0;
        var matricesList = new List<List<Matrix4x4>> { new List<Matrix4x4>() };
        using (var pointQuery = em.CreateEntityQuery(typeof(Point)))
        using (var points = pointQuery.ToEntityArray(Allocator.TempJob))
        {
            for (int i = 0; i < points.Length; i++)
            {
                for (int j = i + 1; j < points.Length; j++)
                {
                    var a = em.GetComponentData<Point>(points[i]);
                    var b = em.GetComponentData<Point>(points[j]);
                    var delta = (b.pos - a.pos);
                    var length = math.length(delta);
                    if (length >= 5f || length <= .2f)
                        continue;

                    ++a.neighborCount;
                    ++b.neighborCount;

                    em.SetComponentData(points[i], a);
                    em.SetComponentData(points[j], b);

                    var beam = new Beam(points[i], points[j], random.NextFloat(.25f, .35f), length, matrixBatchIndex, matricesList[matrixBatchIndex].Count);
                    var barLength = math.length(delta);
                    var norm = delta / barLength;

                    var pos = (a.pos + b.pos) * .5f;
                    var rot = Quaternion.LookRotation(delta);
                    var scale = new Vector3(beam.thickness, beam.thickness, barLength);
                    var beamMatrix = Matrix4x4.TRS(pos, rot, scale);

                    matricesList[matrixBatchIndex].Add(beamMatrix);
                    if (matricesList[matrixBatchIndex].Count == instancesPerBatch)
                    {
                        matrixBatchIndex++;
                        matricesList.Add(new List<Matrix4x4>());
                    }

                    var beamEntity = em.CreateEntity(typeof(Beam));
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
                var delta = point2.pos - point1.pos;

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

        #if DEBUG_DOTS
        Debug.Log($"Generated {buildingCount} buildings and {groundDetailCount} " +
            $"ground details for a total {pointCount} points and {beamCount} beams");
        #endif
    }
}
