using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[DisableAutoCreation]
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
        
        var translation = GetComponentDataFromEntity<Translation>(true);
        var team = GetComponentDataFromEntity<Team>(true);
        
        var hives = hivesQuery.ToEntityArray(Allocator.TempJob);

        var handle = Entities/*.WithReadOnly(translation).WithDeallocateOnJobCompletion(hives)*/.ForEach(
            (Entity entity, ref TargetVelocity targetVelocity, ref State state, ref TargetEntity targetEntity) =>
            {
                if (state.Value != State.StateType.Dropping) return;

                targetVelocity.Value = carryHomeVelocity;


                //var translationll = translation[entity];
                /*targetVelocity.Value = collectVelocity;

                var targetTranslation = translationContainer[targetEntity.Value];
                var distance = math.distance(translation.Value, targetTranslation.Value);

                if (distance <= collectDistance)
                {
                    commonBuffer.AddComponent(0, targetEntity.Value, new Parent
                    {
                        Value = entity
                    });
                    
                    commonBuffer.AddComponent<LocalToParent>(0, targetEntity.Value);

                    state.Value = State.StateType.Dropping;
                }*/

            }).Schedule(inputDependencies);
        
        buffer.AddJobHandleForProducer(handle);
        return handle;
    }
}