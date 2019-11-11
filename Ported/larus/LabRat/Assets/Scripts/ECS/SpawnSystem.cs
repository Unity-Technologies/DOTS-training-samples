using System.Collections;
using System.Collections.Generic;
using ECSExamples;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class SpawnSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameInProgressComponent>();
    }

    protected override void OnUpdate()
    {
        var gameConfig = GetEntityQuery(typeof(GameConfigComponent)).GetSingleton<GameConfigComponent>();
        var time = Time.time;
        Entities.ForEach((Entity entity, ref SpawnerComponent spawner, ref Translation position, ref Rotation rotation) =>
        {
            if (spawner.AlternatePrefab != Entity.Null && spawner.Timer - time >= 0)
            {
                if (spawner.InAlternate)
                {
                    //Debug.Log("Spawning alternate");
                    // TODO: Spawn Cat
                    spawner.Timer = time + Random.Range(1.0f, 3.0f);
                    spawner.InAlternate = false;
                }
                else
                {
                    spawner.Timer = time + Random.Range(4.0f, 15.0f);
                    spawner.InAlternate = true;
                }
            }

            if (spawner.TotalSpawned >= spawner.Max || spawner.InAlternate)
                return;

            spawner.Counter += Time.DeltaTime;
            while (spawner.Counter > spawner.Frequency || spawner.TotalSpawned == 0) {
                spawner.Counter -= spawner.Frequency;
                spawner.TotalSpawned++;

                var speed = gameConfig.EatenSpeed.RandomValue();
                if (spawner.PrimaryType == SpawnerType.Eater)
                    speed = gameConfig.EaterSpeed.RandomValue();

                //Debug.Log("Spawning " + spawner.Prefab + " counter=" + spawner.Counter + " totalSpawned=" + spawner.TotalSpawned + " max=" + spawner.Max);
                var rat = PostUpdateCommands.Instantiate(spawner.Prefab);
                PostUpdateCommands.SetComponent(rat, new WalkComponent{Speed = speed});
                PostUpdateCommands.SetComponent(rat, new Translation{Value = position.Value});
                PostUpdateCommands.SetComponent(rat, new Rotation{Value = rotation.Value});
            }
        });
    }
}