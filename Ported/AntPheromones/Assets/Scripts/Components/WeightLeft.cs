
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEditor.ShaderGraph.Internal;

    public struct WeightLeft : IComponentData
    {
        public float Weight;
        public float Rads;
        public float Degrees;
    }
    
    public struct WeightRight : IComponentData
    {
        public float Weight;
        public float Rads;
        public float Degrees;
    }
    
    public struct WeightForward : IComponentData
    {
        public float Weight;
        public float Rads;
        public float Degrees;
    }
