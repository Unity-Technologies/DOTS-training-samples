using Unity.Entities;

namespace Components
{
    public struct Dead : IComponentData, IEnableableComponent
    {
        public float DeathTimer;
    }
}