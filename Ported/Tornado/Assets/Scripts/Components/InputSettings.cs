using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct InputSettings : IComponentData
    {
        public float cameraAcceleration;
        public float tornadoAcceleration;
        public float friction;
    }
}