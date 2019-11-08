using System.Collections;
using System.Collections.Generic;
using ECSExamples;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpawnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var gameConfig = GetEntityQuery(typeof(GameConfigComponent)).GetSingleton<GameConfigComponent>();
        //var time = Time.ElapsedTime;
        Entities.ForEach((Entity entity, ref SpawnerComponent spawner, ref Translation position, ref Rotation rotation) =>
        {
            /*if (spawner.Timer - (float)time <= 0)
            {
                if (spawner.InAlternate)
                {
                    Debug.Log("Spawning cat");
                    // TODO: Spawn Cat
                    spawner.Timer = (float) time + Random.Range(1.0f, 3.0f);
                    spawner.InAlternate = false;
                }
                else
                {
                    spawner.Timer = (float) time + Random.Range(4.0f, 15.0f);
                    spawner.InAlternate = true;
                }
            }*/


            if (spawner.InAlternate || spawner.TotalSpawned >= spawner.Max)
                return;

            spawner.Counter += Time.deltaTime;
            //bool didSpawn = false;
            while (spawner.Counter > spawner.Frequency) {
                spawner.Counter -= spawner.Frequency;
                spawner.TotalSpawned++;

                var mouseSpeed = gameConfig.Speed.RandomValue();

                //Debug.Log("Spawning mouse");
                var rat = PostUpdateCommands.Instantiate(spawner.Prefab);
                PostUpdateCommands.SetComponent(rat, new WalkComponent{Speed = mouseSpeed});
                PostUpdateCommands.SetComponent(rat, new Translation{Value = position.Value});
                PostUpdateCommands.SetComponent(rat, new Rotation{Value = rotation.Value});
                //didSpawn = true;
            }
            // DEBUG
            //if (didSpawn)
                //PostUpdateCommands.RemoveComponent<SpawnerComponent>(entity);
        });
    }
}