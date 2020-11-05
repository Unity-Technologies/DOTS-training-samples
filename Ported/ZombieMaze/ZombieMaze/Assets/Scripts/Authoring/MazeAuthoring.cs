using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class MazeAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int2 tileMazeSize = new int2(70,70);
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
        var zombieSpawnerEntity = conversionSystem.CreateAdditionalEntity(gameObject);

        spawner.Prefab = zombiePrefabEntity;
        dstManager.AddComponentData(zombieSpawnerEntity, zombieSpawner);
        dstManager.AddComponentData(zombieSpawnerEntity, spawner);
        dstManager.AddComponentData(zombieSpawnerEntity, new Random {Value = new Unity.Mathematics.Random(3049)});
        
        //maze spawner
        var mazeSpawner = new MazeSpawner
        {
            OpenStripsWidth = openStripWidth,
            MazeStripsWidth = mazeStripWidth
        };
        var wallPrefabEntity = conversionSystem.GetPrimaryEntity(wallPrefab);
        var mazeSpawnerEntity = conversionSystem.CreateAdditionalEntity(gameObject);

        spawner.Prefab = wallPrefabEntity;
        dstManager.AddComponentData(mazeSpawnerEntity, mazeSpawner);
        dstManager.AddComponentData(mazeSpawnerEntity, spawner);
        dstManager.AddComponentData(mazeSpawnerEntity, new Random {Value = new Unity.Mathematics.Random(2030)});
        var bufferSize = spawner.MazeSize.x + spawner.MazeSize.y * spawner.MazeSize.x;
        var buffer = dstManager.AddBuffer<MapCell>(mazeSpawnerEntity);
        var bufferInitData = new NativeArray<MapCell>(bufferSize, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        var memSetJob = new MemsetNativeArray<MapCell>
        {
            Source = bufferInitData,
            Value = new MapCell
            {
                Value = (byte) (WallBits.Bottom | WallBits.Top | WallBits.Right | WallBits.Left)
            }
        };
        memSetJob.Run(bufferSize);
        buffer.AddRange(bufferInitData);
        bufferInitData.Dispose();

        var mazeSizeEntity = conversionSystem.CreateAdditionalEntity(gameObject);
        dstManager.AddComponentData(mazeSizeEntity, new MazeSize{Value = (int2)tileMazeSize});

        //TODO: spawn capsules
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(tilePrefab);
        referencedPrefabs.Add(zombiePrefab);
        referencedPrefabs.Add(wallPrefab);
    }
}
