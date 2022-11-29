using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [WithAll(typeof(Carriage))]
    partial struct CarriageJob : IJobEntity
    {
        void Execute(ref CarriageAspect carriage)
        {
            var trainTransform = carriage.TrainTransform;
            var trainPosition = trainTransform.Position;
            var direction = trainTransform.Forward();

            carriage.Position = trainPosition + direction * carriage.Width * carriage.Index;
        }
    }

    [BurstCompile]
    [UpdateAfter(typeof(TrainMovementSystem))]
    public partial struct CarriageMovementSystem : ISystem
    {
        // A ComponentLookup provides random access to a component (looking up an entity).
        // We'll use it to extract the world space position and orientation of the spawn point (cannon nozzle).
        //ComponentLookup<WorldTransform> m_WorldTransformFromEntity;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // ComponentLookup structures have to be initialized once.
            // The parameter specifies if the lookups will be read only or if they should allow writes.
            //m_WorldTransformFromEntity = state.GetComponentLookup<WorldTransform>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //m_WorldTransformFromEntity.Update(ref state);


            var CarriageJob = new CarriageJob
            {
                //WorldTransformFromEntity = m_WorldTransformFromEntity
            };
            CarriageJob.ScheduleParallel();
        }
    }
}