using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


public class GoingForFoodSystem:SystemBase
{
    private EntityQuery boundsQuery;
    protected override void OnCreate()
    {

        var boundsQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(Bounds2D) },
            Any = new ComponentType[] { typeof(TeamA), typeof(TeamB) }
        };
        boundsQuery = GetEntityQuery(boundsQueryDesc);

    }

    private const float pickDistance = 0.5f;
    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var spawnBoundsArray = boundsQuery.ToEntityArray(Allocator.Temp);
        var random = new Random((uint)(Time.DeltaTime * 10000) + 1);

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

                var boundsComponent = GetComponent<Bounds2D>(spawnBoundsArray[0]);

                var distance = math.abs(boundsComponent.Center.x);

                float minX = (boundsComponent.Center.x - boundsComponent.Extents.x);
                float maxX  = (boundsComponent.Center.x + boundsComponent.Extents.x);

                //float3 randomDest = random.NextFloat3()

                if (math.length(targetMoveVector) <= pickDistance)
                {
                    commandBuffer.RemoveComponent<GoingForFood>(entity);
                    commandBuffer.AddComponent<BringingFoodBack>(entity);
                    commandBuffer.SetComponent(entity,new BringingFoodBack(){TargetPosition = new float3((team.index?-1:1)* random.NextFloat(minX, maxX), beeTranslation.Value.y+10,beeTranslation.Value.z)});
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
