using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public class FarmerManager : MonoBehaviour
{
    [SerializeField]
    Mesh m_FarmerMesh;
    [SerializeField]
    Material m_FarmerMaterial;
    void Start()
    {
        CreateFarmerEntity();
    }

    void CreateFarmerEntity()
    {
        
        
        
         EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = manager.CreateArchetype(
            typeof(Translation),
            typeof(Rotation),
            typeof(RenderMesh),
            typeof(RenderBounds),
        typeof(LocalToWorld),
            typeof(FarmerSpeed));
        
         var farmer = manager.CreateEntity(archetype);
        // manager.SetName(farmer,"FarmerEntity");
        // manager.SetComponentData(farmer, new Translation
        // {
        //     Value = new float3(0,0,0)
        // });
        //
        //
        // manager.SetSharedComponentManaged(farmer, new RenderMesh
        // {
        //     mesh = m_FarmerMesh,
        //     material = m_FarmerMaterial,
        // });
        //
        // //---------------------
        // var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //
        // var renderer = cube.GetComponent<MeshRenderer>();
        // var mesh = cube.GetComponent<MeshFilter>().mesh;
        //
        // var desc = new RenderMeshDescription(renderer);
        // var meshArray = new RenderMeshArray(new[] { renderer.material }, new[] { mesh });
        // var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
        //
        // var entity = manager.CreateEntity();
        // manager.SetName(entity, "Cube");
        // RenderMeshUtility.AddComponents(entity, manager, desc, meshArray, materialMeshInfo);
        // manager.SetComponentData(entity, new LocalToWorld { Value = float4x4.identity });
        //
        // Object.DestroyImmediate(cube);
    }

    void EntitySecond()
    {
        
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        var renderer = cube.GetComponent<MeshRenderer>();
        var mesh = cube.GetComponent<MeshFilter>().mesh;

        var desc = new RenderMeshDescription(renderer);
        var meshArray = new RenderMeshArray(new[] { renderer.material }, new[] { mesh });
        var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
        //
        // var entity = EntityManager.CreateEntity();
        // EntityManager.SetName(entity, "Cube");
        // RenderMeshUtility.AddComponents(entity, EntityManager, desc, meshArray, materialMeshInfo);
        // EntityManager.SetComponentData(entity, new LocalToWorld { Value = float4x4.identity });

        Object.DestroyImmediate(cube);
    }


}

