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

public class BeeGatheringSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        // Query for bees that are close enough to a Resource target to collect the Resource
        // TODO how to stop 2 bees collecting the same Resource
        // var cdfeForTranslation = GetComponentDataFromEntity<Translation>(true);
        
        // Entities
        //     .WithAll<IsGathering>()
        //     .ForEach((Entity entity, in Translation translation, in Target target, in Team team) => {
        //         // if bee is close enough to target
        //         if (math.distancesq(translation.Value, cdfeForTranslation[target.Value].Value) < 0.05 * 0.05)
        //         {
        //             ecb.RemoveComponent<IsGathering>(entity);
        //             ecb.AddComponent<IsReturning>(entity);
        //             ecb.AddComponent<IsCarried>(target.Value);
        //             target.Value = ; // position in team's base
        //         }
        //     }).Schedule();
        //
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
    }
}
