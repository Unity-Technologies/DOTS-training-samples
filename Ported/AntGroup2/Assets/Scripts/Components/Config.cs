using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct Config : IComponentData
{
    //const config data
    public Entity WallPrefab;
    public Entity AntPrefab;

    public int TotalAmountOfAnts;
}
