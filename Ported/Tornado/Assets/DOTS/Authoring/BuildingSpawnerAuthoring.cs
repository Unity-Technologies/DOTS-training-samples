using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;

namespace Dots
{
    public class BuildingSpawnerAuthoring : MonoBehaviour
        , IConvertGameObjectToEntity
    {
        public Mesh mesh;
        public Material material;

        public int buildingCount = 35;
        public int minHeight = 4;
        public int maxHeight = 11;
        public int debrisCount = 600;
        public float thicknessMin = 0.25f;
        public float thicknessMax = 0.35f;

        // This function is required by IConvertGameObjectToEntity
        public void Convert(Entity entity, EntityManager dstManager
            , GameObjectConversionSystem conversionSystem)
        {
            // GetPrimaryEntity fetches the entity that resulted from the conversion of
            // the given GameObject, but of course this GameObject needs to be part of
            // the conversion, that's why DeclareReferencedPrefabs is important here.
            dstManager.AddComponentData(entity, new BuildingSpawner
            {
                mesh = mesh,
                material = material,
                buildingCount = buildingCount,
                debrisCount = debrisCount,
                minHeight = minHeight,
                maxHeight = maxHeight,
                thicknessMin = thicknessMin,
                thicknessMax = thicknessMax
            });
        }
    }
}
