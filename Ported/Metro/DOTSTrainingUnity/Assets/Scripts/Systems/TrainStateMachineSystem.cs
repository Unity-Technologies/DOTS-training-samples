using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainStateMachineSystem : SystemBase
{
    private NativeList<float> targetDistances;

    protected override void OnCreate()
    {
        base.OnCreate();

        targetDistances = new NativeList<float>(8, Allocator.Persistent);
        targetDistances.Add(30.0f);
        targetDistances.Add(60.0f);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        targetDistances.Dispose();
    }

    protected override void OnUpdate()
    {
        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        float deltaTime = Time.DeltaTime;

        var distances = targetDistances;
        int numDistances = distances.Length;
        float trainWaitTime = 5.0f;

        Entities.ForEach((
            ref TrainState trainState,
            ref TrackIndex trackIndex,
            ref TrainWaitTimer waitTimer,
            ref TrainTargetDistance targetDist,
            in TrainTotalDistance totalDist) =>
        {
            switch (trainState.value)
            {
                case CurrTrainState.Waiting:
                    {
                        waitTimer.value += deltaTime;

                        if (waitTimer.value >= trainWaitTime)
                        {
                            waitTimer.value = 0.0f;

                            // Set a new target distance
                            int trackIndexVal = trackIndex.value;
                            targetDist.value = distances[trackIndexVal];
                            trackIndex.value = (trackIndexVal + 1) % numDistances;

                            trainState.value = CurrTrainState.Moving;
                        }

                        break;
                    }
                case CurrTrainState.Moving:
                    {
                        break;
                    }
            }
        }).Schedule();
    }
}
