using Unity.Collections;
using Unity.Entities;

public class PheromoneAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public int mapSize = 128;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buf = dstManager.AddBuffer<Pheromone>(entity);
        for (int i = 0; i < mapSize * mapSize; i++)
        {
            buf.Add(new Pheromone
            {
                Value = 0
            });
        }
        
        dstManager.AddComponent<PheromoneMap>(entity);
    }
}