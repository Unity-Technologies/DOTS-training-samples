using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public enum EntityDirection
{
    Right,
    Up,
    Left,
    Down
}

[GenerateAuthoringComponent]
public struct Direction : IComponentData
{
    public EntityDirection Value;
}
// 0->Right
// 1->Up
// 2->Left
// 3->Down