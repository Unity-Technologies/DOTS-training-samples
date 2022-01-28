using Unity.Entities;
using Random = Unity.Mathematics.Random;

[GenerateAuthoringComponent]
public struct BloodSpawningProperties : IComponentData
{
    public Entity bloodEntity;
    public int amountParticles;
}

