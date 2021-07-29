using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

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

        // AI players only
        public double nextArrowTime;
        public float2 arrowPlacementDelayRange;
        public Random random;
    }
}