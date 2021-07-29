using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Player : IComponentData
    {
        public int playerNumber;
        public UnityEngine.Color color;
        public int score;
        public int2 arrowToPlace;
        public Direction arrowDirection;
        public int currentArrow;
        public int2 arrowToRemove;
    }
}