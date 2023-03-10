using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace HighwayRacers
{
    [BurstCompile]
    [UpdateAfter(typeof(CarMoveSystem))]
    public partial struct LaneChangeSystem : ISystem
    {
        // We need type handles to access a chunk's
        // component arrays and entity ID array.
        // It's generally good practice to cache queries and type handles
        // rather than re-retrieving them every update.
        private EntityQuery carChangeLangeStateQuery;
        private ComponentTypeHandle<LaneChangeState> changeLaneStateHandle;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp);
            builder.WithAll<LaneChangeState>();
            carChangeLangeStateQuery = state.GetEntityQuery(builder);
            changeLaneStateHandle = state.GetComponentTypeHandle<LaneChangeState>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var query = SystemAPI.QueryBuilder().WithAll<CarPosition>().Build();
            // get copy of all the CarPosition values from the query
            NativeArray<CarPosition> carPositions = query.ToComponentDataArray<CarPosition>(state.WorldUpdateAllocator);
            var sortJob = carPositions.SortJob(new SortByDistance());
            JobHandle sortHandle = sortJob.Schedule();

            // Type handles must be updated before use in each update.
            changeLaneStateHandle.Update(ref state);
            var testJob = new LaneChangeRequestJob
            {
                config = config,
                carPositions = carPositions,
                changeUpAlternator = UnityEngine.Time.frameCount % 2 == 1
            };
            JobHandle jobHandle = testJob.ScheduleParallel(JobHandle.CombineDependencies(state.Dependency, sortHandle));
            state.Dependency = jobHandle;
    
        /*
			CarChangeLaneSystem
			Query all cars wanting to change lanes (ChangeLaneState).
			If possible to change lanes,
			    DISABLE 'ChangeLaneState'
				ENABLE 'ChangingLaneState'
				create a "ghost" car in target lane
		*/
        }
    }
}