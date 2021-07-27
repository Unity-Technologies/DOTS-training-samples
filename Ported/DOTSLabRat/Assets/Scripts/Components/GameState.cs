using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    public enum GamePhase
    {
        Wait = 0,
        Play = 1,
        End = 2
    }

    [GenerateAuthoringComponent]
    public struct GameState : IComponentData
    {
        public int boardSize;
        public float timer;
        public GamePhase gamePhase;
        public float2 arrowPlacementRate;
        public int maxWalls;
        // BoardArray
    }
}
