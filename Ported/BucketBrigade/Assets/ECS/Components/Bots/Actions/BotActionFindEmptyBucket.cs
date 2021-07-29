using System;
using Unity.Entities;

[Serializable]
public struct BotActionFindEmptyBucket : IComponentData
{
    public bool ActionDone;
}