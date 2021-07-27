using Unity.Entities;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct Player : IComponentData
    {
        public int playerNumber;
        public UnityEngine.Color color;
        public int score;
    }
}