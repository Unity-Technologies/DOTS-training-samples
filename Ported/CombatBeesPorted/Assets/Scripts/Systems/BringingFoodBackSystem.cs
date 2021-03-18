using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


public class BringingFoodBackSystem:SystemBase
{
    private const float dropDistance = 1f;
    private const float carryingSpeed = 0.5f;
    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity,ref Force force, in BringingFoodBack bringingFoodBack, in FoodTarget foodTarget,in Translation beeTranslation, in Team team) =>
            {
                var targetTranslation = bringingFoodBack.TargetPosition;
                var targetMoveVector = targetTranslation - beeTranslation.Value;
                if (math.length(targetMoveVector) <= dropDistance)
                {
                    commandBuffer.RemoveComponent<BringingFoodBack>(entity);
                    commandBuffer.RemoveComponent<FoodTarget>(entity);
                    commandBuffer.RemoveComponent<PossessedBy>(foodTarget.Value);
                    //commandBuffer.DestroyEntity(foodTarget.Value);
                    return;
                }
                force.Value += math.normalize(targetMoveVector)*carryingSpeed;
                     
            }).Run();
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}