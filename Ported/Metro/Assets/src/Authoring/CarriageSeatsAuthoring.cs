using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

public class CarriageSeatsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<UnityEngine.GameObject> Items;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buf = dstManager.AddBuffer<CarriageSeat>(entity);

        var shuffledItems = Items.OrderBy(t => System.Guid.NewGuid()).ToList();
        foreach (var item in shuffledItems)
        {
            buf.Add(new CarriageSeat { Value = conversionSystem.GetPrimaryEntity(item) });
        }
    }

    public void DeclareReferencedPrefabs(List<UnityEngine.GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(Items);
    }
}

public struct CarriageSeat : IBufferElementData
{
    public Entity Value;
}