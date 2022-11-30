using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [WithAll(typeof(Carriage))]
    partial struct CopyTrainTransformations : IJobEntity
    {
        [ReadOnly] public ComponentLookup<WorldTransform> WorldTransformFromEntity;
        void Execute(ref CopyTrainCarriageAspect carriage)
        {
            var trainTransform = WorldTransformFromEntity[carriage.Train];
            carriage.TrainPosition = trainTransform.Position;
            carriage.TrainRotation = trainTransform.Rotation;
            carriage.TrainDirection = trainTransform.Forward();
        }
    }
    
    [BurstCompile]
    [WithAll(typeof(Carriage))]
    partial struct CarriageJob : IJobEntity
    {        
        void Execute(ref CarriageAspect carriage)
        {
            carriage.Position = carriage.TrainPosition - carriage.TrainDirection * carriage.Width * carriage.Index;
            carriage.Rotation = carriage.TrainRotation;
        }
    }

    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct CarriageMovementSystem : ISystem
    {
        // A ComponentLookup provides random access to a component (looking up an entity).
        // We'll use it to extract the world space position and orientation of the spawn point (cannon nozzle).
        ComponentLookup<WorldTransform> m_WorldTransformFromEntity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // ComponentLookup structures have to be initialized once.
            // The parameter specifies if the lookups will be read only or if they should allow writes.
            m_WorldTransformFromEntity = state.GetComponentLookup<WorldTransform>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            m_WorldTransformFromEntity.Update(ref state);

            var copyTrainCarriageJob = new CopyTrainTransformations
            {
                WorldTransformFromEntity = m_WorldTransformFromEntity
            };
            var carriageJob = new CarriageJob();
            
            var copyTrainTransformationsHandle = copyTrainCarriageJob.ScheduleParallel(state.Dependency);
            carriageJob.ScheduleParallel(copyTrainTransformationsHandle).Complete();
            
        }
    }
}