using Unity.Entities;

namespace AntPheromones_ECS
{
    public struct FacingAngle : IComponentData
    {
        public float Value;
    }

    public struct Speed : IComponentData
    {
        public float Value;
    }
//    
//    public struct Movement : IComponentData
//    {
//        public float FacingAngle;
//        public float Speed;
//    }
}