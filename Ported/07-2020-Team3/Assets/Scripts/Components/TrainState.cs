using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TrainState : IComponentData
{
    public int nextPlatform;
    public bool inbound;
    public Entity currentPlatform;
    public float timeUntilDeparture;
}
