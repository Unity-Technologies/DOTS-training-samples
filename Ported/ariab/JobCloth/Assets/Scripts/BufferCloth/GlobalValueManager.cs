using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct GlobalDamping : IComponentData
{
    public float Value;
}

struct GlobalIterationCount : IComponentData
{
    public int Value;
}

public class GlobalValueManager : MonoBehaviour
{
    public EntityManager EntityManager;
    
    public EntityQuery DampingGlobalQuery;
    public EntityQuery IterationCountGlobalQuery;
    
    [Range(0.9f,1.0f)]
    public float DampingValue = 0.95f;

    [Range(1, 32)] 
    public int IterationCount = 4;
    
    private void Start()
    {
        EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        DampingGlobalQuery = EntityManager.CreateEntityQuery(typeof(GlobalDamping));
        IterationCountGlobalQuery = EntityManager.CreateEntityQuery(typeof(GlobalIterationCount));
    }

    private void OnDestroy()
    {
        DampingGlobalQuery.Dispose();
        IterationCountGlobalQuery.Dispose();
    }

    private void Update()
    {
        if (DampingGlobalQuery.CalculateEntityCount() == 0)
            EntityManager.CreateEntity(typeof(GlobalDamping));
        
        DampingGlobalQuery.SetSingleton(new GlobalDamping{Value = DampingValue});
        
        if (IterationCountGlobalQuery.CalculateEntityCount() == 0)
            EntityManager.CreateEntity(typeof(GlobalIterationCount));
        
        IterationCountGlobalQuery.SetSingleton(new GlobalIterationCount{Value = IterationCount});
    }
}
