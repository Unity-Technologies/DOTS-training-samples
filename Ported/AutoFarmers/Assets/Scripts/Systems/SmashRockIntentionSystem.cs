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

        var settings = GetSingleton<CommonSettings>();
        var isRock = PathSystem.isRock;
        var defaultNavigation = PathSystem.defaultNavigation;
        var fullMapZone = new RectInt(0, 0, settings.GridSize.x, settings.GridSize.y);
        var entityRocks = GetEntityQuery(typeof(Rock)).ToEntityArray(Allocator.Temp);
        var rocks = GetEntityQuery(typeof(Rock)).ToComponentDataArray<Rock>(Allocator.Temp);

        var entityManager = EntityManager;
        var deltaTime = Time.DeltaTime;

        int2 resultRockPosition = int2.zero;

        Entities.WithAll<Farmer>()
            .ForEach(
                (Entity entity, ref DynamicBuffer<PathNode> pathNodes, ref SmashRockIntention smashRocks, in Translation translation) =>
                {
                    var farmerPosition = new int2((int) math.floor(translation.Value.x), (int) math.floor(translation.Value.z));

                    if (pathNodes.Length == 0)
                    {
                        var result = PathSystem.FindNearbyRock(farmerPosition.x, farmerPosition.y, 3600, tileBuffer, defaultNavigation, isRock, pathNodes, fullMapZone);
                        if (result == -1)
                        {
                            //If the result is Empty we don't have any nearby rock.
                            //TODO: Increase the range or change the Intention if there is no rock left in the board
                            ecb.RemoveComponent<SmashRockIntention>(entity);
                        }
                        else
                        {
                            PathSystem.Unhash(result, fullMapZone, out resultRockPosition.x, out resultRockPosition.y);
                            var targetRock = Entity.Null;

                            for (int i = 0; i < entityRocks.Length; i++)
                            {
                                var rock = rocks[i];
                                var rect = new RectInt(new Vector2Int(rock.Position.x, rock.Position.y), new Vector2Int(rock.Size.x, rock.Size.y));
                                if (rect.Contains(new Vector2Int(resultRockPosition.x, resultRockPosition.y)))
                                {
                                    targetRock = entityRocks[i];
                                }
                            }
                            smashRocks.TargetRock = targetRock;
                        }
                    }
                }).Run();

        rocks.Dispose();
        entityRocks.Dispose();

        Entities
            .WithAll<Farmer>()
            .WithNone<Searching>()
            .ForEach((Entity entity, ref DynamicBuffer<PathNode> pathNodes, ref SmashRockIntention smashRocks) =>
            {
                if (entityManager.Exists(smashRocks.TargetRock))
                {
                    if (pathNodes.Length == 1)
                    {
                        if (HasComponent<Rock>(smashRocks.TargetRock))
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
                }
            }).Run();


        ecb.Playback(EntityManager);
    }

}