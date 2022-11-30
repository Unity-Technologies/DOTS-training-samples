using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    partial struct TrainPositionsCopy : IJobEntity
    {
        public NativeParallelHashMap<int, float3> positions;
        public NativeParallelHashMap<int, quaternion> rotations;

        public void Execute(in TrainPositionCopyAspect train)
        {
            positions[train.ID] = train.Position;
            rotations[train.ID] = train.Rotation;
        }
    }

    [BurstCompile]
    partial struct CopyPositions : IJobParallelFor
    {
        [ReadOnly] public NativeParallelHashMap<int, float3> positions;
        [ReadOnly] public NativeParallelHashMap<int, quaternion> rotations;

        public TrainPositions TrainPositions;

        public void Execute(int index)
        {
            TrainPositions.TrainsPositions[index] = positions[index];
            TrainPositions.TrainsRotations[index] = rotations[index];
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(TrainSpawnerSystem))]
    public partial struct TrainPositionCopySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Train>();
            state.RequireForUpdate<TrainPositions>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var trainPositions = SystemAPI.GetSingleton<TrainPositions>();
            if (trainPositions.TrainsPositions.Length == 0)
                return;

            foreach (var (train,entity) in SystemAPI.Query<Train>().WithEntityAccess())
            {
                var transform = SystemAPI.GetComponent<WorldTransform>(entity);
                trainPositions.TrainsPositions[train.UniqueTrainID] = transform.Position;
                trainPositions.TrainsRotations[train.UniqueTrainID] = transform.Rotation;
            }
            
            /*var tempPositions = new NativeParallelHashMap<int, float3>(trainPositions.TrainsPositions.Length, Allocator.TempJob);
            var tempRotations = new NativeParallelHashMap<int, quaternion>(trainPositions.TrainsPositions.Length, Allocator.TempJob);

            var copyTrainPositions = new TrainPositionsCopy
            {
                positions = tempPositions,
                rotations = tempRotations
            };
            var copyArrays = new CopyPositions
            {
                positions = tempPositions,
                rotations = tempRotations,
                TrainPositions = trainPositions
            };

            var copyTransforms = copyTrainPositions.Schedule(state.Dependency);
            copyArrays.Schedule(trainPositions.TrainsPositions.Length, 10, copyTransforms).Complete();

            tempPositions.Dispose();
            tempRotations.Dispose();*/
        }
    }
}