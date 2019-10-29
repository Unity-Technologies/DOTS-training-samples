using Unity.Entities;

namespace Unity.Entities.CodeGen.Tests.TestTypes
{
    public struct BoidInAnotherAssembly : IComponentData
    {
    }

    public struct TranslationInAnotherAssembly : IComponentData
    {
        public float Value;
    }

    public struct VelocityInAnotherAssembly : IComponentData
    {
        public float Value;
    }

    public struct AccelerationInAnotherAssembly : IComponentData
    {
        public float Value;
    }

    public struct RotationInAnotherAssembly : IComponentData
    {
        public int Value;
    }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
    public class SpeedInAnotherAssembly : IComponentData
    {
        public float Value;
    }
#endif
}