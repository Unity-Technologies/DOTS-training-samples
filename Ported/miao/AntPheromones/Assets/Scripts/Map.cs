using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct Map : IComponentData
    {
        public const int Width = 128;
        public const float TrailVisibilityModifier = 0.3f;
        public const float TrailDecayRate = 0.95f;

        public float2 ResourcePosition;
        public float2 ColonyPosition; //= new float2(1f, 1f) * Width * 0.5f;
        
        public static bool IsWithinBounds(int positionX, int positionY)
        {
            return positionX >= 0 && positionY >= 0 && positionX < Width && positionY < Width;
        }

        public static bool IsWithinBounds(int2 position)
        {
            return math.any(position < 0) || math.any(position >= Width);
        }
    }
}