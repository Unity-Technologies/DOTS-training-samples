using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct DispatcherStopComponent : IComponentData
{
    public float LeaveTime;    
}
