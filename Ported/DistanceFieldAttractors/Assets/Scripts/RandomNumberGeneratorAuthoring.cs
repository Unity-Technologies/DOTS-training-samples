using Unity.Entities;
using Unity.Mathematics;

public struct RandomNumberGenerator : IComponentData
{
    public Random rng;
}

public class RandomNumberGeneratorAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public uint seed = 0xDCC49D6C;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var random = new Random();
        random.InitState(seed);

        dstManager.AddComponentData(entity, new RandomNumberGenerator { rng = random });
    }
}
