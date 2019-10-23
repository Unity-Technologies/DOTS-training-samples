using HighwayRacers;
using Unity.Entities;
using UnityEngine;

namespace HighwayRacers.Authoring
{
    [RequiresEntityConversion]
    public class CarAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        static int NextCarId = 1;

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
            dstManager.AddComponentData(entity,new CarID { Value = NextCarId++ });
            dstManager.AddComponentData(entity,new CarState()); // TODO: system should add this
            dstManager.AddComponentData(entity,new ColorComponent()); // TODO: system should add this
            dstManager.AddComponentData(entity,new ProximityData());
        }
    }
}
