using System;
using Unity.Entities;

[GenerateAuthoringComponent]
[Serializable]
public struct LookForThrowTargetState : IComponentData
{
    public Entity GrabbedEntity;
}
