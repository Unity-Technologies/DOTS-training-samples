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

public class InitialSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = Utils.GetRandom();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var spawnerEntity = GetSingletonEntity<BeeSpawner>();
        var beeSpawner = GetComponent<BeeSpawner>(spawnerEntity);
        var resourceSpawner = GetComponent<ResourceSpawner>(spawnerEntity);

        var arena = GetSingletonEntity<IsArena>();
        var resourcesBounds = GetComponent<Bounds>(arena).Value;
        resourcesBounds.Extents.z *= 0.3f;
        resourcesBounds.Extents.y *= 0.3f;

        var numberOfBeesPerTeam = (int)(beeSpawner.BeeCount * 0.5);

        //Spawn bees
        Entities
            .ForEach((Entity entity, in Bounds bounds, in Team team) =>
            {
                for (int i = 0; i < numberOfBeesPerTeam; ++i)
                {
                    var instance = ecb.Instantiate(beeSpawner.BeePrefab);
                    ecb.SetComponent(instance, team);

                    var speed = random.NextFloat(0, beeSpawner.MaxSpeed);

                    ecb.SetComponent(instance, new Velocity { Value = UnityEngine.Random.onUnitSphere * speed });

                    ecb.SetComponent(instance, new Speed { Value = speed });

                    var translation = new Translation { Value = bounds.Value.Center };
                    ecb.SetComponent(instance, translation);

                    ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
                    {
                        Value = GetComponent<URPMaterialPropertyBaseColor>(entity).Value
                    });
                }
            }).Run();

        //Spawn resources
        for (int i = 0; i < resourceSpawner.ResourceCount; ++i)
        {
            var instance = ecb.Instantiate(resourceSpawner.ResourcePrefab);

            var translation = new Translation { Value = Utils.BoundedRandomPosition(resourcesBounds, ref random) };
            ecb.SetComponent(instance, translation);
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}
