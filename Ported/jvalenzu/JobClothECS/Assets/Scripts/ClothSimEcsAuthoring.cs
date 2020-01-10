using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

public class ClothSimEcsAuthoring : MonoBehaviour
{
    public enum SystemType
    {
        IJob_IJob,
        IJob_IJobParallel,
        ForEach
    };
    public SystemType system = SystemType.IJob_IJob;
    
    void Start()
    {
        // would prefer to use the new conversion workflow but I couldn't get it to work robustly -
        // invariably my scene would end up incorrectly dying trying to deserialize a
        // Unity.Collections.LowLevel.Unsafe.DisposeSentinel. eg:
        //
        // Error when processing 'AsyncLoadSceneJob(VirtualArtifacts/Extra/89/89acec624b02cac27fb29a6851f893bc.0.entities)': System.MissingMethodException: Default constructor not found for type Unity.Collections.LowLevel.Unsafe.DisposeSentinel
        //
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter)
        {
            // duplicate the mesh since we're going to be mucking with the vertices.
            Mesh originalMesh = meshFilter.sharedMesh;
            Mesh mesh = meshFilter.sharedMesh = Instantiate(originalMesh);
            mesh.MarkDynamic();

            // implicitly convert mesh features
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObject, settings);

            // simulation relies on an implied velocity from position deltas
            DynamicBuffer<VertexStateCurrentElement> vertexStateCurrentElement = entityManager.AddBuffer<VertexStateCurrentElement>(entity);
            vertexStateCurrentElement.ResizeUninitialized(mesh.vertices.Length);

            DynamicBuffer<VertexStateOldElement> vertexStateOldElement = entityManager.AddBuffer<VertexStateOldElement>(entity);
            vertexStateOldElement.ResizeUninitialized(mesh.vertices.Length);

            // this slightly awkward initialization pattern (add buffer, then get) is to avoid:
            // InvalidOperationException: The NativeArray has been deallocated, it is not allowed to access it
            // Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle.CheckWriteAndThrowNoEarlyOut (Unity.Collections.LowLevel.Unsafe.AtomicSafetyHandle handle) <0x1a344da60 + 0x00052> in <ad86e8e508c54768abd7c1fb9256ddc2>:0
            vertexStateCurrentElement = entityManager.GetBuffer<VertexStateCurrentElement>(entity);
            vertexStateOldElement = entityManager.GetBuffer<VertexStateOldElement>(entity);
            for (int i=0,n=mesh.vertices.Length; i<n; ++i)
            {
                vertexStateCurrentElement[i] = mesh.vertices[i];
                vertexStateOldElement[i] = mesh.vertices[i];
            }

            switch (system)
            {
                case SystemType.IJob_IJob:
                {
                    entityManager.AddComponentData(entity, new ClothInstanceIJobIJob
                    {
                        worldToLocalMatrix = transform.worldToLocalMatrix,
                        localY0 = transform.worldToLocalMatrix.MultiplyPoint(new Vector3(0,0,0)).y, // not quite right
                    });
                    ClothSimEcsIJobSystem.AddSharedComponents(entity, originalMesh, entityManager);
                    break;
                }
                case SystemType.IJob_IJobParallel:
                {
                    entityManager.AddComponentData(entity, new ClothInstanceIJobIJobParallel
                    {
                        worldToLocalMatrix = transform.worldToLocalMatrix,
                        localY0 = transform.worldToLocalMatrix.MultiplyPoint(new Vector3(0,0,0)).y, // not quite right
                    });
                    ClothSimEcsIJobParallelSystem.AddSharedComponents(entity, originalMesh, entityManager);
                    break;
                }
                case SystemType.ForEach:
                {
                    entityManager.AddComponentData(entity, new ClothInstanceForEach
                    {
                        worldToLocalMatrix = transform.worldToLocalMatrix,
                        localY0 = transform.worldToLocalMatrix.MultiplyPoint(new Vector3(0,0,0)).y, // not quite right
                        clothConstraints = ClothSimForEach.AddBlobAssetReference(entity, originalMesh, entityManager)
                    });
                    break;
                }
            }


            meshFilter.sharedMesh = originalMesh;
            UnityEngine.Object.Destroy(gameObject);
        }
    }
};
