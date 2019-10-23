using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class LevelConversion : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        // EntityManager -> In CONVERSION world: will not be saved, stored or serialized
        // DstEntityManager -> In destination world: what will be serialized into the subscene

        Entities.ForEach<Metro>(Convert);
    }

    void Convert(Entity entity, Metro metroComponent)
    {
        Debug.Log("Start Metro Conversion");

        metroComponent.SetupMetroLines();

        GeneratePathfindingData(entity, metroComponent);
        GenerateTrainTracksBezierData(entity, metroComponent);

        SetupPlatforms(metroComponent);

        Object.DestroyImmediate(Metro.GetRoot);
    }

    static void GeneratePathfindingData(Entity entity, Metro metroComponent)
    {
        Debug.Log("GeneratePathfindingData");
        var lookup = PathLookupHelper.CreatePathLookup(Pathfinding.GetAllPlatformsInScene());
    }

    void GenerateTrainTracksBezierData(Entity entity, Metro metroComponent)
    {
        Debug.Log("GenerateTrainTracksBezierData");
    }

    void SetupPlatforms(Metro metro)
    {
        Debug.Log("SetupPlatforms");
        var singleton = CreateAdditionalEntity(metro.transform);

        var platforms = Pathfinding.GetAllPlatformsInScene().OrderBy(i => i.platformIndex).ToList();
        using (var blobBuilder = new BlobBuilder(Allocator.Temp))
        {
            ref var builderRoot = ref blobBuilder.ConstructRoot<PlatformsTrBlob>();
            var translationBuilder = blobBuilder.Allocate(ref builderRoot.translations, platforms.Count);
            var rotationsBuilder = blobBuilder.Allocate(ref builderRoot.rotations, platforms.Count);

            for (var i = 0; i < platforms.Count; i++)
            {
                var p = platforms[i].transform;
                translationBuilder[i] = p.position;
                rotationsBuilder[i] = p.rotation;
            }

            var blob = blobBuilder.CreateBlobAssetReference<PlatformsTrBlob>(Allocator.Persistent);
            DstEntityManager.AddComponentData(singleton, new PlatformTransforms{ value = blob });
            DstEntityManager.AddComponentData(singleton, new SpawnPlatforms{ platformPrefab = GetPrimaryEntity(metro.prefab_platform)});
        }
    }
}
