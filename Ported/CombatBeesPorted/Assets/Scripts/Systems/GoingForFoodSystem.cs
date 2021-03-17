using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;


public class GoingForFoodSystem:SystemBase
{
    private EntityQuery boundsQuery;
    private const float boundsPadding=0.8f;
    protected override void OnCreate()
    {

        var boundsQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(SpawnBounds) },
            Any = new ComponentType[] { typeof(TeamA), typeof(TeamB) }
        };
        boundsQuery = GetEntityQuery(boundsQueryDesc);

    }

    private const float pickDistance = 1f;
    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var spawnBoundsArray = boundsQuery.ToEntityArray(Allocator.Temp);
        var random = new Random((uint)(Time.ElapsedTime * 10000) + 1);

        Entities
            .ForEach((Entity entity, ref Force force,in GoingForFood goingForFood,in FoodTarget foodTarget,in Translation beeTranslation, in Team team) =>
            {
                var targetFood = foodTarget.Value;
                if(!HasComponent<LocalToWorld>(targetFood))
                {
                    commandBuffer.RemoveComponent<GoingForFood>(entity);
                    commandBuffer.RemoveComponent<FoodTarget>(entity);
                    return;
                }
                else if (HasComponent<PossessedBy>(targetFood))
                {
                    commandBuffer.RemoveComponent<GoingForFood>(entity);
                    commandBuffer.RemoveComponent<FoodTarget>(entity);
                    return;
                }
                var targetTranslation = GetComponent<Translation>(targetFood);
                var targetMoveVector = targetTranslation.Value - beeTranslation.Value;

                

                //float3 randomDest = random.NextFloat3()

                if (math.length(targetMoveVector) <= pickDistance)
                {
                    var boundsComponent = GetComponent<SpawnBounds>(spawnBoundsArray[0]);
                    float extentsX = boundsComponent.Extents.x - boundsPadding;
                    float extentsY = boundsComponent.Extents.y - boundsPadding*2.5f; //Double padding on Y axis do bees don't try to bring food too close to ceiling or floor.
                    float extentsZ = boundsComponent.Extents.z - boundsPadding;
                    float randomTargetX = random.NextFloat(boundsComponent.Center.x - extentsX, boundsComponent.Center.x + extentsX);
                    float randomTargetY = random.NextFloat(boundsComponent.Center.y - extentsY, boundsComponent.Center.y + extentsY);
                    float randomTargetZ = random.NextFloat(boundsComponent.Center.z - extentsZ, boundsComponent.Center.z + extentsZ);
                    float3 randomTarget=new float3((team.index?-1:1)* randomTargetX,randomTargetY,randomTargetZ);
                    commandBuffer.RemoveComponent<GoingForFood>(entity);
                    commandBuffer.AddComponent<BringingFoodBack>(entity);
                    commandBuffer.SetComponent(entity,new BringingFoodBack(){TargetPosition = randomTarget});
                    commandBuffer.AddComponent<PossessedBy>(targetFood);
                    commandBuffer.SetComponent(targetFood,new PossessedBy(){Bee = entity});
                    return;
                }
                force.Value += math.normalize(targetMoveVector);
                     
            }).Run();
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
