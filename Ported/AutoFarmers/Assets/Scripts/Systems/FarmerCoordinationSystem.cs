using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct FarmerCoordinationSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //TODO: Monitor the state of the world and then alter states of available farmers to grow the farm.
        //Instead of the entities themselves figuring out what is closest to them and going towards that, they are assigned targets.
        //They are also assigned FarmerStates alongside the targets
        //May be useful to create a general purpose component of "Targetable" to be attached onto anything you can target
        //The AI here would essentially:
        //- Monitor if anything is ready to be harvested, see whos available and tell them to harvest
        //- If there isn't enough space to grow more land, see whos available and tell them to cut into rocks
        //- Till soil that is ready to be tilled
        //- Whenever a task is done, the farmer will go into IDLE.  IDLE will be read as needing instruction which will be allocated on next loop
        
    }
}