using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using Unity.Mathematics;
using Components;


namespace Authoring
{
    public class GenerationAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
    {
        public float3 minParticleSpawnPosition;
        public float3 maxParticleSpawnPosition;
        public int cubeSize;
        // .. more will come

        public UnityGameObject barPrefab;
    

        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            // Conversion only converts the GameObjects in the scene.
            // This function allows us to inject extra GameObjects,
            // in this case prefabs that live in the assets folder.
            referencedPrefabs.Add(barPrefab);
        }

        // This function is required by IConvertGameObjectToEntity
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            // GetPrimaryEntity fetches the entity that resulted from the conversion of
            // the given GameObject, but of course this GameObject needs to be part of
            // the conversion, that's why DeclareReferencedPrefabs is important here.
            dstManager.AddComponentData(entity, new GenerationParameters
            {
                barPrefab = conversionSystem.GetPrimaryEntity(barPrefab),
                cubeSize = cubeSize,
            
            });
        }
    }
}

