using Unity.Entities;
using Random = Unity.Mathematics.Random;


[GenerateAuthoringComponent]
public struct BeeBloodSpawner : IComponentData
{
    public Entity bloodEntity;
    public int amountParticles;
    public float steps;
    public Random random;
}

