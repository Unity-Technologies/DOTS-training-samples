using System;
using Unity.Entities;

[Serializable]
public struct BotActionFillBucket : IComponentData
{
    public bool ActionDone;
}
