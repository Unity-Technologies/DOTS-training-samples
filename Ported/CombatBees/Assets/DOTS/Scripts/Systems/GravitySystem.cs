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

public class GravitySystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var time = Time.DeltaTime;
        var gravity = new float3(0, -0.98f, 0);

        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();
            
        Entities
            .WithAll<HasGravity>()
            .ForEach((Entity entity, ref Velocity velocity, ref Translation translation, in NonUniformScale scale) =>
            {
                velocity.Value += gravity * time;
                translation.Value += velocity.Value;

                var halfHeight = scale.Value.y / 2f;

                if (translation.Value.y < halfHeight)
                {
                    translation.Value.y = halfHeight;
                    
                    ecb.RemoveComponent<HasGravity>(entity);
                    ecb.AddComponent<OnCollision>(entity);
                }
            }).Schedule();
        
        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
