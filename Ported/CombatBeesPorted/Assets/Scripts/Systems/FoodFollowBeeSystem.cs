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
        var translationFromEntity = GetComponentDataFromEntity<Translation>(true);
        Entities
        .ForEach((Entity entity,ref Translation foodTranslation, in Food food,in PossessedBy possessedBy) =>
        {
            foodTranslation.Value = translationFromEntity[possessedBy.Bee].Value+new float3(0,-1,0);
        }).Run();
    }
    
}
