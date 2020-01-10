using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Unity.Rendering;

[UpdateAfter(typeof(ClothSimRenderMesh))]
public class ClothSimForEach : JobComponentSystem
{
    public enum ArbitraryResourceConstants
    {
        kMaxBlobAssets = 128
    };

    static Dictionary<Mesh, int> s_MeshToBlobAssetReferenceIndexLookup = new Dictionary<Mesh, int>();
    static BlobAssetReference<ClothConstraintAsset>[] blobAssets = new BlobAssetReference<ClothConstraintAsset>[(int)ArbitraryResourceConstants.kMaxBlobAssets];
    static int blobAssetCount;

    // table-lookup to make the verlet math work out
    //
    // p0 | p1 | c0 | d0
    // -----------------
    //  0 |  0 |  1 |  1
    //  0 |  1 |  2 |  0
    //  1 |  0 |  0 |  2
    //  1 |  1 |  0 |  0
    NativeArray<float> scaleX = new NativeArray<float>(new float[]{1.0f, 2.0f, 0.0f, 0.0f}, Allocator.Persistent);
    NativeArray<float> scaleY = new NativeArray<float>(new float[]{1.0f, 0.0f, 2.0f, 0.0f}, Allocator.Persistent);

    // we both find relevant entities and partition their immutable data via the ClothBarSimEcs
    // component.
    //
    // debated moving this into a two-stage initialization, but it's tough to actually jobify since
    // we're going to end up accessing the managed Mesh type for vertex and normal data.  Moving
    // it the other direction, to a tool time conversion, seems preferable.
    static public BlobAssetReference<ClothConstraintAsset> AddBlobAssetReference(Entity entity, Mesh mesh, EntityManager dstManager)
    {
        if (!s_MeshToBlobAssetReferenceIndexLookup.ContainsKey(mesh))
        {
            BlobBuilder builder = new BlobBuilder(Allocator.Temp);
            ref ClothConstraintAsset root = ref builder.ConstructRoot<ClothConstraintAsset>();
            BlobBuilderArray<byte> pins = builder.Allocate(ref root.pinState, mesh.vertices.Length);

            if (mesh.normals == null)
            {
                for (int i=0,n=pins.Length; i<n; ++i) {
                    if (mesh.vertices[i].y > 0.3f)
                        pins[i] = 1;
                }
            }
            else
            {
                for (int i=0,n=pins.Length; i<n; ++i) {
                    if (mesh.normals[i].y >= 0.9f && mesh.vertices[i].y > 0.3f)
                        pins[i] = 1;
                }
            }

            HashSet<Vector2Int> barLookup = new HashSet<Vector2Int>();
            int[] triangles = mesh.triangles;
            for (int i=0, n=triangles.Length; i<n; i += 3)
            {
                for (int j=0; j<3; ++j)
                {
                    Vector2Int pair = new Vector2Int();
                    pair.x = triangles[i + j];
                    pair.y = triangles[i + (j + 1)%3];
                    if (pair.x > pair.y) {
                        int temp = pair.x;
                        pair.x = pair.y;
                        pair.y = temp;
                    }

                    if (barLookup.Contains(pair) == false &&
                        // two pinned verts can't move, so don't simulate them
                        pins[pair.x] + pins[pair.y] != 2)
                    {
                        barLookup.Add(pair);
                    }
                }
            }

            List<Vector2Int> barList = new List<Vector2Int>(barLookup);

            var constraintArray = builder.Allocate(ref root.constraints, barList.Count);

            for (int i=0,n=barList.Count; i<n; ++i) {
                Vector2Int pair = barList[i];
                Vector3 p1 = mesh.vertices[pair.x];
                Vector3 p2 = mesh.vertices[pair.y];

                ClothConstraint constraint = new ClothConstraint();
                constraint.x = (ushort) pair.x;
                constraint.y = (ushort) pair.y;
                constraint.pinPair = (ushort) ((pins[pair.x]<<1) | pins[pair.y]);
                constraint.length = (ushort) ((p2 - p1).magnitude * 256);

                constraintArray[i] = constraint;
            }

            int index = blobAssetCount++;
            blobAssets[index] = builder.CreateBlobAssetReference<ClothConstraintAsset>(Allocator.Persistent);
            s_MeshToBlobAssetReferenceIndexLookup.Add(mesh, index);

            builder.Dispose();
        }

        return blobAssets[s_MeshToBlobAssetReferenceIndexLookup[mesh]];
    }

    protected override void OnDestroy()
    {
        scaleX.Dispose();
        scaleY.Dispose();

        for (int i=0,n=blobAssets.Length; i<n; ++i)
        {
            if (blobAssets[i].IsCreated)
                blobAssets[i].Release();
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        UnityEngine.Profiling.Profiler.BeginSample("ClothSetup");

        // can't hoist, will generate il2cpp codegen error about other omitted captures
        float4 worldGravity = new float4(-Vector3.up * Time.DeltaTime*Time.DeltaTime, 0.0f);

        NativeArray<float> localScaleX = scaleX;
        NativeArray<float> localScaleY = scaleY;

        JobHandle combinedJobHandle = Entities
            .ForEach((int entityInQueryIndex,
                      ref DynamicBuffer<VertexStateOldElement> oldVertexState,
                      ref DynamicBuffer<VertexStateCurrentElement> currentVertexState,
                      in ClothInstanceForEach clothInstance,
                      in LocalToWorld localToWorld) =>
            {
                float3 localGravity = math.mul(clothInstance.worldToLocalMatrix, worldGravity).xyz;

                NativeArray<float3> vertices = currentVertexState.Reinterpret<float3>().AsNativeArray();
                NativeArray<float3> oldVertices = oldVertexState.Reinterpret<float3>().AsNativeArray();
                int vLength = clothInstance.clothConstraints.Value.pinState.Length;

                // update vertices according to length constraints.  This process is serial
                // due to the nature of the dependencies between vertices.
                ref BlobArray<ClothConstraint> constraints = ref clothInstance.clothConstraints.Value.constraints;

                for (int i=0,n=constraints.Length; i<n; ++i)
                {
                    ClothConstraint constraint = constraints[i];

                    float length = constraint.length * 1.953125e-3f; // (1/256) * (1/2)
                    float3 p1 = vertices[constraint.x];
                    float3 p2 = vertices[constraint.y];
                    float3 v0 = p2 - p1;
                    float3 v1 = v0 * (0.5f - length / math.length(v0));

                    int pinPairIndex = constraint.pinPair;
                    p1 += v1 * localScaleX[pinPairIndex];
                    p2 -= v1 * localScaleY[pinPairIndex];

                    vertices[constraint.x] = p1;
                    vertices[constraint.y] = p2;
                }

                ref BlobArray<byte> pins = ref clothInstance.clothConstraints.Value.pinState;
                for (int i=0,n=pins.Length; i<n; ++i)
                {
                    if (pins[i] != 0)
                        continue;

                    float3 oldVert = oldVertices[i];
                    float3 vert = vertices[i];

                    float3 v0 = vert;
                    float3 v1 = 2.0f * vert - oldVert + localGravity;

                    if (v1.y < clothInstance.localY0) {
                        float3 worldPos = math.transform(localToWorld.Value, v1);
                        Vector3 oldWorldPos = math.transform(localToWorld.Value, v0);

                        oldWorldPos.y = (worldPos.y - oldWorldPos.y) * .5f;
                        worldPos.y = 0.0f;

                        v0 = math.transform(clothInstance.worldToLocalMatrix, oldWorldPos);
                        v1 = math.transform(clothInstance.worldToLocalMatrix, worldPos);
                    }

                    oldVertices[i] = v0;
                    vertices[i] = v1;
                }
            }).Schedule(inputDeps);

        UnityEngine.Profiling.Profiler.EndSample();

        return combinedJobHandle;
    }
}
