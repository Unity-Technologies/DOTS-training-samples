using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(4)]
struct ConstrainedPoint : IBufferElementData
{
    float3 position;
    int neightbours;
}

struct ConstrainedBar : IComponentData
{
    int p1Index, p2Index;
    float lenght;
    Color color;
}

Entity {
DynamicBuffer<ConstrainedPoint> points;
dynamicbuffer<ConstrainedBar> bars;
}



struct FreeBar : IComponentData
{
    float3 p1;
    float3 p2;
    float lenght;
    Color color;
}

float3[] freePoinst;

struct FreeBar : IComponentData
{
    float lenght;
    Color color;
}

FreeBar[] freeBars;


class TornadoSystem : JobComponentSystem
{
    int anchorsEndIndex;

    
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {

        Entities.WithName().
        return inputDeps;
    }
}