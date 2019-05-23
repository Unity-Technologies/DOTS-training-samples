using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class ClothSim : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;

    NativeArray<Vector3> vertices;
    NativeArray<Vector3> oldVertices;
    NativeArray<int> pins;
    NativeArray<Vector2Int> bars;
    NativeArray<float> barLengths;
    Vector3[] editedVertices;

    BarJob barJob;
    JobHandle barJobHandle;

    UpdateMeshJob meshJob;
    JobHandle meshJobHandle;

    static int barCount = 0;

    bool firstFrame = true;

    struct BarJob : IJob
    {
        public NativeArray<Vector3> vertices;
        [ReadOnly] public NativeArray<Vector2Int> bars;
        [ReadOnly] public NativeArray<float> barLengths;
        [ReadOnly] public NativeArray<int> pins;

        public void Execute()
        {
            for (int i = 0; i < bars.Length; i++)
            {
                Vector2Int pair = bars[i];

                Vector3 p1 = vertices[pair.x];
                Vector3 p2 = vertices[pair.y];
                int pin1 = pins[pair.x];
                int pin2 = pins[pair.y];

                float length = (p2 - p1).magnitude;
                float extra = (length - barLengths[i]) * .5f;
                Vector3 dir = (p2 - p1).normalized;

                if (pin1 == 0 && pin2 == 0)
                {
                    vertices[pair.x] += extra * dir;
                    vertices[pair.y] -= extra * dir;
                }
                else if (pin1 == 0 && pin2 == 1)
                {
                    vertices[pair.x] += extra * dir * 2f;
                }
                else if (pin1 == 1 && pin2 == 0)
                {
                    vertices[pair.y] -= extra * dir * 2f;
                }
            }
        }
    }

    struct UpdateMeshJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        public NativeArray<Vector3> oldVertices;
        [ReadOnly] public NativeArray<int> pins;
        public Vector3 gravity;
        public Matrix4x4 localToWorld;
        public Matrix4x4 worldToLocal;

        public void Execute(int i)
        {
            if (pins[i] == 0)
            {
                Vector3 oldVert = oldVertices[i];
                Vector3 vert = vertices[i];

                Vector3 startPos = vert;
                oldVert -= gravity;
                vert += (vert - oldVert);
                Vector3 worldPos = localToWorld.MultiplyPoint3x4(vert);
                oldVert = startPos;

                if (worldPos.y < 0f)
                {
                    Vector3 oldWorldPos = localToWorld.MultiplyPoint3x4(oldVert);
                    oldWorldPos.y = (worldPos.y - oldWorldPos.y) * .5f;
                    worldPos.y = 0f;
                    vert = worldToLocal.MultiplyPoint3x4(worldPos);
                    oldVert = worldToLocal.MultiplyPoint3x4(oldWorldPos);
                }

                vertices[i] = vert;
                oldVertices[i] = oldVert;
            }
        }
    }

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        mesh.MarkDynamic();

        vertices = new NativeArray<Vector3>(mesh.vertices, Allocator.Persistent);
        pins = new NativeArray<int>(vertices.Length, Allocator.Persistent);
        oldVertices = new NativeArray<Vector3>(vertices, Allocator.Persistent);
        editedVertices = new Vector3[vertices.Length];

        Vector3[] normals = mesh.normals;

        for (int i = 0; i < pins.Length; i++)
        {
            if (normals[i].y > .9f && vertices[i].y > .3f)
            {
                pins[i] = 1;
            }
        }

        HashSet<Vector2Int> barLookup = new HashSet<Vector2Int>();
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2Int pair = new Vector2Int();
                pair.x = triangles[i + j];
                pair.y = triangles[i + (j + 1) % 3];
                if (pair.x > pair.y)
                {
                    int temp = pair.x;
                    pair.x = pair.y;
                    pair.y = temp;
                }

                if (barLookup.Contains(pair) == false)
                {
                    barLookup.Add(pair);
                }
            }
        }

        List<Vector2Int> barList = new List<Vector2Int>(barLookup);
        barCount += barList.Count;

        bars = new NativeArray<Vector2Int>(barList.ToArray(), Allocator.Persistent);
        barLengths = new NativeArray<float>(barList.Count, Allocator.Persistent);

        for (int i = 0; i < barList.Count; i++)
        {
            Vector2Int pair = barList[i];
            Vector3 p1 = vertices[pair.x];
            Vector3 p2 = vertices[pair.y];
            barLengths[i] = (p2 - p1).magnitude;
        }
    }

    void Update()
    {
        if (firstFrame)
        {
            firstFrame = false;
        }
        else
        {
            meshJobHandle.Complete();

            meshJob.vertices.CopyTo(editedVertices);
            mesh.vertices = editedVertices;
        }

        barJob = new BarJob
        {
            vertices = vertices,
            pins = pins,
            bars = bars,
            barLengths = barLengths
        };

        meshJob = new UpdateMeshJob
        {
            vertices = vertices,
            oldVertices = oldVertices,
            pins = pins,
            gravity = transform.InverseTransformVector(-Vector3.up * Time.deltaTime * Time.deltaTime),
            localToWorld = transform.localToWorldMatrix,
            worldToLocal = transform.worldToLocalMatrix
        };

        barJobHandle = barJob.Schedule();
        meshJobHandle = meshJob.Schedule(vertices.Length, 128, barJobHandle);
    }

    private void LateUpdate()
    {
    }

    void OnDestroy()
    {
        meshJobHandle.Complete();
        vertices.Dispose();
        pins.Dispose();
        bars.Dispose();
        barLengths.Dispose();
        oldVertices.Dispose();
    }
}