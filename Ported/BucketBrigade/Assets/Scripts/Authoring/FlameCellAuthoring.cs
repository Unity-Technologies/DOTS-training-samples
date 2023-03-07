using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Color = Components.Color;

public class FlameCellAuthoring : MonoBehaviour
{
    class Baker : Baker<FlameCellAuthoring>
    {
        public override void Bake(FlameCellAuthoring cellAuthoring)
        {
            AddComponent<FlameCell>();
        }
    }
}

public struct FlameCell : IComponentData
{
    
}

