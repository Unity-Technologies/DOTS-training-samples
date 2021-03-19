using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BeeAttacking: SystemBase
{
    private const float beeAttackChaseDistance = 10;
    EndSimulationEntityCommandBufferSystem endSim;    

    protected override void OnCreate()
    {
        base.OnCreate();
        endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = endSim.CreateCommandBuffer();
        //endSim.AddJobHandleForProducer(Dependency);
        //.WithDisposeOnCompletion(commandBuffer)
        var deltaTime = Time.DeltaTime;
        
        //var vector = random.NextFloat3Direction();
        var random = new Unity.Mathematics.Random(1 + (uint)(Time.ElapsedTime*10000));
        Entities
            .WithAll<Bee>()
            .ForEach((Entity beeEntity, ref Force force, in Translation position, in Attacking beeAttacking) =>
            {                   
                //check beeAttacking is still a valid entity and have a bee component
                if(HasComponent<Bee>(beeAttacking.TargetBee)) {                    
                    var delta = GetComponent<Translation>(beeAttacking.TargetBee).Value - position.Value; 
                    var deltaLength = math.length(delta);
                    force.Value += math.normalize(delta);
                    if ( deltaLength< beeAttackChaseDistance)
                    {
                        force.Value += (1-deltaLength/beeAttackChaseDistance) * math.normalize(delta)*5f;
                    }
                    if( deltaLength < 1 ) {
                        // remove PossessedBy
                        if(HasComponent<BringingFoodBack>(beeAttacking.TargetBee)) {
                            var food = GetComponent<FoodTarget>(beeAttacking.TargetBee).Value;
                            if (HasComponent<PossessedBy>(food))
                            {
                                commandBuffer.RemoveComponent<PossessedBy>(food);
                                commandBuffer.SetComponent(food, GetComponent<Velocity>(beeAttacking.TargetBee));
                            }
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
                        commandBuffer.SetComponent<Force>(beeAttacking.TargetBee,new Force(){Value = force.Value*15f});

                    }
                } else {
                    // dead or entity is invalid
                    commandBuffer.RemoveComponent<Attacking>(beeEntity);                    
                }
            }).Schedule();

        endSim.AddJobHandleForProducer(Dependency);
    }
}
