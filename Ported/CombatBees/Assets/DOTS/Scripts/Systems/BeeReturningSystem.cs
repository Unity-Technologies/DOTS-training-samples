
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

public class BeeReturningSystem : SystemBase
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
        var YellowBase = GetSingletonEntity<YellowBase>();
        var YellowBaseAABB = EntityManager.GetComponentData<Bounds>(YellowBase).Value;

        var BlueBase = GetSingletonEntity<BlueBase>();
        var BlueBaseAABB = EntityManager.GetComponentData<Bounds>(BlueBase).Value;

        var offset = new float3(0, -0.98f, 0);

        Entities
             .WithAll<IsReturning>()
             .ForEach((Entity entity, in TargetPosition targetPosition, in Target target, in Translation translation) => {
                 
                 // if bee is close enough to Base
                 if (math.distancesq(translation.Value, targetPosition.Value) < 0.025)
                 {
                     ecb.RemoveComponent<IsReturning>(entity);
                     ecb.RemoveComponent<Target>(entity);
                     ecb.RemoveComponent<IsCarried>(target.Value);
                     ecb.AddComponent<HasGravity>(target.Value);
                 } else 
                 {
                     ecb.SetComponent<Translation>(target.Value, new Translation { Value = translation.Value + offset });
                 }
             }).Schedule();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
