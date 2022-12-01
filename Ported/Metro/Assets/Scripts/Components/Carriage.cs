using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;
using UnityEngine.SocialPlatforms;

public struct Carriage : IComponentData
{
    public int Index;

    public Entity Train;
    public int uniqueTrainID;
}