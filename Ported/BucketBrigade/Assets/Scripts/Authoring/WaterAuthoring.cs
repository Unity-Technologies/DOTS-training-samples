using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class WaterAuthoring : MonoBehaviour
{
    public float volume;
    public float capacity;
    public float waterRemoved;

    class Baker : Baker<WaterAuthoring>
    {
        public override void Bake(WaterAuthoring waterAuthoring)
        {
            AddComponent<Water>();
            AddComponent<Volume>();
        }
    }

    public struct Water : IComponentData { }
}
