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

public class AntSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random(1234);

        Entities
            .ForEach((Entity entity, in AntSpawner spawner, in Respawn respawn) =>
            {
                ecb.RemoveComponent<Respawn>(entity);
                
                var side = (int)Math.Sqrt(spawner.AntCount);

                for (int i = 0; i < spawner.AntCount; i++)
                {
                    var x = (float) (i % side) / side * 10 - 5;
                    var y = (float) (i / side) / side * 10 - 5;
                    
                    var instance = ecb.Instantiate(spawner.AntPrefab);
                    
                    ecb.SetComponent(instance, new Translation {Value = new float3(x, y, 0)});
                    ecb.SetComponent(instance, new Position {Value = new float2(x, y)});
                    ecb.SetComponent(instance, new URPMaterialPropertyBaseColor 
                    {
                        Value = new float4((float)0x30 / 0x100, (float)0x36 / 0x100, (float)0x5A / 0x100, 0)
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}