using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct PheromoneColourRValueBuffer : IBufferElementData
    {
        public float Value; 
        
        public static implicit operator PheromoneColourRValueBuffer(float rValue) 
            => new PheromoneColourRValueBuffer { Value = rValue };
        
        public static implicit operator float(PheromoneColourRValueBuffer buffer) 
            => buffer.Value;
    }
}