using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct TrainPosition : IComponentData
{
    public Entity track;
    public float position;
    public float speed;
}
