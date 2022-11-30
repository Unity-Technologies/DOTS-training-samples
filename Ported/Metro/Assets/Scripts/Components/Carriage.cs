using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;
using UnityEngine.SocialPlatforms;

public struct Carriage : IComponentData
{
    public NativeArray<float2> Seats;
    public NativeList<Entity> Passengers;
    public int Index;

    public Entity Train;
    public int uniqueTrainID;
}