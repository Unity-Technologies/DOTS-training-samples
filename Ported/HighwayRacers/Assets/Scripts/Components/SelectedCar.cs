using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SelectedCar : IComponentData
{
    public Entity Selected;
}
