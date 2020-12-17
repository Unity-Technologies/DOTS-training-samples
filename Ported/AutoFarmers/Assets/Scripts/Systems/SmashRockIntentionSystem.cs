using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FarmerIntentionSystem))]
public class SmashRockIntentionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var pathBuffers = this.GetBufferFromEntity<PathNode>();
        var pathMovement = World.GetExistingSystem<PathMovement>();

        var entityManager = EntityManager;
        var deltaTime = Time.DeltaTime;

        int2 resultRockPosition = int2.zero;
        bool foundRock = false;
        Entity targetRock = Entity.Null;

        //Testing
        // var rocks = GetEntityQuery(typeof(Rock)).ToEntityArray(Allocator.Temp);
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("SmashRock added");
        //     Entities.WithAll<Farmer>()
        //         .ForEach((Entity entity, ref SmashRocks smashRocks) => { smashRocks.TargetRock = rocks[0]; })
        //         .WithoutBurst().Run();
        // }


        Entities.WithAll<Farmer>()
            .ForEach((Entity entity, ref Velocity speed, ref SmashRocks smashRocks, in Translation translation) =>
            {
                var pathNodes = pathBuffers[entity];
                var farmerPosition = new int2((int) math.floor(translation.Value.x),
                    (int) math.floor(translation.Value.z));

                if (pathNodes.Length == 0)
                {
                    var result = pathMovement.FindNearbyRock(farmerPosition.x, farmerPosition.y, 3600, tileBuffer,
                        pathNodes);
                    // if (result == -1)
                    // {
                    //     //If the result is Empty we don't have any nearby rock.
                    //     //TODO: Increase the range or change the Intention if there is no rock left in the board
                    //     ecb.RemoveComponent<SmashRocks>(entity);
                    //     foundRock = false;
                    // }
                    // else
                    // {
                    //     pathMovement.Unhash(result, out resultRockPosition.x, out resultRockPosition.y);
                    //     foundRock = true;
                    // }
                }
            }).WithoutBurst().Run();

        if (foundRock)
        {
            Entities.ForEach((Entity entity, in Rock rock) =>
            {
                var rect = new RectInt(new Vector2Int(rock.Position.x, rock.Position.y), new Vector2Int(rock.Size.x, rock.Size.y));
                if (rect.Contains(new Vector2Int(resultRockPosition.x, resultRockPosition.y)))
                {
                    targetRock = entity;
                }
            }).Run();
            
            Entities.WithAll<Farmer>().WithNone<Searching>()
                .ForEach((Entity entity, ref SmashRocks smashRocks) =>
                {
                    if (entityManager.Exists(targetRock))
                    {
                        smashRocks.TargetRock = targetRock;
                        var rock = GetComponent<Rock>(smashRocks.TargetRock);
                        var health = rock.Health - (0.5f * deltaTime);
                        ecb.SetComponent(smashRocks.TargetRock, new Rock {Health = health, Position = rock.Position, Size = rock.Size});

                        if (rock.Health <= 0)
                        {
                            ecb.RemoveComponent<SmashRocks>(entity);
                        }
                    }
                }).Run();
        }

        ecb.Playback(EntityManager);
    }
}