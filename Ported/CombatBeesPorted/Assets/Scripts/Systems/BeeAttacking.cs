using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BeeAttacking: SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        //var vector = random.NextFloat3Direction();
        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));
        Entities
            .WithAll<Bee>()
            .WithoutBurst() // TODO remove in final version, keep for reference to random
            .ForEach((Entity beeEntity, ref Force force, in Translation position, in Attacking beeAttacking) =>
            {                   
                //check beeAttacking is still a valid entity and have a bee component
                if(HasComponent<Bee>(beeAttacking.TargetBee)) {                    
                    var delta = GetComponent<Translation>(beeAttacking.TargetBee).Value - position.Value;                
                    force.Value += math.normalize(delta);
                    if(  math.length(delta) < 1 ) {
                        // remove PossessedBy
                        if(HasComponent<BringingFoodBack>(beeEntity)) {
                            var food = GetComponent<FoodTarget>(beeEntity).Value;
                            commandBuffer.RemoveComponent<PossessedBy>(food);
                        }
                        // bee is not attacking go to iddle state
                        commandBuffer.RemoveComponent<Attacking>(beeEntity);

                        // set bee dead
                        commandBuffer.RemoveComponent<Bee>(beeAttacking.TargetBee);
                        commandBuffer.RemoveComponent<BringingFoodBack>(beeAttacking.TargetBee);
                        commandBuffer.RemoveComponent<TeamA>(beeAttacking.TargetBee);
                        commandBuffer.RemoveComponent<TeamB>(beeAttacking.TargetBee);
                        commandBuffer.RemoveComponent<Team>(beeAttacking.TargetBee);
                        commandBuffer.RemoveComponent<GoingForFood>(beeAttacking.TargetBee);
                        commandBuffer.RemoveComponent<Team>(beeAttacking.TargetBee);
                        commandBuffer.RemoveComponent<FoodTarget>(beeAttacking.TargetBee);
                        
                        commandBuffer.AddComponent<Scale>(beeAttacking.TargetBee, new Scale() { Value = 1 });
                        commandBuffer.AddComponent<ShrinkAndDestroy>(beeAttacking.TargetBee, new ShrinkAndDestroy(){ age = -2f, lifetime = 5f});
                        commandBuffer.AddComponent<BloodSpawnConfiguration>(beeAttacking.TargetBee, new BloodSpawnConfiguration() { Count = 10, Direction =  math.normalize(delta)});
                    }
                } else {
                    // dead or entity is invalid
                    commandBuffer.RemoveComponent<Attacking>(beeEntity);                    
                }
            }).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
}
