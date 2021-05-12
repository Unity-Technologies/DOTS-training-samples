using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float totalDistance;
    public float maxSpeed;
    public GameObject[] doors;
    public float targetDistance = 0;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TrackIndex());
        dstManager.AddComponentData(entity, new TrainWaitTimer());
        dstManager.AddComponentData(entity, new TrainCurrDistance());
        dstManager.AddComponentData(entity, new TrainTargetDistance());
        dstManager.AddComponentData(entity, new TrainTotalDistance() { value = totalDistance });
        dstManager.AddComponentData(entity, new TrainCurrSpeed());
        dstManager.AddComponentData(entity, new TrainMaxSpeed() { value = maxSpeed });
        dstManager.AddComponentData(entity, new TrainState() { value = CurrTrainState.Waiting });
        dstManager.AddBuffer<DoorEntities>(entity);
        var doorBuffer = dstManager.GetBuffer<DoorEntities>(entity);

        int numDoors = doors.Length;
        for(int dIdx=0; dIdx<numDoors; ++dIdx)
        {
            var ent = conversionSystem.GetPrimaryEntity(doors[dIdx]);
            doorBuffer.Add(ent);
        }
        
        dstManager.SetComponentData(entity, new TrainTargetDistance(){value = targetDistance});
        
    }
}
