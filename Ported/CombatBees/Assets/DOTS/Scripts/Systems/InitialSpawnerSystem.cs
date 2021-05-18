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
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        var spawnerEntity = GetSingletonEntity<InitialSpawner>();
        var spawner = GetComponent<InitialSpawner>(spawnerEntity);


        //Spawn bees
        Entities
            .ForEach(( in Bounds bounds, in Team team) =>
            {
                
                for (int i = 0; i < spawner.BeeCount/2; ++i)
                {
                    var instance = ecb.Instantiate(spawner.BeePrefab);
                    ecb.AddComponent(instance, team);

                    var translation = new Translation { Value = bounds.Value.Center };
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        //Spawn resources
        Entities
            .WithNone<Team>()
            .ForEach((in Bounds bounds) =>
            {

                for (int i = 0; i < spawner.ResourceCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.ResourcePrefab);

                    var translation = new Translation { Value = bounds.Value.Center };
                    //ecb.SetComponent(instance, translation);
                }
            }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}
