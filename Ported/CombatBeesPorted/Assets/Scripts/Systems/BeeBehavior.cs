using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class BeeBehavior: SystemBase
{
    private EntityQueryDesc allBeesQueryDesc; 
    protected override void OnCreate()
        {
            allBeesQueryDesc = new EntityQueryDesc
            {
                All = new ComponentType[] {typeof(Bee)} //, ComponentType.ReadOnly<WorldRenderBounds>()
            };
        }

    protected override void OnUpdate()
    {

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));                

        EntityQuery allBeesQuery = GetEntityQuery(allBeesQueryDesc);
        NativeArray<Entity> allBees = allBeesQuery.ToEntityArray(Allocator.Temp);

        Entities
            .WithNone<GoingForFood,Attacking,BringingFoodBack>()
            .ForEach((in Entity beeEntity, in Bee bee, in Translation translation, in Team team) =>
            {
                if(random.NextBool()) {
                    // go for food                    

                } else {
                    var enemyTeam = !team.index;
                    // go for attack
                    // find nearest enemy bee
                    float minDistance = 0xFFFFFF;
                    Entity enemyBee = Entity.Null;
                    foreach(Entity anotherBee in allBees) {
                        if( GetComponent<Team>(anotherBee).index != enemyTeam) continue;
                        float distance = math.distance(GetComponent<Translation>(anotherBee).Value,translation.Value);
                        if(distance<minDistance) {
                            minDistance = distance;
                            enemyBee = anotherBee;
                        }
                    }
                    // add Attacking component 
                    if(enemyBee != Entity.Null) {
                        commandBuffer.AddComponent(beeEntity,new Attacking() { TargetBee = enemyBee });
                    }                    
                }
            }).Run();
        
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
