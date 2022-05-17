using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class WaterAuthoring : MonoBehaviour
{
    public float WaterLevel;
    public float3 Scale;
}

class WaterBaker : Baker<WaterAuthoring>
{
    public override void Bake(WaterAuthoring authoring)
    {
        Water water = new Water();

        water.Scale = authoring.Scale;
        water.WaterLevel = authoring.WaterLevel;

        AddComponent<Water>(water);
    }
}