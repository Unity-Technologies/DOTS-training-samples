using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TrainReference : IComponentData
{
    public int Index;
    public Entity Train;
}
