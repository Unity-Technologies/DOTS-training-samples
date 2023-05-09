using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Miscellaneous.StateChangeEnableable
{
    public partial struct BurnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.StateChangeEnableable>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new BurnJob {
                rate = 0.0f
            }.ScheduleParallel();
        }
    }

    [WithAll(typeof(Burner))]
    [BurstCompile]
    partial struct BurnJob : IJobEntity
    {
        public float rate;

        void Execute()
        {
            //rate += currentRate;
            if (rate > 1.0f)
                rate = 1.0f;
        }
    }
}
