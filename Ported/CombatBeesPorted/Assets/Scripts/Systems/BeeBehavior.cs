using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BeeBehavior: SystemBase
{
    private EntityQuery FoodQuery;
    protected override void OnCreate()
    {
        FoodQuery=GetEntityQuery(ComponentType.ReadOnly<Food>());
    }

    protected override void OnUpdate()
    {

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var foodEntities=FoodQuery.ToEntityArray(Allocator.Temp);
        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));                
        Entities
            .WithNone<GoingForFood,Attacking,BringingFoodBack>()
            .ForEach((Entity entity, in Bee bee, in Translation translation) =>
            {
                if(random.NextBool())
                {
                    int foodCount = foodEntities.Length;
                    if (foodCount == 0)
                        return;
                    commandBuffer.AddComponent<GoingForFood>(entity);
                    commandBuffer.AddComponent<FoodTarget>(entity);
                    commandBuffer.SetComponent(entity,new FoodTarget(){Value = foodEntities[random.NextInt(foodCount)]});
                } else {
                    // go for attack
                }
            }).Run();
        
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
