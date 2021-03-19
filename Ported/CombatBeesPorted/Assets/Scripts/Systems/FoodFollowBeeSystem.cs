using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(MoveSystem))]
public class FoodFollowBeeSystem: SystemBase
{   
    protected override void OnUpdate()
    {
        Entities            
            .ForEach((Entity entity, in Food food,in PossessedBy possessedBy) =>
            {    
                SetComponent(entity, new Translation() {Value = new float3(0,-0.70f,0) + GetComponent<Translation>(possessedBy.Bee).Value} );                
            }).Schedule();
        
    }
    
}
