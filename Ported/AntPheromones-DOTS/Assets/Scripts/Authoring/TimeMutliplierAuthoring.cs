using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TimeMutliplierAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float StartingSimulationSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var timeMulitplier = new TimeMultiplier{SimulationSpeed = StartingSimulationSpeed};

        dstManager.AddComponentData(entity, timeMulitplier);
    }
}
