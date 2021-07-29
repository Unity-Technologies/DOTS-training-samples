using System;
using Unity.Entities;

[Serializable]
public struct BotActionPickEmptyBucket : IComponentData
{
    public bool ActionDone;
}
