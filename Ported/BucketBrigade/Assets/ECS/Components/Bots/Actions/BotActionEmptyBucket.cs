using System;
using Unity.Entities;

[Serializable]
public struct BotActionEmptyBucket : IComponentData
{
    public bool ActionDone;
}
