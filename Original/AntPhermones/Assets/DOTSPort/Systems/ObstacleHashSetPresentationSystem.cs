using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct ObstacleHashSetPresentationSystem : ISystem
{
   // public void OnCreate(ref SystemState state)
   // {
   //     state.RequireForUpdate<GlobalSettings>();
   //     state.RequireForUpdate<PheromoneBufferElement>();
   //     state.RequireForUpdate<CollisionHashSet>();
   // }
//
   // public void OnUpdate(ref SystemState state)
   // {
   //     var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
   //     var hashSet = SystemAPI.GetSingleton<CollisionHashSet>();
   //     var pheromoneBufferElement = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();
   //     
   //     Debug.Log($"ObstacleHashSetPresentationSystem: {hashSet.CollisionSet.Count}");
//
   //     foreach (var pos in hashSet.CollisionSet)
   //     {
   //         int index = PheromonesSystem.PheromoneIndex(pos.x, pos.y, globalSettings.MapSizeX);
   //         pheromoneBufferElement[index] = 1f;
   //     }
   // }
}
