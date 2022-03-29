using System.Collections.Generic;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;


namespace Sample.Authoring
{
    public class GenerationParametersAuthoring : UnityMonoBehaviour
        , IConvertGameObjectToEntity
        , IDeclareReferencedPrefabs
    {
        [UnityRange(0, 10000)] public int particleCount = 1000;
        public float3 minParticleSpawnPosition = new float3(-50.0f, 0.0f, -50.0f);
        public float3 maxParticleSpawnPosition = new float3(50.0f, 50.0f, 50.0f);
        public UnityGameObject particlePrefab;
        [UnityRange(0.1f, 1.0f)] public float minParticleScale = 0.2f;
        [UnityRange(0.1f, 2.0f)] public float maxParticleScale = 0.7f;
        [UnityRange(0.0f, 1.0f)] public float minColorMultiplier = 0.3f;
        [UnityRange(0.0f, 1.0f)] public float maxColorMultiplier = 0.7f;

        public int cubeSize;
        public UnityGameObject barPrefab;


        // This function is required by IDeclareReferencedPrefabs
        public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
        {
            // Conversion only converts the GameObjects in the scene.
            // This function allows us to inject extra GameObjects,
            // in this case prefabs that live in the assets folder.
            referencedPrefabs.Add(particlePrefab);
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
                particleCount =  particleCount,
                minParticleSpawnPosition =  minParticleSpawnPosition,
                maxParticleSpawnPosition =  maxParticleSpawnPosition,
                particlePrefab = conversionSystem.GetPrimaryEntity(particlePrefab),
                minParticleScale =  minParticleScale,
                maxParticleScale =  maxParticleScale,
                minColorMultiplier = minColorMultiplier,
                maxColorMultiplier = maxColorMultiplier,
                barPrefab = conversionSystem.GetPrimaryEntity(barPrefab),
                cubeSize = cubeSize,
            });
        }
    }
}