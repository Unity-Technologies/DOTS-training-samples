using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CatsSpeedRange : IComponentData
{
    public int minSpeed;
    public int maxSpeed;
}
