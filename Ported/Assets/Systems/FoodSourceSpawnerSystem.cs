using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class FoodSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random((uint) System.DateTime.Now.Ticks);

        Entities
            .ForEach((Entity entity, in FoodSpawner spawner, in Respawn respawn) =>
            {
                ecb.RemoveComponent<Respawn>(entity);
                
                var foodSource = ecb.Instantiate(spawner.FoodPrefab);

                var direction = random.NextFloat(0, 2.0f * Mathf.PI);
                var position = new float3(Mathf.Cos(direction), Mathf.Sin(direction), 0) * 40f;

                ecb.SetComponent(foodSource, new Translation {Value = position});
                ecb.SetComponent(foodSource, new URPMaterialPropertyBaseColor
                {
                    Value = new float4(0, 1, 0, 0)
                });
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}