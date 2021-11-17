using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Globals : IComponentData
{
    public Entity FoodPrefab;
    public Entity GibletPrefab;
    public int StartingFoodCount;
}
