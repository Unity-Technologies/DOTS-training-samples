using HighwayRacers;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class CarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [Tooltip("Distance from center of car to the front.")]
    public float distanceToFront = 1;
    [Tooltip("Distance from center of car to the back.")]
    public float distanceToBack = 1;

    public Color defaultColor = Color.gray;
    public Color maxSpeedColor = Color.green;
    public Color minSpeedColor = Color.red;

    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var data = new CarSettings()
        {
            DefaultSpeed = Random.Range(Game.instance.defaultSpeedMin, Game.instance.defaultSpeedMax),
            OvertakePercent = Random.Range(Game.instance.overtakePercentMin, Game.instance.overtakePercentMax),
            LeftMergeDistance = Random.Range(Game.instance.leftMergeDistanceMin, Game.instance.leftMergeDistanceMax),
            MergeSpace = Random.Range(Game.instance.mergeSpaceMin, Game.instance.mergeSpaceMax),
            OvertakeEagerness = Random.Range(Game.instance.overtakeEagernessMin, Game.instance.overtakeEagernessMax),
        };
        dstManager.AddComponentData(entity,data);
        dstManager.AddComponentData(entity,new CarState());
        dstManager.AddComponentData(entity,new ColorComponent());
        dstManager.AddComponentData(entity,new CarStateOnTrack());
        var sharedData = new CarSharedData
        {
            distanceToFront = distanceToFront,
            distanceToBack =  distanceToBack,
            defaultColor = defaultColor,
            maxSpeedColor = maxSpeedColor,
            minSpeedColor = minSpeedColor
        };
        dstManager.AddComponentData(entity,new ProximityData());
        dstManager.AddComponentData(entity,sharedData);
    }
}
