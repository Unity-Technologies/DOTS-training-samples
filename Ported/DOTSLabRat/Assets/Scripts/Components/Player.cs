using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRATS
{
    public struct Player : IComponentData
    {
        public int playerNumber;
        public UnityEngine.Color color;
        public int score;
        public int2 arrowToPlace;
        public Direction arrowDirection;
        
        // AI players only
        public double nextArrowTime;
        public float2 arrowPlacementDelayRange;
    }
}