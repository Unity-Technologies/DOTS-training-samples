using System;
using Unity.Entities;

[Serializable]
public struct BotActionPickBucket : IComponentData
{
    public bool ActionDone;
}
