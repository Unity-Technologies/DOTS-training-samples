using Unity.Entities;

namespace src
{
    public struct LinePosition : IComponentData
    {
        public Entity Line;
        public int CurrentIndex;
        public float Progression;
    }
}