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

        Entities.WithAll<Farmer>()
            .ForEach(
                (Entity entity, ref SmashRockIntention smashRocks, in Translation translation) =>
                {
                    var pathNodes = pathBuffers[entity];
                    var farmerPosition = new int2((int) math.floor(translation.Value.x),
                        (int) math.floor(translation.Value.z));

                    if (pathNodes.Length == 0)
                    {
                        var result = pathMovement.FindNearbyRock(farmerPosition.x, farmerPosition.y, 3600, tileBuffer,
                            pathNodes);
                        if (result == -1)
                        {
                            //If the result is Empty we don't have any nearby rock.
                            //TODO: Increase the range or change the Intention if there is no rock left in the board
                            ecb.RemoveComponent<SmashRockIntention>(entity);
                        }
                        else
                        {
                            pathMovement.Unhash(result, out resultRockPosition.x, out resultRockPosition.y);
                            var targetRock = PositionToRock(resultRockPosition);
                            smashRocks.TargetRock = targetRock;
                        }
                    }
                }).WithoutBurst().Run();

        Entities.WithAll<Farmer>().WithNone<Searching>()
            .ForEach((Entity entity, ref SmashRockIntention smashRocks) =>
            {
                if (entityManager.Exists(smashRocks.TargetRock))
                {
                    var pathNodes = pathBuffers[entity];
                    if (pathNodes.Length == 1)
                    {
                        var rock = GetComponent<Rock>(smashRocks.TargetRock);
                        rock.Health -= 1f * deltaTime;
                        ecb.SetComponent(smashRocks.TargetRock, rock);

                        if (rock.Health <= 0)
                        {
                            ecb.RemoveComponent<SmashRockIntention>(entity);
                        }
                    }
                }
            }).Run();


        ecb.Playback(EntityManager);
    }

    private Entity PositionToRock(int2 rockPosition)
    {
        var entityRocks = GetEntityQuery(typeof(Rock)).ToEntityArray(Allocator.Temp);
        foreach (var entityRock in entityRocks)
        {
            var rock = GetComponent<Rock>(entityRock);
            var rect = new RectInt(new Vector2Int(rock.Position.x, rock.Position.y),
                new Vector2Int(rock.Size.x, rock.Size.y));
            if (rect.Contains(new Vector2Int(rockPosition.x, rockPosition.y)))
            {
                return entityRock;
            }
        }

        return Entity.Null;
    }
}