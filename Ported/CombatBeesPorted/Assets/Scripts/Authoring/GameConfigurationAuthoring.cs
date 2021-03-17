using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Components
{
    public class GameConfigurationAuthoring: MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject BeeTeamAPrefab;
        public GameObject BeeTeamBPrefab;
        public GameObject FoodPrefab;
        public float HivePosition=40;
        
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(BeeTeamAPrefab);
            referencedPrefabs.Add(BeeTeamBPrefab);
            referencedPrefabs.Add(FoodPrefab);
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new GameConfiguration()
            {
                BeeTeamAPrefab = conversionSystem.GetPrimaryEntity(BeeTeamAPrefab),
                BeeTeamBPrefab = conversionSystem.GetPrimaryEntity(BeeTeamBPrefab),
                FoodPrefab =  conversionSystem.GetPrimaryEntity(FoodPrefab),
                HivePosition = HivePosition
            });
            var teamASpawnArea= conversionSystem.CreateAdditionalEntity(this);
            var nonUniformScale=dstManager.GetComponentData<NonUniformScale>(entity);
            float3 extents = new float3((nonUniformScale.Value.x / 2 - HivePosition)/2, nonUniformScale.Value.y / 2,nonUniformScale.Value.z / 2);
            dstManager.AddComponentData(teamASpawnArea,new SpawnBounds()
            {
                Extents = extents,
                Center = new float3(nonUniformScale.Value.x/2-extents.x,0,0)
            });
            dstManager.AddComponentData(teamASpawnArea,new TeamA());
            
            var teamBSpawnArea= conversionSystem.CreateAdditionalEntity(this);
            dstManager.AddComponentData(teamBSpawnArea,new SpawnBounds()
            {
                Extents = extents,
                Center = new float3((nonUniformScale.Value.x/2-extents.x)*-1,0,0)
            });
            dstManager.AddComponentData(teamBSpawnArea,new TeamB());
        }

        
    }
}