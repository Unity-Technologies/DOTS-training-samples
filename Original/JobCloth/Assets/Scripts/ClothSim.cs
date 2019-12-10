using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Linq;
using Unity.Burst;

public class ClothSim : MonoBehaviour, IConvertGameObjectToEntity
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

	struct BarJob:IJob {
		public NativeArray<Vector3> vertices;
		[ReadOnly]
		public NativeArray<Vector2Int> bars;
		[ReadOnly]
		public NativeArray<float> barLengths;
		[ReadOnly]
		public NativeArray<int> pins;

		public void Execute() {

			for (int i = 0; i < bars.Length; i++) {
				Vector2Int pair = bars[i];

				Vector3 p1 = vertices[pair.x];
				Vector3 p2 = vertices[pair.y];
				int pin1 = pins[pair.x];
				int pin2 = pins[pair.y];

				float length = (p2 - p1).magnitude;
				float extra = (length - barLengths[i]) * .5f;
				Vector3 dir = (p2 - p1).normalized;

				if (pin1 == 0 && pin2 == 0) {
					vertices[pair.x] += extra * dir;
					vertices[pair.y] -= extra * dir;
				} else if (pin1 == 0 && pin2 == 1) {
					vertices[pair.x] += extra * dir * 2f;
				} else if (pin1 == 1 && pin2 == 0) {
					vertices[pair.y] -= extra * dir * 2f;
				}
			}
		}
	}

	struct UpdateMeshJob:IJobParallelFor {
		public NativeArray<Vector3> vertices;
		public NativeArray<Vector3> oldVertices;
		[ReadOnly]
		public NativeArray<int> pins;
		public Vector3 gravity;
		public Matrix4x4 localToWorld;
		public Matrix4x4 worldToLocal;

		public void Execute(int i) {
			if (pins[i] == 0) {

				Vector3 oldVert = oldVertices[i];
				Vector3 vert = vertices[i];

				Vector3 startPos = vert;
				oldVert -= gravity;
				vert += (vert - oldVert);
				Vector3 worldPos = localToWorld.MultiplyPoint3x4(vert);
				oldVert = startPos;

				if (worldPos.y < 0f) {
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

	void Start () {
		meshFilter = GetComponent<MeshFilter>();
		mesh = meshFilter.mesh;
		mesh.MarkDynamic();

		vertices = new NativeArray<Vector3>(mesh.vertices,Allocator.Persistent);
		pins = new NativeArray<int>(vertices.Length,Allocator.Persistent);
		oldVertices = new NativeArray<Vector3>(vertices,Allocator.Persistent);
		editedVertices = new Vector3[vertices.Length];

		Vector3[] normals = mesh.normals;

		for (int i=0;i<pins.Length;i++) {
			if (normals[i].y>.9f && vertices[i].y>.3f) {
				pins[i] = 1;
			}
		}

		HashSet<Vector2Int> barLookup = new HashSet<Vector2Int>();
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3) {
			for (int j=0;j<3;j++) {
				Vector2Int pair = new Vector2Int();
				pair.x = triangles[i + j];
				pair.y = triangles[i + (j + 1)%3];
				if (pair.x>pair.y) {
					int temp = pair.x;
					pair.x = pair.y;
					pair.y = temp;
				}
				if (barLookup.Contains(pair)==false) {
					barLookup.Add(pair);
				}
			}
		}
		List<Vector2Int> barList = new List<Vector2Int>(barLookup);
		barCount += barList.Count;

		bars = new NativeArray<Vector2Int>(barList.ToArray(),Allocator.Persistent);
		barLengths = new NativeArray<float>(barList.Count,Allocator.Persistent);

		for (int i=0;i<barList.Count;i++) {
			Vector2Int pair = barList[i];
			Vector3 p1 = vertices[pair.x];
			Vector3 p2 = vertices[pair.y];
			barLengths[i] = (p2 - p1).magnitude;
		}
	}
	
	void Update () {

		if (firstFrame) {
			firstFrame = false;
		} else {
			meshJobHandle.Complete();

			meshJob.vertices.CopyTo(editedVertices);
			mesh.vertices = editedVertices;
		}
		
		barJob = new BarJob {
			vertices = vertices,
			pins = pins,
			bars = bars,
			barLengths = barLengths
		};

		meshJob = new UpdateMeshJob {
			vertices = vertices,
			oldVertices = oldVertices,
			pins=pins,
			gravity = transform.InverseTransformVector(-Vector3.up * Time.deltaTime*Time.deltaTime),
			localToWorld = transform.localToWorldMatrix,
			worldToLocal = transform.worldToLocalMatrix
		};

		barJobHandle = barJob.Schedule();
		meshJobHandle = meshJob.Schedule(vertices.Length,128,barJobHandle);
	}

	private void LateUpdate() {
		
	}

	void OnDestroy() {
		meshJobHandle.Complete();
        if (vertices.IsCreated) vertices.Dispose();
        if (pins.IsCreated) pins.Dispose();
        if (bars.IsCreated) bars.Dispose();
        if (barLengths.IsCreated) barLengths.Dispose();
        if (oldVertices.IsCreated) oldVertices.Dispose();
	}

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var mesh        = GetComponent<MeshFilter>().sharedMesh;
        var material    = GetComponent<MeshRenderer>().sharedMaterial;
        var gravity     = (float3)transform.InverseTransformVector(-Vector3.up * Time.deltaTime * Time.deltaTime);

        
        var vertices    = mesh.vertices;
        var normals     = mesh.normals;
        var indices     = mesh.triangles;
        var newMesh     = new Mesh();
        newMesh.vertices = vertices;
        newMesh.normals = normals;
        newMesh.triangles = indices;

        dstManager.AddComponentData(entity, new ClothComponent
        {
            Mesh = newMesh,
            Gravity = gravity,
            Material = material
        }); ;
    }
}

public class ClothComponent : IComponentData
{
    public Mesh Mesh;
    public float3 Gravity;
    public Material Material;

    public NativeArray<float3>  CurrentClothPosition;
    public NativeArray<float3>  PreviousClothPosition;
    public NativeArray<float3>  ClothNormals;
    public NativeArray<int>     Pins;

    public NativeArray<int2>    Constraint1Indices;
    public NativeArray<float>   Constraint1Lengths;

    public NativeArray<int2>    Constraint2Indices;
    public NativeArray<float>   Constraint2Lengths;
}

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
class ClothComponentSystem : ComponentSystem
{

    override protected void OnStartRunning()
    {
        Entities.ForEach((ClothComponent cloth) =>
        {
            var vertices = cloth.Mesh.vertices;
            var normals = cloth.Mesh.normals;
            var indices = cloth.Mesh.triangles;

            cloth.CurrentClothPosition = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.PreviousClothPosition = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.ClothNormals = new NativeArray<float3>(vertices.Length, Allocator.Persistent);
            cloth.Pins = new NativeArray<int>(vertices.Length, Allocator.Persistent);
            var pinned = new bool[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                cloth.PreviousClothPosition[i] =
                cloth.CurrentClothPosition[i] = vertices[i];

                cloth.ClothNormals[i] = normals[i];

                if (normals[i].y > .9f && vertices[i].y > .3f)
                {
                    pinned[i] = true;
                    cloth.Pins[i] = 1;
                }
            }

            var constraint1Lookup = new HashSet<int2>();
            var constraint2Lookup = new HashSet<int2>(); 
            for (int i = 0; i < indices.Length; i += 3)
            {
                var index0 = indices[i + 0];
                var index1 = indices[i + 1];
                var index2 = indices[i + 2];

                int2 constraint0;
                int2 constraint1;
                int2 constraint2;

                if (index0 > index1) constraint0 = new int2(index0, index1); else constraint0 = new int2(index1, index0);
                if (index1 > index2) constraint1 = new int2(index1, index2); else constraint1 = new int2(index2, index1);
                if (index2 > index0) constraint2 = new int2(index2, index0); else constraint2 = new int2(index0, index2);

                if (cloth.Pins[index0] == 0 || cloth.Pins[index1] == 0)
                {
                    if (cloth.Pins[index0] == 1)
                        constraint1Lookup.Add(new int2(index0, index1));
                    else 
                    if (cloth.Pins[index1] == 1)
                        constraint1Lookup.Add(new int2(index1, index0));
                    else
                        constraint2Lookup.Add(constraint0);
                }
                if (cloth.Pins[index1] == 0 || cloth.Pins[index2] == 0)
                {
                    if (cloth.Pins[index1] == 1)
                        constraint1Lookup.Add(new int2(index2, index1));
                    else
                    if (cloth.Pins[index2] == 1)
                        constraint1Lookup.Add(new int2(index1, index2));
                    else
                        constraint2Lookup.Add(constraint1);
                }
                if (cloth.Pins[index0] == 0 || cloth.Pins[index2] == 0)
                {
                    if (cloth.Pins[index2] == 1)
                        constraint1Lookup.Add(new int2(index2, index0));
                    else
                    if (cloth.Pins[index0] == 1)
                        constraint1Lookup.Add(new int2(index0, index2));
                    else
                        constraint2Lookup.Add(constraint2);
                }
            }

            cloth.Constraint1Indices = new NativeArray<int2>(constraint1Lookup.ToArray(), Allocator.Persistent);
            cloth.Constraint2Indices = new NativeArray<int2>(constraint2Lookup.ToArray(), Allocator.Persistent);

            cloth.Constraint1Lengths = new NativeArray<float>(constraint1Lookup.Count, Allocator.Persistent);
            for (int i = 0; i < cloth.Constraint1Indices.Length; i++)
            {
                var constraint = cloth.Constraint1Indices[i];

                var vertex0 = cloth.PreviousClothPosition[constraint.x];
                var vertex1 = cloth.PreviousClothPosition[constraint.y];

                cloth.Constraint1Lengths[i] = math.length(vertex1 - vertex0);
            }

            cloth.Constraint2Lengths = new NativeArray<float>(constraint2Lookup.Count, Allocator.Persistent);
            for (int i = 0; i < cloth.Constraint2Indices.Length; i ++)
            {
                var constraint = cloth.Constraint2Indices[i];

                var vertex0 = cloth.PreviousClothPosition[constraint.x];
                var vertex1 = cloth.PreviousClothPosition[constraint.y];

                cloth.Constraint2Lengths[i] = math.length(vertex1 - vertex0);
            }
        });
    }

    override protected void OnDestroy()
    {
        Entities.ForEach((ClothComponent cloth) =>
        {
            if (cloth.CurrentClothPosition .IsCreated) cloth.CurrentClothPosition.Dispose();
            if (cloth.PreviousClothPosition.IsCreated) cloth.PreviousClothPosition.Dispose();
            if (cloth.ClothNormals         .IsCreated) cloth.ClothNormals.Dispose();
            if (cloth.Pins                 .IsCreated) cloth.Pins.Dispose();
            
            if (cloth.Constraint1Indices    .IsCreated) cloth.Constraint1Indices.Dispose();
            if (cloth.Constraint1Lengths    .IsCreated) cloth.Constraint1Lengths.Dispose();

            if (cloth.Constraint2Indices    .IsCreated) cloth.Constraint2Indices.Dispose();
            if (cloth.Constraint2Lengths    .IsCreated) cloth.Constraint2Lengths.Dispose();
        });
    }

    [BurstCompile]
    struct Constraint1Job : IJob
    {
        public NativeArray<float3> vertices;
        [ReadOnly] public NativeArray<int2> constraintIndices;
        [ReadOnly] public NativeArray<float> constraintLengths;
        [ReadOnly] public NativeArray<int> pins;

        public void Execute()
        {
            for (int i = 0; i < constraintIndices.Length; i++)
            {
                int2 pair = constraintIndices[i];
                int pin1 = pins[pair.x];
                int pin2 = pins[pair.y];

                float3 p1 = vertices[pair.x];
                float3 p2 = vertices[pair.y];

                var delta = p2 - p1;
                var length = math.length(delta);
                var extra = (length - constraintLengths[i]) * .5f;
                var dir = delta / length;

                if (pin1 == 0 && pin2 == 1)
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


    [BurstCompile]
    struct Constraint2Job : IJob
    {
        public            NativeArray<float3> vertices;
        [ReadOnly] public NativeArray<int2> constraintIndices;
        [ReadOnly] public NativeArray<float> constraintLengths;

        public void Execute()
        {
            for (int i = 0; i < constraintIndices.Length; i++)
            {
                int2 pair = constraintIndices[i];

                float3 p1 = vertices[pair.x];
                float3 p2 = vertices[pair.y];

                var delta = p2 - p1;
                var length = math.length(delta);
                var extra = (length - constraintLengths[i]) * .5f;
                var dir = delta / length;

                vertices[pair.x] += extra * dir;
                vertices[pair.y] -= extra * dir;
            }
        }
    }

    [BurstCompile]
    struct UpdateMeshJob : IJobParallelFor
    {
        public NativeArray<float3> vertices;
        public NativeArray<float3> oldVertices;
        [ReadOnly]
        public NativeArray<int> pins;
        public float3 gravity;
        public float4x4 localToWorld;
        public float4x4 worldToLocal;

        public void Execute(int i)
        {
            if (pins[i] == 0)
            {
                float3 oldVert = oldVertices[i];
                float3 vert = vertices[i];

                float3 startPos = vert;
                oldVert -= gravity;
                vert += (vert - oldVert);
                float3 worldPos = math.mul(localToWorld, new float4(vert, 1)).xyz;
                oldVert = startPos;

                if (worldPos.y < 0f)
                {
                    float3 oldWorldPos = math.mul(localToWorld, new float4(oldVert, 1)).xyz;
                    oldWorldPos.y = (worldPos.y - oldWorldPos.y) * .5f;
                    worldPos.y = 0f;
                    vert = math.mul(worldToLocal, new float4(worldPos, 1)).xyz;
                    oldVert = math.mul(worldToLocal, new float4(oldWorldPos, 1)).xyz;
                }

                vertices[i] = vert;
                oldVertices[i] = oldVert;
            }
        }
    }

    override protected void OnUpdate()
    {

        Entities.ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            var constraint1Job = new Constraint1Job
            {
                vertices = cloth.CurrentClothPosition,
                pins = cloth.Pins,
                constraintIndices = cloth.Constraint1Indices,
                constraintLengths = cloth.Constraint1Lengths
            };

            var constraint2Job = new Constraint2Job
            {
                vertices = cloth.CurrentClothPosition,
                constraintIndices = cloth.Constraint2Indices,
                constraintLengths = cloth.Constraint2Lengths
            };

            var meshJob = new UpdateMeshJob
            {
                vertices = cloth.CurrentClothPosition,
                oldVertices = cloth.PreviousClothPosition,
                pins = cloth.Pins,
                gravity = cloth.Gravity,
                localToWorld = localToWorld.Value,
                worldToLocal = math.inverse(localToWorld.Value)
            };

            var constraint1Handle = constraint1Job.Schedule();
            var constraint2Handle = constraint2Job.Schedule(constraint1Handle);
            var meshHandle = meshJob.Schedule(cloth.CurrentClothPosition.Length, 128, constraint2Handle);
            meshHandle.Complete();
        });

        Entities.ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            cloth.Mesh.SetVertices(cloth.CurrentClothPosition);
        });

        Entities.ForEach((ClothComponent cloth, ref LocalToWorld localToWorld) =>
        {
            Graphics.DrawMesh(cloth.Mesh, localToWorld.Value, cloth.Material, 0);
        });
    }
}