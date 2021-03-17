using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class FoodFollowBeeSystem: SystemBase
{
    
    protected override void OnUpdate()
    {
        var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        
        
        var translationFromEntity = GetComponentDataFromEntity<Translation>(true);
        Entities
            .ForEach((Entity entity,ref Translation foodTranslation, in Food food,in PossessedBy possessedBy) =>
            {
                if (!HasComponent<Bee>(possessedBy.Bee))
                {
                    commandBuffer.RemoveComponent<PossessedBy>(entity); // Hack to remove food attached to dead bee
                }
                foodTranslation.Value = translationFromEntity[possessedBy.Bee].Value+new float3(0,-1,0);
            }).Run();
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
    
}
