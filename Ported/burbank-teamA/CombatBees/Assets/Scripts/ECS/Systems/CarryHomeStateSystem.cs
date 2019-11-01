using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CarryHomeStateSystem : JobComponentSystem
{
    EntityQuery hivesQuery;
    EndSimulationEntityCommandBufferSystem buffer;

    protected override void OnCreate()
    {
        buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        hivesQuery = GetEntityQuery(ComponentType.ReadOnly<Team>(), ComponentType.Exclude<State>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var commonBuffer = buffer.CreateCommandBuffer().ToConcurrent();
        
        var dropDistance = GetSingleton<DropDistance>().Value;
        var carryHomeVelocity = GetSingleton<CarryHomeVelocity>().Value;
        
        var translations = GetComponentDataFromEntity<Translation>(true);
        var teams = GetComponentDataFromEntity<Team>(true);
        
        var hives = hivesQuery.ToEntityArray(Allocator.TempJob);

        var handle = Entities
            .WithReadOnly(translations)
            .WithReadOnly(teams)
            .WithDeallocateOnJobCompletion(hives)
            .ForEach(
            (Entity entity, ref TargetVelocity targetVelocity, ref State state, ref TargetEntity targetEntity, ref CollectedEntity collected, in Team team) =>
            {
                if (state.Value != State.StateType.Dropping) return;

                targetVelocity.Value = carryHomeVelocity;

                //targetEntity.Value = hives[team.Value - 1];
                for (var i = 0; i < hives.Length; i++)
                {
                    var hive = hives[i];
                    var hiveTeam = teams[hive];
                    if (hiveTeam.Value == team.Value)
                        targetEntity.Value = hive;
                }
                
                var myTranslation = translations[entity]; 
                var targetTranslation = translations[targetEntity.Value];
                var distance = math.distance(myTranslation.Value, targetTranslation.Value); 

                commonBuffer.SetComponent(0, targetEntity.Value, new Translation { Value = new float3(targetTranslation.Value.x, myTranslation.Value.y, myTranslation.Value.z) });


                if (distance > dropDistance) return;
                
                commonBuffer.RemoveComponent<LocalToParent>(0, collected.Value);
                commonBuffer.RemoveComponent<Parent>(0, collected.Value);
                commonBuffer.SetComponent(0, collected.Value, new GravityMultiplier {Value = 1});
                commonBuffer.SetComponent(0, collected.Value, new Translation { Value = myTranslation.Value });

                state.Value = State.StateType.Idle;
                collected.Value = Entity.Null;
                
            }).Schedule(inputDependencies);
        buffer.AddJobHandleForProducer(handle);
        return handle;
    }
}