using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public int simulationSpeed;
    public int mapSize;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SimulationSpeed()
        {
            Value = simulationSpeed
        });

        dstManager.AddComponentData(entity, new ScreenSize()
        {
            Value = mapSize
        });
    }
}