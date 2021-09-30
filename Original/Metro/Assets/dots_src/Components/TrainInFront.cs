using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TrainInFront : IComponentData
{
    public Entity Train;
}
