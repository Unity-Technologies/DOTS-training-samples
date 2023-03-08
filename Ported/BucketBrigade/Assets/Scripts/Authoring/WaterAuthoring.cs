using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;
using Color = Components.Color;

public class WaterAuthoring : MonoBehaviour
{
    public float volume;
    public float capacity;
    public float waterRemoved;
    
    class Baker : Baker<WaterAuthoring>
    {
        public override void Bake(WaterAuthoring waterAuthoring)
        {
            AddComponent(new Water
                {
                    capacity = waterAuthoring.capacity,
                    waterRemoved = waterAuthoring.waterRemoved
                }
            );
            AddComponent<Volume>();
        }
    }

    public struct Water : IComponentData
    {
        public float capacity;
        public float waterRemoved;
    }

}
