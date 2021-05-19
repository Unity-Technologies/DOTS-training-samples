using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public class BeeGatheringSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();

        // Query for bees that are close enough to a Resource target to collect the Resource
        // TODO how to stop 2 bees collecting the same Resource
        var cdfeForTranslation = GetComponentDataFromEntity<Translation>(true);
        var YellowBase = GetSingletonEntity<YellowBase>();
        var YellowBaseAABB = EntityManager.GetComponentData<Bounds>(YellowBase).Value;

        var BlueBase = GetSingletonEntity<BlueBase>();
        var BlueBaseAABB = EntityManager.GetComponentData<Bounds>(BlueBase).Value;

        Entities
             .WithReadOnly(cdfeForTranslation)
             .WithAll<IsGathering>()
             .ForEach((Entity entity, ref TargetPosition targetPosition, in Target target, in Translation translation,  in Team team) => {

                if (cdfeForTranslation.HasComponent(target.Value))//(Value.StorageInfoFromEntity.Exists(target)) 
                {
                     if (math.distancesq(translation.Value, cdfeForTranslation[target.Value].Value) < 0.025)
                     {
                         ecb.RemoveComponent<IsGathering>(entity);
                         ecb.AddComponent<IsReturning>(entity);
                         ecb.AddComponent<IsCarried>(target.Value);

                         if (team.Id == 0) targetPosition.Value = YellowBaseAABB.Center;
                         else targetPosition.Value = BlueBaseAABB.Center;

                     }
                }
              
             }).Schedule();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
