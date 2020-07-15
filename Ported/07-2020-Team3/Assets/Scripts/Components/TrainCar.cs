using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TrainCar : IComponentData
{
    public Entity train;
    public int indexInTrain;
}
