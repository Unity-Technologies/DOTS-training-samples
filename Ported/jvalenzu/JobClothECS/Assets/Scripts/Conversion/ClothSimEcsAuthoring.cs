#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

public class ClothSimEcsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum SystemType
    {
        IJob_IJob,
        IJob_IJobParallel,
        ForEach
    };
    public SystemType system = SystemType.IJob_IJob;

    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter)
        {
	    Mesh originalMesh = meshFilter.sharedMesh;

            // duplicate the mesh since we're going to be mucking with the vertices.
	    Mesh dupedMesh = meshFilter.sharedMesh = UnityEngine.Object.Instantiate(originalMesh);
            dupedMesh.MarkDynamic();

            // simulation relies on an implied velocity from position deltas
            DynamicBuffer<VertexStateCurrentElement> vertexStateCurrentElement = entityManager.AddBuffer<VertexStateCurrentElement>(entity);
            vertexStateCurrentElement.ResizeUninitialized(dupedMesh.vertices.Length);

            DynamicBuffer<VertexStateOldElement> vertexStateOldElement = entityManager.AddBuffer<VertexStateOldElement>(entity);
            vertexStateOldElement.ResizeUninitialized(dupedMesh.vertices.Length);

	    // re-get after structural changes
            vertexStateCurrentElement = entityManager.GetBuffer<VertexStateCurrentElement>(entity);
            vertexStateOldElement = entityManager.GetBuffer<VertexStateOldElement>(entity);
            for (int i=0,n=dupedMesh.vertices.Length; i<n; ++i)
            {
                vertexStateCurrentElement[i] = dupedMesh.vertices[i];
                vertexStateOldElement[i] = dupedMesh.vertices[i];
            }

            switch (system)
            {
                case ClothSimEcsAuthoring.SystemType.IJob_IJob:
		    {
			entityManager.AddComponentData(entity, new ClothInstanceIJobIJob
			{
			    worldToLocalMatrix = transform.worldToLocalMatrix,
			    localY0 = transform.worldToLocalMatrix.MultiplyPoint(new Vector3(0,0,0)).y, // not quite right
			});
			ClothSimEcsIJobSystem.AddSharedComponents(entity, originalMesh, entityManager);
			break;
		    }
                case ClothSimEcsAuthoring.SystemType.IJob_IJobParallel:
		    {
			entityManager.AddComponentData(entity, new ClothInstanceIJobIJobParallel
			{
			    worldToLocalMatrix = transform.worldToLocalMatrix,
			    localY0 = transform.worldToLocalMatrix.MultiplyPoint(new Vector3(0,0,0)).y, // not quite right
			});
			ClothSimEcsIJobParallelSystem.AddSharedComponents(entity, originalMesh, entityManager);
			break;
		    }
                case ClothSimEcsAuthoring.SystemType.ForEach:
		    {
			entityManager.AddComponentData(entity, new ClothInstanceForEach
			{
			    worldToLocalMatrix = transform.worldToLocalMatrix,
			    localY0 = transform.worldToLocalMatrix.MultiplyPoint(new Vector3(0,0,0)).y, // not quite right
			    clothConstraints = ClothSimForEach.AddBlobAssetReference(entity, originalMesh)
			});
			break;
		    }
            }
        }
    }
};
#endif
