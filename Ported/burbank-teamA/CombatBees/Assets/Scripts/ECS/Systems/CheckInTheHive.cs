using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class CheckInTheHive : JobComponentSystem
{

    BeginInitializationEntityCommandBufferSystem bufferSystem;

    GameObject[] spawningTeamBees;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        spawningTeamBees = GameObject.FindObjectOfType<MouseInputManager>().spawningTeamBees;

    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>();
        var commandBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent();

        var team1 = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningTeamBees[0], World.Active);
        var team2 = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawningTeamBees[1], World.Active);

        return Entities.WithAny<ResourceTag>()
            .ForEach((Entity e, ref Translation t) =>
            {

                if (t.Value.y < -gameBounds.Value.y && math.abs(t.Value.x) > gameBounds.Value.x * gameBounds.PlayAreaPercentage)
                    {

                        var spawnedBee = commandBuffer.Instantiate(0, t.Value.x<0?team1:team2);
                        commandBuffer.SetComponent(0, spawnedBee, new Translation
                        {
                            Value = t.Value
                        });

                        commandBuffer.DestroyEntity(0, e);
                    }
            })
            .Schedule(inputDeps);
    }
}
