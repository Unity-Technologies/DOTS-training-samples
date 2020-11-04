using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class MazeAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public uint2 tileMazeSize = new uint2(70,70);
    public uint mazeStripWidth = 6;
    public uint openStripWidth = 4;
    public uint numZombies = 5;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject zombiePrefab;
    public GameObject wallPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //tile spawner
        var tilePrefabEntity = conversionSystem.GetPrimaryEntity(tilePrefab);
        var spawner = new Spawner
        {
            MazeSize =  tileMazeSize,
            Prefab = tilePrefabEntity
        };

        dstManager.AddComponentData(entity, spawner);
        dstManager.AddComponent<TileSpawner>(entity);
        
        //zombie spawner
        var zombieSpawner = new ZombieSpawner
        {
            NumZombies = numZombies
        };
        var zombiePrefabEntity = conversionSystem.GetPrimaryEntity(zombiePrefab);
        var zombieSpawnerEntity = conversionSystem.CreateAdditionalEntity(this.gameObject);

        spawner.Prefab = zombiePrefabEntity;
        dstManager.AddComponentData(zombieSpawnerEntity, zombieSpawner);
        dstManager.AddComponentData(zombieSpawnerEntity, spawner);
        
        //maze spawner
        var mazeSpawner = new MazeSpawner
        {
            OpenStripsWidth = openStripWidth,
            MazeStripsWidth = mazeStripWidth
        };
        var wallPrefabEntity = conversionSystem.GetPrimaryEntity(wallPrefab);
        var mazeSpawnerEntity = conversionSystem.CreateAdditionalEntity(this.gameObject);

        spawner.Prefab = wallPrefabEntity;
        dstManager.AddComponentData(mazeSpawnerEntity, mazeSpawner);
        dstManager.AddComponentData(mazeSpawnerEntity, spawner);   
        
        //TODO: spawn capsules
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(tilePrefab);
        referencedPrefabs.Add(zombiePrefab);
        referencedPrefabs.Add(wallPrefab);
    }
}
