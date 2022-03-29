using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class TrainAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    // Add fields to your component here. Remember that:
    //
    // * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
    //   running.
    // 
    // * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
    //   must be one of the supported types.
    //
    // For example,
    //    public float scale;
    [SerializeField] private int trainId = 0;
    [SerializeField] private float maxSpeed = .2f;
    

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<TrainComponent>(entity);
        dstManager.AddSharedComponentData(entity, new TrainIDComponent() { Value = trainId });
        dstManager.AddComponentData<SpeedComponent>(entity, new SpeedComponent() { Value = maxSpeed });
        
        dstManager.AddComponent<TrackPositionComponent>(entity);
        dstManager.AddComponent<WaypointIndexComponent>(entity);
    }
}
