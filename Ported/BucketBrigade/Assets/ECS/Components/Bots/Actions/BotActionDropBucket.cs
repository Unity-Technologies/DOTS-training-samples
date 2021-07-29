using System;
using Unity.Entities;

[Serializable]
public struct BotActionDropBucket : IComponentData
{
    public bool ActionDone;
}
