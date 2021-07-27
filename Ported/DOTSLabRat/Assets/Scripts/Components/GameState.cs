using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct GameState : IComponentData
    {
        public int boardSize;
        public float timer;
        public float2 arrowPlacementRate;
        // BoardArray
    }
}
