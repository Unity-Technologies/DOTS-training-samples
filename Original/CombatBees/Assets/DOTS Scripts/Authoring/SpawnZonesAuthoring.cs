using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnZonesAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public AABB LevelBounds;
    public AABB Team1Zone;
    public AABB Team2Zone;
    
    public GameObject BeeTeam1Prefab;
    public GameObject BeeTeam2Prefab;
    public GameObject FoodPrefab;
    public GameObject BloodPrefab;
    
    [Range(0, 100)] public int BeesPerFood;
    [Range(0, 1000)] public float FlightJitter;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (BeeTeam1Prefab == null)
        {
            return;
        }

        if (BeeTeam2Prefab == null)
        {
            return;
        }
        dstManager.AddComponentData(entity, new SpawnZones()
        {
            LevelBounds = LevelBounds,
            Team1Zone = Team1Zone,
            Team2Zone = Team2Zone,
            BeeTeam1Prefab = conversionSystem.GetPrimaryEntity(BeeTeam1Prefab),
            BeeTeam2Prefab = conversionSystem.GetPrimaryEntity(BeeTeam2Prefab),
            FoodPrefab = conversionSystem.GetPrimaryEntity(FoodPrefab),
            BloodPrefab = conversionSystem.GetPrimaryEntity(BloodPrefab),
            BeesPerFood = BeesPerFood,
            FlightJitter = FlightJitter,
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BeeTeam1Prefab);
        referencedPrefabs.Add(BeeTeam2Prefab);
        referencedPrefabs.Add(FoodPrefab);
        referencedPrefabs.Add(BloodPrefab);
    }
}
