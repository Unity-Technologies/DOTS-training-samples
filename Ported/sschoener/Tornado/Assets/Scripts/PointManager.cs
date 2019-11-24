using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
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

    NativeArray<Point> m_Points;
    NativeArray<Bar> m_Bars;
    int pointCount
    {
        get => m_PointCountArray[0];
        set => m_PointCountArray[0] = value;
    }
    NativeArray<int> m_PointCountArray;

    public static float tornadoX;
    public static float tornadoZ;

    float m_TornadoFader;

    Matrix4x4[][] m_Matrices;
    MaterialPropertyBlock[] m_MatProps;
    List<GCHandle> m_GcHandles = new List<GCHandle>();

    Transform m_Cam;
    static readonly int k_Color = Shader.PropertyToID("_Color");

    const int k_InstancesPerBatch = 1023;

    void Awake()
    {
        Random.InitState(0);
        m_PointCountArray = new NativeArray<int>(1, Allocator.Persistent);
    }

    void Start()
    {
        Generate();
        m_Cam = Camera.main.transform;
    }

    void OnDestroy()
    {
        m_Bars.Dispose();
        m_Points.Dispose();
        m_PointCountArray.Dispose();
    }

    public static float TornadoSway(float y, float time) => math.sin(y / 5f + time / 4f) * 3f;

    struct BarWithGraphics
    {
        public Bar bar;
        public Matrix4x4 matrix;
        public Color color;
    }

    [BurstCompile]
    struct CollectBarsJob : IJobParallelFor
    {
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<float3> Positions;
        public NativeQueue<BarWithGraphics>.ParallelWriter Output;
        public uint Seed;

        public void Execute(int index)
        {
            var rng = new Unity.Mathematics.Random(Seed * (1 + (uint)index) * 100153);
            var pos1 = Positions[index];
            for (int j = index + 1; j < Positions.Length; j++)
            {
                var pos2 = Positions[j];
                if (pos2.y >= pos1.y + 5f)
                    break;
                var delta = pos1 - pos2;
                var l = math.length(delta);
                if (l < 5f && l > .2f)
                {
                    var bar = new BarWithGraphics()
                    {
                        bar = new Bar
                        {
                            point1 = index,
                            point2 = j,
                            length = l,
                            thickness = rng.NextFloat(.25f, .35f)
                        }
                    };

                    {
                        var pos = (pos1 + pos2) / 2;
                        var rot = quaternion.LookRotation(delta, new float3(0, 1, 0));
                        var scale = new float3(bar.bar.thickness, bar.bar.thickness, bar.bar.length);
                        bar.matrix = float4x4.TRS(pos, rot, scale);
                    }
                    {
                        var proj = math.dot(new float3(0, 1, 0), delta / l);
                        float upDot = math.acos(math.abs(proj)) / math.PI;
                        bar.color = Color.white * upDot * rng.NextFloat(.7f, 1f);
                    }

                    Output.Enqueue(bar);
                }
            }
        }
    }

    void Generate()
    {
        List<Point> pointsList = new List<Point>();

        using (new ProfilerMarker("CreateBuildings").Auto())
        {
            // buildings
            for (int i = 0; i < 35; i++)
            {
                int height = Random.Range(4, 12);
                float3 pos = new float3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));
                float spacing = 2f;
                for (int j = 0; j < height; j++)
                {
                    {
                        var point = new Point
                        {
                            pos = new float3(pos.x + spacing, j * spacing, pos.z - spacing),
                            anchor = j == 0
                        };
                        point.old = point.pos;
                        pointsList.Add(point);
                    }
                    {
                        var point = new Point
                        {
                            pos = new float3(pos.x - spacing, j * spacing, pos.z - spacing),
                            anchor = j == 0
                        };
                        point.old = point.pos;
                        pointsList.Add(point);
                    }
                    {
                        var point = new Point
                        {
                            pos = new float3(pos.x, j * spacing, pos.z + spacing),
                            anchor = j == 0
                        };
                        point.old = point.pos;
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
                    var point = new Point
                    {
                        pos = pos + new float3(
                            Random.Range(-.2f, -.1f),
                            Random.Range(0f, 3f),
                            Random.Range(.1f, .2f)
                        )
                    };
                    point.old = point.pos;
                    pointsList.Add(point);
                }
                {
                    var point = new Point
                    {
                        pos = pos + new float3(
                            Random.Range(.1f, .2f),
                            Random.Range(0, .2f),
                            Random.Range(-.1f, -.2f)
                        ),
                        anchor = Random.value < .1f
                    };
                    point.old = point.pos;

                    pointsList.Add(point);
                }
            }
        }

        using (new ProfilerMarker("CreateBars").Auto())
        {
            pointsList.Sort((p1, p2) => p1.pos.y.CompareTo(p2.pos.y));
            var bars = new NativeQueue<BarWithGraphics>(Allocator.TempJob);
            var positions = new NativeArray<float3>(pointsList.Count, Allocator.TempJob);
            for (int i = 0; i < pointsList.Count; i++)
                positions[i] = pointsList[i].pos;

            new CollectBarsJob
            {
                Output = bars.AsParallelWriter(),
                Positions = positions,
                Seed = (uint)Random.Range(1, 1000000)
            }.Schedule(positions.Length, 1).Complete();

            int numBatches = bars.Count / k_InstancesPerBatch;
            int remaining = bars.Count % k_InstancesPerBatch;
            numBatches += remaining > 0 ? 1 : 0;

            m_Matrices = new Matrix4x4[numBatches][];
            int lastBatchSize = remaining == 0 ? k_InstancesPerBatch : remaining;
            for (int i = 0; i < numBatches; i++)
            {
                int batchSize = i == numBatches - 1 ? lastBatchSize : k_InstancesPerBatch;
                m_Matrices[i] = new Matrix4x4[batchSize];
            }

            using (new ProfilerMarker("SetNeighbors").Auto())
            {
                m_MatProps = new MaterialPropertyBlock[numBatches];
                for (int i = 0; i < numBatches; i++)
                    m_MatProps[i] = new MaterialPropertyBlock();

                Vector4[] colors = new Vector4[k_InstancesPerBatch];
                m_Bars = new NativeArray<Bar>(bars.Count, Allocator.Persistent);
                for (int i = 0; i < m_Bars.Length; i++)
                {
                    var bwm = bars.Dequeue();
                    m_Bars[i] = bwm.bar;
                    {
                        var p1 = pointsList[m_Bars[i].point1];
                        p1.neighborCount++;
                        pointsList[m_Bars[i].point1] = p1;
                        var p2 = pointsList[m_Bars[i].point2];
                        p2.neighborCount++;
                        pointsList[m_Bars[i].point2] = p2;
                    }
                    int batch = i / k_InstancesPerBatch;
                    int index = i % k_InstancesPerBatch;
                    m_Matrices[batch][index] = bwm.matrix;
                    colors[index] = bwm.color;
                    if (index == colors.Length - 1 || i == m_Bars.Length - 1)
                        m_MatProps[batch].SetVectorArray(k_Color, colors);
                }
            }

            bars.Dispose();
        }

        using (new ProfilerMarker("RemapPoints").Auto())
        {
            var pointRemap = new int[pointsList.Count];
            m_Points = new NativeArray<Point>(m_Bars.Length * 2, Allocator.Persistent);
            pointCount = 0;

            for (int i = 0; i < pointsList.Count; i++)
            {
                if (pointsList[i].neighborCount > 0)
                {
                    m_Points[pointCount] = pointsList[i];
                    pointRemap[i] = pointCount;
                    pointCount++;
                }
            }

            for (int i = 0; i < m_Bars.Length; i++)
            {
                var bar = m_Bars[i];
                bar.point1 = pointRemap[bar.point1];
                bar.point2 = pointRemap[bar.point2];
                m_Bars[i] = bar;
            }
        }

        Debug.Log(pointCount + " points, room for " + m_Points.Length + " (" + m_Bars.Length + " bars)");
    }

    [BurstCompile]
    struct UpdatePointsJob : IJobParallelFor
    {
        public NativeArray<Point> Points;
        public float TornadoMaxForceDist;
        public float TornadoHeight;
        public float TornadoFader;
        public float TornadoForce;
        public float TornadoInwardForce;
        public float TornadoUpForce;
        public float Friction;
        public float InvDamping;
        public float Time;
        public float TornadoX;
        public float TornadoZ;
        public uint Seed;

        public void Execute(int i)
        {
            var rng = new Unity.Mathematics.Random((1 + (uint)i) * Seed * 20479);
            var point = Points[i];
            if (point.anchor == false)
            {
                float3 start = point.pos;

                point.old.y += .01f;

                // tornado force
                float tdx = TornadoX + TornadoSway(point.pos.y, Time) - point.pos.x;
                float tdz = TornadoZ - point.pos.z;
                float tornadoDist = math.sqrt(tdx * tdx + tdz * tdz);
                tdx /= tornadoDist;
                tdz /= tornadoDist;
                if (tornadoDist < TornadoMaxForceDist)
                {
                    float force = (1f - tornadoDist / TornadoMaxForceDist);
                    float yFader = math.clamp(1f - point.pos.y / TornadoHeight, 0, 1);
                    force *= TornadoFader * TornadoForce * rng.NextFloat(-.3f, 1.3f);
                    float forceY = TornadoUpForce;
                    float forceX = -tdz + tdx * TornadoInwardForce * yFader;
                    float forceZ = tdx + tdz * TornadoInwardForce * yFader;
                    point.old -= new float3(forceX, forceY, forceZ) * force;
                }

                point.pos += (point.pos - point.old) * InvDamping;

                point.old = start;
                if (point.pos.y < 0f)
                {
                    point.pos.y = 0f;
                    point.old.y = -point.old.y;
                    point.old.x += (point.pos.x - point.old.x) * Friction;
                    point.old.z += (point.pos.z - point.old.z) * Friction;
                }

                Points[i] = point;
            }
        }
    }

    [BurstCompile]
    unsafe struct ComputeMatrixJob : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction]
        public Matrix4x4** m_Matrices;

        [ReadOnly]
        public NativeArray<float3> Delta;
        [ReadOnly]
        public NativeArray<Point> Points;
        public NativeArray<Bar> Bars;
        
        
        public void Execute(int i)
        {
            var bar = Bars[i];
            if (math.dot(Delta[i], bar.oldDelta) < .99f)
            {
                ref var matrix = ref m_Matrices[i / k_InstancesPerBatch][i % k_InstancesPerBatch];

                // bar has rotated: expensive full-matrix computation
                matrix = Matrix4x4.TRS(
                    (Points[bar.point1].pos + Points[bar.point2].pos) / 2,
                    Quaternion.LookRotation(Delta[i]),
                    new Vector3(bar.thickness, bar.thickness, bar.length));
                bar.oldDelta = Delta[i];
            }
            else
            {
                // bar hasn't rotated: only update the position elements
                var p = (Points[bar.point1].pos + Points[bar.point2].pos) / 2;
                ref var matrix = ref m_Matrices[i / k_InstancesPerBatch][i % k_InstancesPerBatch];
                matrix.m03 = p.x;
                matrix.m13 = p.y;
                matrix.m23 = p.z;
            
            }
        }
    }

    [BurstCompile]
    struct ComputePushJob : IJob
    {
        [ReadOnly]
        public NativeArray<Bar> Bars;
        public NativeArray<Point> Points;
        [WriteOnly]
        public NativeArray<float> ExtraDist;
        [WriteOnly]
        public NativeArray<float3> Delta;
        public void Execute()
        {
            for (int i = 0; i < Bars.Length; i++)
            {
                var bar = Bars[i];
                var point1 = Points[bar.point1];
                var point2 = Points[bar.point2];

                var d = point2.pos - point1.pos;
                float dist = math.length(d);
                d = Delta[i] = d / dist;
                float extra = ExtraDist[i] = dist - bar.length;

                {
                    var push = d * extra * .5f;
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
                }
                Points[bar.point1] = point1;
                Points[bar.point2] = point2;
            }
        }
    }

    [BurstCompile]
    struct DoBreakJob : IJob
    {
        public NativeArray<Bar> Bars;
        public float BreakResistance;
        public NativeArray<Point> Points;
        public NativeArray<float> ExtraDist;
        public NativeArray<int> PointCount;
        
        public void Execute()
        {
            for (int i = 0; i < Bars.Length; i++)
            {
                if (math.abs(ExtraDist[i]) > BreakResistance)
                {
                    var bar = Bars[i];
                    int origPoint1 = bar.point1;
                    int origPoint2 = bar.point2;
                    var point1 = Points[origPoint1];
                    var point2 = Points[origPoint2];
                    if (point2.neighborCount > 1)
                    {
                        point2.neighborCount--;
                        Point newPoint = point2;
                        newPoint.neighborCount = 1;
                        Points[PointCount[0]] = newPoint;
                        bar.point2 = PointCount[0];
                        PointCount[0] += 1;

                        Points[origPoint2] = point2;
                        Bars[i] = bar;
                    }
                    else if (point1.neighborCount > 1)
                    {
                        point1.neighborCount--;
                        Point newPoint = point1;
                        newPoint.neighborCount = 1;
                        Points[PointCount[0]] = newPoint;
                        bar.point1 = PointCount[0];
                        PointCount[0] += 1;
                        
                        Points[origPoint1] = point1;
                        Bars[i] = bar;
                    }
                }
            }
        }
    }

    unsafe void FixedUpdate()
    {
        m_TornadoFader = Mathf.Clamp01(m_TornadoFader + Time.deltaTime / 10f);

        float invDamping = 1f - damping;

        var updatePointsJob = new UpdatePointsJob
        {
            TornadoFader = m_TornadoFader,
            InvDamping = invDamping,
            TornadoForce = tornadoForce,
            TornadoHeight = tornadoHeight,
            TornadoInwardForce = tornadoInwardForce,
            TornadoUpForce = tornadoUpForce,
            TornadoMaxForceDist = tornadoMaxForceDist,
            Friction = friction,
            Points = m_Points,
            TornadoX = tornadoX,
            TornadoZ = tornadoZ,
            Time = Time.fixedTime,
            Seed = (uint)Random.Range(1, 1000000)
        }.Schedule(pointCount, 16);

        NativeArray<float> extraDist = new NativeArray<float>(m_Bars.Length, Allocator.TempJob);
        NativeArray<float3> delta = new NativeArray<float3>(m_Bars.Length, Allocator.TempJob);

        var pushJob = new ComputePushJob
        {
            Bars = m_Bars,
            Delta = delta,
            ExtraDist = extraDist,
            Points = m_Points
        }.Schedule(updatePointsJob);

        var matrices = new Matrix4x4*[m_Matrices.Length];
        for (int i = 0; i < m_Matrices.Length; i++)
        {
            m_GcHandles.Add(GCHandle.Alloc(m_Matrices[i], GCHandleType.Pinned));
            matrices[i] = (Matrix4x4*)m_GcHandles[i].AddrOfPinnedObject().ToPointer();
        }

        var matrixHandle = GCHandle.Alloc(matrices, GCHandleType.Pinned);
        var matrixJob = new ComputeMatrixJob
        {
            Bars = m_Bars,
            Delta = delta,
            Points = m_Points,
            m_Matrices = (Matrix4x4**)matrixHandle.AddrOfPinnedObject().ToPointer()
        }.Schedule(m_Bars.Length, 8, pushJob);

        new DoBreakJob
        {
            BreakResistance = breakResistance,
            ExtraDist = extraDist,
            Bars = m_Bars,
            Points = m_Points,
            PointCount = m_PointCountArray
        }.Schedule(matrixJob).Complete();
        
        matrixHandle.Free();
        for (int i = 0; i < m_GcHandles.Count; i++)
            m_GcHandles[i].Free();
        m_GcHandles.Clear();
        
        delta.Dispose();
        extraDist.Dispose();
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
