using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameAI;
using Rendering;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public class RenderingUnity : MonoBehaviour
{
    public const float scale = 2.1f;

    public static float3 Tile2WorldPosition(int2 pos, int2 worldSizeHalf)
    {
        pos -= worldSizeHalf; 
        return new float3(pos.x, 0.0f, pos.y) * scale;
    }
    
    public static float3 Tile2WorldSize(int2 reqSize)
    {
        return new float3(reqSize.x, 1.0f/scale, reqSize.y) * scale;
    }
    
    static RenderingUnity m_instance;
    public static RenderingUnity instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<RenderingUnity>();
            return m_instance;
        }
    }

    public Entity CreateDrone(EntityManager em)
    {
        var atype = em.CreateArchetype(
            typeof(Translation),
            typeof(NonUniformScale), 
            typeof(LocalToWorld), 
            typeof(RenderMesh), 
            typeof(RenderingAnimationComponent),
            typeof(RenderingAnimationDroneFlyComponent),
            typeof(AITagTaskNone),
            typeof(TilePositionable));
        return Create(em, drone, atype);
    }
    
    public Entity CreateFarmer(EntityManager em)
    {
        var atype = em.CreateArchetype(
            typeof(Translation),
            typeof(NonUniformScale),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(RenderingAnimationComponent),
            typeof(FarmerAITag),
            typeof(AITagTaskNone),
            typeof(TilePositionable));
        return Create(em, farmer, atype);
    }
    
    public Entity CreateGround(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(LocalToWorld), typeof(RenderMesh));
        return Create(em, ground, atype);
    }

    public Entity CreateStone(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh));
        return Create(em, rock, atype);
    }

    public Entity CreatePlant(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh));
        return Create(em, plant, atype);
    }
    
    public Entity CreateStore(EntityManager em)
    {
        var atype = em.CreateArchetype(typeof(Translation),
            typeof(NonUniformScale), typeof(LocalToWorld), typeof(RenderMesh));
        return Create(em, store, atype);
    }

    private Entity Create(EntityManager em, MeshRenderer meshRenderer, EntityArchetype atype)
    {
        var e = em.CreateEntity(atype);
        Assert.IsTrue(em.HasComponent<RenderMesh>(e));
        em.SetSharedComponentData(e, new RenderMesh
        {
            mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh,
            material = meshRenderer.sharedMaterial,
            castShadows = ShadowCastingMode.On,
            receiveShadows = true
        });
        
        if (em.HasComponent<Translation>(e))
            em.SetComponentData(e, new Translation {Value = new float3(0, meshRenderer.transform.position.y, 0)});
        
        if (em.HasComponent<NonUniformScale>(e))
            em.SetComponentData(e, new NonUniformScale {Value = meshRenderer.transform.localScale});

        if (em.HasComponent<RenderingAnimationDroneFlyComponent>(e))
            em.SetComponentData(e, new RenderingAnimationDroneFlyComponent {offset = Random.Range(-Mathf.PI, Mathf.PI)});
        
        //TODO: make it disabled (how?)
        
        return e;
    }
    
    public MeshRenderer drone;
    public MeshRenderer farmer;
    public MeshRenderer ground;
    public MeshRenderer plant;
    public MeshRenderer rock;
    public MeshRenderer store;

    public void Initialize()
    {
        drone = MeshRendererScriptableObject.instance.drone;
        farmer = MeshRendererScriptableObject.instance.farmer;
        ground = MeshRendererScriptableObject.instance.ground;
        plant = MeshRendererScriptableObject.instance.plant;
        rock = MeshRendererScriptableObject.instance.rock;
        store = MeshRendererScriptableObject.instance.store;
            
        Assert.IsNotNull(drone, "drone == null");
        Assert.IsNotNull(farmer, "farmer == null");
        Assert.IsNotNull(ground, "ground == null");
        Assert.IsNotNull(plant, "plant == null");
        Assert.IsNotNull(rock, "rock == null");
        Assert.IsNotNull(store, "store == null");
    }
    
    private void Awake()
    {
        Initialize();
    }
}
