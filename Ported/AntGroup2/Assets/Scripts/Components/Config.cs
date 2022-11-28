using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct Config : IComponentData
{
    public Entity AntPrefab;

    public int TotalAmountOfAnts;
}
