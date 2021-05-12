using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TrainCarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int trainCarIndex = 0;
    public UnityEngine.Color color;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TrainCarIndex(){value = trainCarIndex});
        dstManager.AddComponent<TrainEngineRef>(entity);
        dstManager.AddComponentData(entity, new Color() {value = new float4(color.r, color.g, color.b, color.a)});
    }
}
