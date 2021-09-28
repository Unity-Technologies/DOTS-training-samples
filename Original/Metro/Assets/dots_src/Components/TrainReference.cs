using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TrainReference : IComponentData
{
    public int Index;
    public Entity Train;
}
