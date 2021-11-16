using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class PointManager : MonoBehaviour, IConvertGameObjectToEntity
{
    public Mesh barMesh;
    public Material barMaterial;

    Tornado[] tornados;
    MeshRenderer ground;

    public int buildingCount = 35;
    public int groundDetailCount = 600;

    public float breakResistance;
    [Range(0f, 1f)] public float damping;
    [Range(0f, 1f)] public float friction;

    Bars bars;
    Points points;    

    Matrix4x4[][] matrices;
    MaterialPropertyBlock[] matProps;

    const int instancesPerBatch = 1023;

    internal void Awake()
    {
        Time.timeScale = 0f;
    }

    internal void Start()
    {
        ground = FindObjectOfType<Ground>().gameObject.GetComponent<MeshRenderer>();
        tornados = Resources.FindObjectsOfTypeAll<Tornado>();
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
        var beams = new List<Beam>();
        var matricesList = new List<List<Matrix4x4>> { new List<Matrix4x4>() };
        for (int i = 0; i < pointsList.Count; i++)
        {
            for (int j = i + 1; j < pointsList.Count; j++)
            {
                var a = pointsList[i];
                var b = pointsList[j];
                var delta = (b.position - a.position);
                var length = delta.magnitude;
                if (length >= 5f || length <= .2f)
                    continue;

                ++a.neighborCount;
                ++b.neighborCount;

                var beam = new Beam(i, j);
                beams.Add(beam);
            }
        }
        
        points = new Points(beams.Count * 2);
        for (int i = 0; i < pointsList.Count; ++i)
        {
            if (pointsList[i].neighborCount > 0)
                points.Add(pointsList[i]);
        }
        
        Debug.Log($"{points.count} ({pointsList.Count}) points, room for {points.capacity} ({beams.Count} bars)");

        bars = new Bars(beams);
        matProps = new MaterialPropertyBlock[beams.Count];
        var colors = new Vector4[instancesPerBatch];
        for (int i = 0; i < beams.Count; i++)
        {
            var delta = points.pos[beams[i].point2] - points.pos[beams[i].point1];

            bars.length[i] = delta.magnitude;
            var pos = (points.pos[beams[i].point1] + points.pos[beams[i].point2]) * .5f;
            var rot = Quaternion.LookRotation(delta);
            var scale = new Vector3(beams[i].thickness, beams[i].thickness, bars.length[i]);
            bars.matrix[i] = Matrix4x4.TRS(pos, rot, scale);

            matricesList[batch].Add(bars.matrix[i]);
            if (matricesList[batch].Count == instancesPerBatch)
            {
                batch++;
                matricesList.Add(new List<Matrix4x4>());
            }

            float upDot = Mathf.Acos(Mathf.Abs(Vector3.Dot(Vector3.up, delta.normalized))) / Mathf.PI;
            var color = Color.white * upDot * Random.Range(.7f, 1f);

            colors[i % instancesPerBatch] = color;
            if ((i + 1) % instancesPerBatch == 0 || i == beams.Count - 1)
            {
                var block = new MaterialPropertyBlock();
                block.SetVectorArray("_Color", colors);
                matProps[i / instancesPerBatch] = block;
            }
        }

        matrices = new Matrix4x4[matricesList.Count][];
        for (int i = 0; i < matrices.Length; i++)
            matrices[i] = matricesList[i].ToArray();

        Time.timeScale = 1f;
    }

    internal void FixedUpdate()
    {
        foreach (var tornado in tornados)
        {
            if (!tornado.simulate)
                continue;

            float invDamping = 1f - damping;
            for (int i = 0; i < points.count; i++)
            {
                if (points.anchor[i])
                    continue;

                var start = points.pos[i];
                points.old[i].y += .01f;

                // tornado force
                float tdx = tornado.x + Tornado.Sway(points.pos[i].y) - points.pos[i].x;
                float tdz = tornado.y - points.pos[i].z;
                float tornadoDist = Mathf.Sqrt(tdx * tdx + tdz * tdz);
                tdx /= tornadoDist;
                tdz /= tornadoDist;
                if (tornadoDist < tornado.maxForceDist)
                {
                    float force = (1f - tornadoDist / tornado.maxForceDist);
                    float yFader = Mathf.Clamp01(1f - points.pos[i].y / tornado.height);
                    force *= tornado.fader * tornado.force * Random.Range(-.3f, 1.3f);
                    float forceY = tornado.upForce;
                    points.old[i].y -= forceY * force;
                    float forceX = -tdz + tdx * tornado.inwardForce * yFader;
                    float forceZ = tdx + tdz * tornado.inwardForce * yFader;
                    points.old[i].x -= forceX * force;
                    points.old[i].z -= forceZ * force;
                }

                points.pos[i] += (points.pos[i] - points.old[i]) * invDamping;
				points.old[i] = start;

                if (points.pos[i].y < 0f)
                {
                    points.pos[i].y = 0f;
                    points.old[i].y = -points.old[i].y;
                    points.old[i].x += (points.pos[i].x - points.old[i].x) * friction;
                    points.old[i].z += (points.pos[i].z - points.old[i].z) * friction;
                }
            }

            for (int i = 0; i < bars.count; i++)
            {
                var beam = bars.beams[i];
                var delta = points.pos[beam.point2] - points.pos[beam.point1]; // TODO: precompute delta and length
                var dist = delta.magnitude;
                var extraDist = dist - bars.length[i];
                var push = (delta / dist * extraDist) * .5f;

                if (!points.anchor[beam.point1] && !points.anchor[beam.point2])
                {
                    points.pos[beam.point1] += push;
                    points.pos[beam.point2] -= push;
                }
                else if (points.anchor[beam.point1])
                {
                    points.pos[beam.point2] -= push * 2f;
                }
                else if (points.anchor[beam.point2])
                {
                    points.pos[beam.point1] += push * 2f;
                }

                var translate = (points.pos[beam.point1] + points.pos[beam.point2]) * .5f;
                if (delta.x / dist * bars.delta[i].x + delta.y / dist * bars.delta[i].y + delta.z / dist * bars.delta[i].z < .99f)
                {
                    // bar has rotated: expensive full-matrix computation
                    bars.matrix[i] = Matrix4x4.TRS(translate, Quaternion.LookRotation(delta), new Vector3(beam.thickness, beam.thickness, bars.length[i]));
                    bars.delta[i] = delta / dist;
                }
                else
                {
                    // bar hasn't rotated: only update the position elements
                    var matrix = bars.matrix[i];
                    matrix.m03 = translate.x;
                    matrix.m13 = translate.y;
                    matrix.m23 = translate.z;
                    bars.matrix[i] = matrix;
                }

                if (Mathf.Abs(extraDist) > breakResistance)
                {
                    if (points.neighbors[beam.point2] > 1)
                    {
                        points.neighbors[beam.point2]--;
                        var newIndex = points.count++;
                        points.old[newIndex] = points.pos[newIndex] = points.pos[beam.point2];
                        points.anchor[newIndex] = points.anchor[beam.point2];
                        points.neighbors[newIndex] = 1;

                        bars.beams[i] = new Beam(beam.point1, newIndex);
                    }
                    else if (points.neighbors[beam.point1] > 1)
                    {
                        points.neighbors[beam.point1]--;

                        var newIndex = points.count++;
                        points.old[newIndex] = points.pos[newIndex] = points.pos[beam.point1];
                        points.anchor[newIndex] = points.anchor[beam.point1];
                        points.neighbors[newIndex] = 1;

                        bars.beams[i] = new Beam(newIndex, beam.point2);
                    }
                }

                matrices[i / instancesPerBatch][i % instancesPerBatch] = bars.matrix[i];
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
