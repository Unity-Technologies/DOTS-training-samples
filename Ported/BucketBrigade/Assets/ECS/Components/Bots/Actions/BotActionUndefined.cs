using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct BotActionUndefined : IComponentData
{
    public bool ActionDone;
}
