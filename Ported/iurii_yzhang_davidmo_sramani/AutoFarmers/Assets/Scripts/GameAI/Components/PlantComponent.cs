using Unity.Entities;
using Unity.Mathematics;
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
        public PlantType Type;
    };
}