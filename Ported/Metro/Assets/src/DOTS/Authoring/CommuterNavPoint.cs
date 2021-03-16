using Unity.Entities;
using Unity.Mathematics;

namespace src.DOTS.Authoring
{
    [GenerateAuthoringComponent]
    public class CommuterNavPoint : IComponentData
    {
        public float3 translation;
    }
}