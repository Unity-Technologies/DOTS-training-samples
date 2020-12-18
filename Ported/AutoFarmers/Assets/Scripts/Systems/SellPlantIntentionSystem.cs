using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FarmerIntentionSystem))]
public class SellPlantIntentionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var pathMovement = World.GetExistingSystem<PathMovement>();

        var entityManager = EntityManager;
        var deltaTime = Time.DeltaTime;

        int2 resultPlantPosition = int2.zero;

        Entities.WithAll<Farmer>()
            .ForEach(
                (Entity entity, ref DynamicBuffer<PathNode> pathNodes, ref SellPlantIntention sellPlantIntention, in Translation translation) =>
                {
                    var farmerPosition = new int2((int) math.floor(translation.Value.x),
                        (int) math.floor(translation.Value.z));

                    if (pathNodes.Length == 0)
                    {
                        // var result = pathMovement.FindNearbyPlant(farmerPosition.x, farmerPosition.y, 3600, tileBuffer, pathNodes);
                        // if (result == -1)
                        // {
                        //     //If the result is Empty we don't have any nearby rock.
                        //     //TODO: Increase the range or change the Intention if there is no plant left in the board
                        //     ecb.RemoveComponent<SellPlantIntention>(entity);
                        // }
                        // else
                        // {
                        //     pathMovement.Unhash(result, out resultPlantPosition.x, out resultPlantPosition.y);
                        //     var targetRock = PositionToPlant(resultPlantPosition);
                        //     sellPlantIntention.TargetPlant = targetRock;
                        // }
                    }
                }).WithoutBurst().Run();

        Entities.WithAll<Farmer>().WithNone<Searching>()
            .ForEach((Entity entity, ref DynamicBuffer<PathNode> pathNodes, ref SellPlantIntention sellPlantIntention) =>
            {
                if (entityManager.Exists(sellPlantIntention.TargetPlant))
                {
                    if (pathNodes.Length == 1)
                    {
                        var plant = GetComponent<Plant>(sellPlantIntention.TargetPlant);
                        //TODO: Take the plant and go to the Silo
                    }
                }
            }).Run();


        ecb.Playback(EntityManager);
    }

    private Entity PositionToPlant(int2 plantPosition)
    {
        var entityPlants = GetEntityQuery(typeof(Rock)).ToEntityArray(Allocator.Temp);
        foreach (var entityPlant in entityPlants)
        {
            var plant = GetComponent<Rock>(entityPlant);
            var rect = new RectInt(new Vector2Int(plant.Position.x, plant.Position.y),
                new Vector2Int(plant.Size.x, plant.Size.y));
            if (rect.Contains(new Vector2Int(plantPosition.x, plantPosition.y)))
            {
                return entityPlant;
            }
        }

        return Entity.Null;
    }
}