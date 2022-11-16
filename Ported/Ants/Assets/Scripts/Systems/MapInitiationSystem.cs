using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial class MapInitiationSystem : SystemBase
{
    private const float DECAY_SPEED = 30f;
    private float decayCounter;
    
    private MapTextureManager quadSystem;
    protected override void OnCreate()
    {
        var totalCells = MapTextureManager.MAP_WIDTH * MapTextureManager.MAP_HEIGHT;
        NativeArray<TileType> tileTypes = new NativeArray<TileType>(totalCells, Allocator.Persistent);
        NativeArray<byte> strengthAtIndexMap = new NativeArray<byte>(totalCells, Allocator.Persistent);
        for (int i = 0; i < totalCells; i++)
        {
            tileTypes[i] = TileType.Road;
            strengthAtIndexMap[i] = 0;
        }

        var gridEntity = EntityManager.CreateEntity();

        
        EntityManager.AddComponentData(gridEntity, new MapData
        {
            Rows = MapTextureManager.MAP_WIDTH,
            Columns = MapTextureManager.MAP_HEIGHT,
            TileTypes = tileTypes,
            StrengthList = strengthAtIndexMap
        });
    }

    protected override void OnDestroy()
    {
        MapData mapData = SystemAPI.GetSingleton<MapData>();
        mapData.TileTypes.Dispose();
        mapData.StrengthList.Dispose();
    }

    protected override void OnUpdate()
    {
        if (quadSystem == null)
        {
            quadSystem = GameObject.Find("PlayArea").GetComponent<MapTextureManager>();
        }

        if (quadSystem == null) return;
        
        MapDataAspect mapAspect = default;
        foreach (MapDataAspect mapDataAspect in SystemAPI.Query<MapDataAspect>())
        {
            mapAspect = mapDataAspect;
            break;
        }
        
        /*
        Job.WithCode(() =>
        {
            for (int i = 0; i < mapData.StrengthMap.Length; i++)
            {
                mapData.StrengthMap[i] = 255;
            }
        }).Run();
        */
        
        if (decayCounter < 1f)
        {
            decayCounter += DECAY_SPEED * UnityEngine.Time.deltaTime;
            return;
        }

        decayCounter = 0;
        
        /*
        var rand = Random.CreateFromIndex((uint)UnityEngine.Time.frameCount);
        
        for (int i = 0; i < mapAspect.MapData.ValueRO.StrengthList.Length; i++)
        {
            var currentValue = mapAspect.MapData.ValueRW.StrengthList[i] - 1;
            var nextValue = (byte) math.max(currentValue, 0);
            mapAspect.MapData.ValueRW.StrengthList[i] = nextValue;
        }
        */
        
        Job.WithCode(() =>
        {
            //var rand = Random.CreateFromIndex((uint)UnityEngine.Time.frameCount);
        
            for (int i = 0; i < mapAspect.MapData.ValueRO.StrengthList.Length; i++)
            {
                var currentValue = mapAspect.MapData.ValueRW.StrengthList[i] - 1;
                var nextValue = (byte) math.max(currentValue, 0);
                mapAspect.MapData.ValueRW.StrengthList[i] = nextValue;
            }
        }).Run();
        
        quadSystem.SetTextureData(mapAspect.MapData.ValueRO.StrengthList);
    }
}
