using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BeeBehavior: SystemBase
{
    private EntityQueryDesc allBeesQueryDesc; 
    private EntityQuery FoodQuery;
    protected override void OnCreate()
    {
        FoodQuery=GetEntityQuery(ComponentType.ReadOnly<Food>());
        allBeesQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(Bee)} //, ComponentType.ReadOnly<WorldRenderBounds>()
        };

    }

    protected override void OnUpdate()
    {

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var foodEntities=FoodQuery.ToEntityArray(Allocator.Temp);
        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));                

        EntityQuery allBeesQuery = GetEntityQuery(allBeesQueryDesc);
        NativeArray<Entity> allBees = allBeesQuery.ToEntityArray(Allocator.Temp);

        Entities
            .WithNone<GoingForFood,Attacking,BringingFoodBack>()
            .ForEach((Entity entity, in Bee bee, in Translation translation, in Team team) =>
            {
                if(random.NextBool())                
                {
                    int foodCount = foodEntities.Length;
                    if (foodCount == 0)
                        return;
                    commandBuffer.AddComponent<GoingForFood>(entity);
                    commandBuffer.AddComponent<FoodTarget>(entity, new FoodTarget(){Value = foodEntities[random.NextInt(foodCount)] } );
                } else {
                    var enemyTeam = !team.index;
                    // go for attack
                    // find nearest enemy bee
                    float minDistance = 0xFFFFFF;
                    Entity enemyBee = Entity.Null;
                    foreach(Entity anotherBee in allBees) {
                        //if( !HasComponent<Team>(anotherBee) ) continue;
                        if( GetComponent<Team>(anotherBee).index != enemyTeam) continue;
                        float distance = math.distance(GetComponent<Translation>(anotherBee).Value,translation.Value);
                        if(distance<minDistance) {
                            minDistance = distance;
                            enemyBee = anotherBee;
                        }
                    }
                    // add Attacking component 
                    if(enemyBee != Entity.Null) {
                        commandBuffer.AddComponent(entity,new Attacking() { TargetBee = enemyBee });
                    }                    
                }
            }).Run();
        
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
