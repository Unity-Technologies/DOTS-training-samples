using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

    public partial struct FireSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new FireJob {
                rate = 0.0f
            }.ScheduleParallel();
        }
    }

    [WithAll(typeof(Burner))]
    [BurstCompile]
    partial struct FireJob : IJobEntity
    {
        public float rate;

        void Execute()
        {
            //rate += currentRate;
            if (rate > 1.0f)
                rate = 1.0f;
        }
    }
