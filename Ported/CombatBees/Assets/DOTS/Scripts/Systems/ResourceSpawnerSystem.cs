using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class ResourceSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var arena = GetSingletonEntity<IsArena>();
            var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

            var random = new Random((uint)DateTime.Now.Ticks);
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var spawnerEntity = GetSingletonEntity<ResourceSpawner>();
            var resourceSpawner = GetComponent<ResourceSpawner>(spawnerEntity);
            
            var instance = ecb.Instantiate(resourceSpawner.ResourcePrefab);
            
            var translation = new Translation { Value = Utils.BoundedRandomPosition(arenaAABB, ref random) };
            ecb.SetComponent(instance, translation);
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
