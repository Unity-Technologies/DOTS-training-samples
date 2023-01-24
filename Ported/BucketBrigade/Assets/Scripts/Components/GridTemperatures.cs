using Unity.Entities;
using UnityEngine;

public struct GridTemperatures : IComponentData
{
    public float[][] temperatures;
}
