using System;
using Unity.Entities;

[Serializable]
public struct BotActionFindBucket : IComponentData
{
    public bool ActionDone;
}