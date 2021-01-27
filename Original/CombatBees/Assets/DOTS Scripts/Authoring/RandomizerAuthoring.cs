using Unity.Entities;
using UnityEngine;

public class RandomizerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public uint RandomSeed;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (RandomSeed == 0)
        {
            return;
        }
        dstManager.AddComponentData(entity, new Randomizer
        {
            Random = new Unity.Mathematics.Random(RandomSeed),
        });
    }
}
