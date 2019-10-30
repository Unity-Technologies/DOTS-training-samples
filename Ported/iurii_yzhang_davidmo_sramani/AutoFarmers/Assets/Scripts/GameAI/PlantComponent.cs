using Unity.Entities;
using UnityEngine;

namespace GameAI
{
    public enum PlantType
    {
        SmallPlant,
        LargePlant
    };
    
    
    public struct PlantComponent : IComponentData
    {
        PlantType type;
        Vector2Int position;
    };
}