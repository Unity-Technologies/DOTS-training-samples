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

        GenerateTrainTracksBezierData(entity, metroComponent);

        SetupPlatforms(entity, metroComponent);
        SetupCommuters(entity, metroComponent);
        SetupTrains(entity, metroComponent);

        Object.DestroyImmediate(Metro.GetRoot);
    }

    void GenerateTrainTracksBezierData(Entity entity, Metro metroComponent)
    {
        Debug.Log("GenerateTrainTracksBezierData");
    }

    void SetupTrains(Entity entity, Metro metro)
    {
        Debug.Log("SetupTrains");

        DstEntityManager.AddComponentData(entity, new SpawnTrains{  prefab = GetPrimaryEntity(metro.prefab_trainCarriage)});
    }

    void SetupCommuters(Entity entity, Metro metro)
    {
        Debug.Log("SetupCommuters");

        var lookup = PathLookupHelper.CreatePathLookup(Pathfinding.GetAllPlatformsInScene());
        DstEntityManager.AddComponentData(entity, new SpawnCommuters{ numberOfCommuters = metro.maxCommuters, prefab = GetPrimaryEntity(metro.prefab_commuter)});
        DstEntityManager.AddComponentData(entity, new PathLookup { value = lookup });
    }

    void SetupPlatforms(Entity entity, Metro metro)
    {
        Debug.Log("SetupPlatforms");

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
            DstEntityManager.AddComponentData(entity, new PlatformTransforms{ value = blob });
            DstEntityManager.AddComponentData(entity, new SpawnPlatforms{ platformPrefab = GetPrimaryEntity(metro.prefab_platform)});
        }
    }
}
