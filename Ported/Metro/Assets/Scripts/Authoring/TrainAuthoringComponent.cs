using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private int trainId = 0;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<TrainComponent>(entity);
        dstManager.AddComponentData(entity, 
            new TrainStateComponent { Value = TrainNavigationSystem.TrainState.Accelerating });
        dstManager.AddSharedComponentData(entity, new TrainIDComponent() { Value = trainId });
        dstManager.AddComponent<SpeedComponent>(entity);
        
        dstManager.AddComponent<TrackPositionComponent>(entity);
        dstManager.AddComponent<WaypointIndexComponent>(entity);
    }
}
