using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct DoorMovement : IComponentData
{
    public bool LeftDoor;
    public float Progress;
    public bool LeftTrainSide;
}
