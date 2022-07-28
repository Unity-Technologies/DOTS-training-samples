using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct BeeMovementSystem : ISystem
{
    ComponentDataFromEntity<NotCollected> _notCollected;
    ComponentDataFromEntity<YellowTeam> _yellowTeam;
    ComponentDataFromEntity<LocalToWorld> localToWorld;
    ComponentDataFromEntity<Food> food;
    ComponentDataFromEntity<Bee> _bee;

    private EntityQuery myQuery;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
     //   var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        state.RequireForUpdate<Base>();
       // _notCollected = SystemAPI.GetComponentDataFromEntity<NotCollected>();
       _notCollected = state.GetComponentDataFromEntity<NotCollected>();
       _yellowTeam = state.GetComponentDataFromEntity<YellowTeam>(true);
       localToWorld = state.GetComponentDataFromEntity<LocalToWorld>(false);
       food = state.GetComponentDataFromEntity<Food>();
       _bee = state.GetComponentDataFromEntity<Bee>();
       
        var queryBuilder = new EntityQueryDescBuilder(Allocator.Persistent);
        queryBuilder.AddAll(ComponentType.ReadWrite<Bee>());
        queryBuilder.AddAll(ComponentType.ReadWrite<Translation>());
        queryBuilder.AddAll(ComponentType.ReadWrite<NonUniformScale>());
        queryBuilder.AddAll(ComponentType.ReadWrite<Rotation>());
        queryBuilder.FinalizeQuery();
        myQuery = state.GetEntityQuery(queryBuilder);
    }

    public void OnDestroy(ref SystemState state) { }

    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _notCollected.Update(ref state);
        _yellowTeam.Update(ref state);
        localToWorld.Update(ref state);
        food.Update(ref state);
        _bee.Update(ref state);
        
        var dt = Time.deltaTime;
        var et = (float)state.Time.ElapsedTime;
        var speed = 10f;
        var offsetValue = 1f;
        var random = Random.CreateFromIndex(state.GlobalSystemVersion);
        var target = new float3(50, 0, 0);

        var baseComponent = SystemAPI.GetSingleton<Base>();
        var baseEntity = SystemAPI.GetSingletonEntity<Base>();

        
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var ecbSingletonStart = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb2 = ecbSingletonStart.CreateCommandBuffer(state.WorldUnmanaged);
        
        
        NativeArray<ArchetypeChunk> chunks =
            myQuery.ToArchetypeChunkArray(Allocator.Temp);
        
        EntityTypeHandle entityHandle = state.GetEntityTypeHandle();
        
      
        ComponentTypeHandle<Translation> translationHandle =
            state.GetComponentTypeHandle<Translation>(false);
        
        ComponentTypeHandle<Bee> beeHandle =
            state.GetComponentTypeHandle<Bee>(false);
        
        
        ComponentTypeHandle<Rotation> rotationHandle =
            state.GetComponentTypeHandle<Rotation>(false);
        
        ComponentTypeHandle<NonUniformScale> scaleHandle =
            state.GetComponentTypeHandle<NonUniformScale>(false);


        for (int i = 0; i < chunks.Length; i++)
        {
           
            
            var chunk = chunks[i];
            NativeArray<Entity> entities =
                chunk.GetNativeArray(entityHandle);
            NativeArray<Translation> translations =
                chunk.GetNativeArray(translationHandle);
            NativeArray<Bee> bees =
                chunk.GetNativeArray(beeHandle);
            NativeArray<Rotation> rotations =
                chunk.GetNativeArray(rotationHandle);
            NativeArray<NonUniformScale> scales =
                chunk.GetNativeArray(scaleHandle);
            
            
            
            for (int j = 0; j < chunk.Count; j++)
            {
                _notCollected.Update(ref state);
                _yellowTeam.Update(ref state);
                localToWorld.Update(ref state);
                food.Update(ref state);
                _bee.Update(ref state);
                
                var entity = entities[j];
                var translation = translations[j];
                var bee = bees[j];
                var rotation = rotations[j];
                var scale = scales[j];
                
                var randomPlaceInSpawn = _yellowTeam.HasComponent(entity)
                    ? random.NextFloat3(baseComponent.yellowBase.GetBaseLowerLeftCorner(), baseComponent.yellowBase.GetBaseUpperRightCorner())
                    : random.NextFloat3(baseComponent.blueBase.GetBaseLowerLeftCorner(), baseComponent.blueBase.GetBaseUpperRightCorner());

                var position = translation.Value;

                SpeedHandler(bee, ref speed, ref state);

                var floatOne = new float3(0.45f, 0.5f, 1);

                var scaleOffsetFloat = floatOne * (floatOne + new float3(1, 1, speed) / 30);

                scale.Value = scaleOffsetFloat;

                var offset = new float3(
                    noise.cnoise(new float2(et, offsetValue)),
                    noise.cnoise(new float2(et, offsetValue)),
                    noise.cnoise(new float2(et, offsetValue))
                );

                if (bee.target == baseEntity)
                {
                    target = bee.targetPos;
                }
                else if (bee.target != Entity.Null  && (_bee.HasComponent(bee.target) || food.HasComponent(bee.target)) )
                {
                    target = localToWorld[bee.target].Position;
                }
                else
                {
                    bee.state = BeeState.Idle;
                    bee.target = baseEntity;
                    bee.targetPos = randomPlaceInSpawn;
                    target = bee.targetPos;
                }

                

                if (bee.state == BeeState.Attacking)
                {
                    if(!_bee.HasComponent(bee.target))
                    {
                        bee.state = BeeState.Idle;
                        bee.target = baseEntity;
                        bee.targetPos = randomPlaceInSpawn;
                        target = bee.targetPos;
                    }
                    if (math.distance(position, target) < 0.25f)
                    {
                        bee.targetPos = float3.zero;
                        ecb2.DestroyEntity(bee.target);
                        
                        var beeSpawn = SystemAPI.GetSingleton<InitialSpawn>();
                        var instance = ecb.Instantiate(beeSpawn.bloodPrefab);
                        ecb.SetComponent(instance, new Translation { Value = position});

                        bee.target = Entity.Null;
                        bee.state = BeeState.Idle;
                    }
                } 
                else if (bee.state == BeeState.Collecting && food.HasComponent(bee.target))
                {
                    if (bee.target == Entity.Null || !_notCollected.IsComponentEnabled(bee.target))
                    {
                        bee.state = BeeState.Idle;
                        bee.target = baseEntity;
                        bee.targetPos = randomPlaceInSpawn;
                        target = bee.targetPos;
                    }
                    
                    if (math.distance(position, target) < 0.25f)
                    {
                        var component = food[bee.target];
                        component.target = entity;
                        ecb.SetComponent(bee.target, component);
                        
                        _notCollected.SetComponentEnabled(bee.target, false);

                        bee.target = baseEntity;
                        bee.targetPos = randomPlaceInSpawn;
                        bee.state = BeeState.Hauling;

                        target = bee.targetPos;
                    }
                }
                else if (bee.state == BeeState.Hauling)
                {
                    if (math.distance(position, target) < 0.25f)
                    {
                        bee.state = BeeState.Idle;
                        bee.target = baseEntity;
                        bee.targetPos = randomPlaceInSpawn;
                        target = bee.targetPos;
                    }
                }
                else if (bee.state == BeeState.Idle)
                {
                    if (math.distance(position, target) < 0.25f)
                    {
                        bee.target = baseEntity;
                        bee.targetPos = randomPlaceInSpawn;
                        target = bee.targetPos;
                    }
                }
                
                var direction = math.normalizesafe(target - translation.Value);

                rotation.Value = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));
                position += (direction + offset) * speed * dt;
                CheckBounds(ref position);

                translation.Value = position;
                
                
                ecb.SetComponent(entity, bee);
                ecb.SetComponent(entity, translation);
                ecb.SetComponent(entity, rotation);
                ecb.SetComponent(entity, scale);

            }
            /*entities.Dispose();
            translations.Dispose();
            rotations.Dispose();
            bees.Dispose();
            scales.Dispose();*/
        }
        
    }
    
    void CheckBounds(ref float3 position)
    {
        if (position.x < -50) position.x = -50;
        if (position.x > 50) position.x = 50;
        if (position.y < -10) position.y = -10;
        if (position.y > 10) position.y = 10;
        if (position.z < -10) position.z = -10;
        if (position.z > 10) position.z = 10;
    }

    void SpeedHandler(Bee bee, ref float speed, ref SystemState state)
    {
        Random random = Random.CreateFromIndex(state.GlobalSystemVersion);
        if (bee.state == BeeState.Attacking) speed = random.NextFloat(30f, 45f);
        if (bee.state == BeeState.Collecting) speed = random.NextFloat(15f, 30f);
        if (bee.state == BeeState.Hauling) speed = random.NextFloat(10f, 30f);
        if (bee.state == BeeState.Idle) speed = random.NextFloat(5f, 10f);
        
    }
    
}
