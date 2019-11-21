using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct PheromoneColourRValue : IBufferElementData
    {
        public float Value; 
        
        public static implicit operator PheromoneColourRValue(float rValue) 
            => new PheromoneColourRValue { Value = rValue };
        
        public static implicit operator float(PheromoneColourRValue buffer) 
            => buffer.Value;
    }
}