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

public class DestroyerSystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var timeDeltaTime = Time.DeltaTime;
 
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();

        Entities
            .ForEach((Entity entity, ref LifeSpan lifespan) =>
            {
                lifespan.Value -= timeDeltaTime;

                if (lifespan.Value<=0)
                {
                    //Destroys entity
                    ecb.DestroyEntity(entity);
                }
            }).Schedule();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
