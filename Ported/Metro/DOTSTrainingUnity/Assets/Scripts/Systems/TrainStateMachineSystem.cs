using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TrainStateMachineSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        float deltaTime = Time.DeltaTime;
        float trainWaitTime = 5.0f;

        var doorState = GetComponentDataFromEntity<DoorState>();

        var stopDistances = Line.allStopPointSubarrays;
        var stopPointSubarrayIndices = Line.stopPointSubarrayIndices;
        var numStopPointsInLine = Line.numStopPointsInLine;

        Entities
            .WithReadOnly(stopDistances)
            .WithReadOnly(stopPointSubarrayIndices)
            .WithReadOnly(numStopPointsInLine)
            .ForEach((
            ref DynamicBuffer <DoorEntities> doorEntBuffer,
            ref TrainState trainState,
            ref PlatformIndex platformIndex,
            ref TrainWaitTimer waitTimer,
            ref TrainTargetDistance targetDist,
            in TrackIndex trackIndex) =>
        {
            switch (trainState.value)
            {
                case CurrTrainState.Arrived:
                    {
                        // TODO: put real logic here
                        int doorSide = platformIndex.value;

                        int numDoors = doorEntBuffer.Length;
                        int halfNumDoors = numDoors / 2;

                        for(int dIdx=0; dIdx < halfNumDoors; ++dIdx)
                        {
                            var currEnt = doorEntBuffer[dIdx + doorSide * halfNumDoors];
                            doorState[currEnt] = new DoorState() { value = CurrentDoorState.Open };
                        }

                        trainState.value = CurrTrainState.Waiting;
                        break;
                    }
                case CurrTrainState.Waiting:
                    {
                        waitTimer.value += deltaTime;

                        if (waitTimer.value >= trainWaitTime)
                        {
                            // Close the doors

                            // TODO: put real logic here
                            int doorSide = platformIndex.value;

                            int numDoors = doorEntBuffer.Length;
                            int halfNumDoors = numDoors / 2;

                            for (int dIdx = 0; dIdx < halfNumDoors; ++dIdx)
                            {
                                var currEnt = doorEntBuffer[dIdx + doorSide * halfNumDoors];
                                doorState[currEnt] = new DoorState() { value = CurrentDoorState.Close };
                            }

                            // Tell the train to move
                            waitTimer.value = 0.0f;

                            // TODO: make an array of the number of stops per line and use that here

                            int startIndex = stopPointSubarrayIndices[trackIndex.value];
                            int numStops = numStopPointsInLine[trackIndex.value];
                            var stopDistancesForLine = stopDistances.GetSubArray(startIndex, numStops);

                            // Set a new target distance
                            int platformIndexVal = platformIndex.value;

                            targetDist.value = stopDistancesForLine[platformIndexVal];

                            platformIndex.value = (platformIndexVal + 1) % numStops;

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
