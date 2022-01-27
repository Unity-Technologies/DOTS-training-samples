using Unity.Entities;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    [GenerateAuthoringComponent]
    public struct BloodSpawner : IComponentData
    {
        public Entity bloodEntity;
        public Random random;
        public int amountParticles;
        public float steps;
        public bool animationStarted;
    }
}