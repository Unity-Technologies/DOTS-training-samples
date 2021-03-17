using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


public class GoingForFoodSystem:SystemBase
{
    private const float pickDistance = 0.5f;
    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, ref Force force,in GoingForFood goingForFood,in FoodTarget foodTarget,in Translation beeTranslation, in Team team) =>
            {
                var targetFood = foodTarget.Value;
                if (HasComponent<PossessedBy>(targetFood))
                {
                    commandBuffer.RemoveComponent<GoingForFood>(entity);
                    commandBuffer.RemoveComponent<FoodTarget>(entity);
                    return;
                }
                var targetTranslation = GetComponent<Translation>(targetFood);
                var targetMoveVector = targetTranslation.Value - beeTranslation.Value;
                if (math.length(targetMoveVector) <= pickDistance)
                {
                    commandBuffer.RemoveComponent<GoingForFood>(entity);
                    commandBuffer.AddComponent<BringingFoodBack>(entity);
                    commandBuffer.SetComponent(entity,new BringingFoodBack(){TargetPosition = new float3((team.index?-1:1)*50,beeTranslation.Value.y+10,beeTranslation.Value.z)});
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
