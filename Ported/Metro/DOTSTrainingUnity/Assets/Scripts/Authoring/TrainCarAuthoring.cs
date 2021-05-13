using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TrainCarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int trainCarIndex = 0;
    public UnityEngine.Color color;
    public GameObject theTrainEngine;
    public GameObject doorLeft;
    public GameObject doorRight;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponents(entity, new ComponentTypes(typeof(TrainCarIndex), typeof(TrainEngineRef), typeof(Color)));
        dstManager.SetComponentData(entity, new TrainCarIndex(){value = trainCarIndex});
        
        Entity trainEntity = conversionSystem.GetPrimaryEntity(theTrainEngine);
        dstManager.SetComponentData(entity, new TrainEngineRef(){value = trainEntity});
        
        dstManager.SetComponentData(entity, new Color() {value = new float4(color.r, color.g, color.b, color.a)});
        dstManager.AddComponentData(entity, new DoorsRef() { doorEntLeft = conversionSystem.GetPrimaryEntity(doorLeft), doorEntRight = conversionSystem.GetPrimaryEntity(doorRight)});
    }
}
