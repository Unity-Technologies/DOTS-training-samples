using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct QuadrantData
{
    public Tile tile;
}

public partial struct QuadrantSystem : ISystem
{

    public static float radius;
    public static int yMult;
    public static NativeParallelMultiHashMap<int, QuadrantData> quadrantMultiHashMap;
    
    
    public static int GetPositionHashMapKey(float3 position)
    {
        return (int)(math.floor(position.x / radius) + (yMult * math.floor(position.z / radius)));
    }

    private void DebugDrawQuadrant(float3 position)
    {
        Vector3 lowerLeft = new Vector3(math.floor(position.x / radius) * radius,
            0,math.floor(position.x / radius) * radius);
        Debug.DrawLine(lowerLeft,lowerLeft+new Vector3(+1,+0,+0)*radius);
        Debug.DrawLine(lowerLeft,lowerLeft+new Vector3(+0,+0,+1)*radius);
        Debug.DrawLine(lowerLeft+new Vector3(+1,+0,+0)*radius,lowerLeft+new Vector3(+1,+0,+1)*radius);
        Debug.DrawLine(lowerLeft+new Vector3(+0,+0,+1)*radius,lowerLeft+new Vector3(+1,+0,+1)*radius);
        Debug.Log(GetPositionHashMapKey(position) + " "+ position);
    }

    public int GetTileCountInHashMap(NativeParallelMultiHashMap<int,QuadrantData> tiles, int key)
    {
        int count = 0;
        QuadrantData quadrantData;
        NativeParallelMultiHashMapIterator<int> nativeParallelMultiHashMapIterator;
        if (tiles.TryGetFirstValue(key, out quadrantData, out nativeParallelMultiHashMapIterator))
        {
            
            do
            {
                count++;
            } while (tiles.TryGetNextValue(out quadrantData,ref nativeParallelMultiHashMapIterator));

        }

        return count;

    }

    public void OnCreate(ref SystemState state)
    {
       yMult = 1000;
       
        
       quadrantMultiHashMap = new NativeParallelMultiHashMap<int, QuadrantData>(0,Allocator.Persistent);
    }

    public void OnDestroy(ref SystemState state)
    {
        quadrantMultiHashMap.Dispose();
    }

    public void OnUpdate(ref SystemState state)
    {
        
        var config = SystemAPI.GetSingleton<Config>();
        radius = config.heatRadius;
        
        EntityQuery tileEntities = SystemAPI.QueryBuilder().WithAll<Tile,OnFire>().Build();

        quadrantMultiHashMap.Clear();
        if (tileEntities.CalculateEntityCount() > quadrantMultiHashMap.Capacity)
        {
            quadrantMultiHashMap.Capacity = tileEntities.CalculateEntityCount();
        }

        SetQuadrantHashMapJob setQuadrantHashMapJob = new SetQuadrantHashMapJob
        {
            quadrantMultiHashMap = quadrantMultiHashMap.AsParallelWriter()
        };

        JobHandle jobHandle =  setQuadrantHashMapJob.ScheduleParallel(tileEntities,state.Dependency);
        jobHandle.Complete();

    }
    
    public partial struct SetQuadrantHashMapJob : IJobEntity
    {
        public NativeParallelMultiHashMap<int, QuadrantData>.ParallelWriter quadrantMultiHashMap;
    
        void Execute(LocalTransform localTransform, Tile tile)
        {
            int key = GetPositionHashMapKey(localTransform.Position);
            
            quadrantMultiHashMap.Add(key,new QuadrantData
            {
                tile = tile
            });
        }
    }

}


