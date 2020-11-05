using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

using UnityEngine;

[UpdateAfter(typeof(MazeGenerator))]
public class DijkstraGeneratorSystem : SystemBase
{   
    const int kWidth  = 1000;
    const int kHeight = 1000;
    public JobHandle MazeDependency { get; set; }

    protected override void OnCreate()
    {
        // Temp code to create a single map entity for testing.
        var e = EntityManager.CreateEntity(typeof(DijkstraMap));
#if UNITY_EDITOR
        EntityManager.SetName(e, "DijkstraMap");
#endif
        EntityManager.AddBuffer<MapCell>(e);
        EntityManager.AddBuffer<DistCell>(e);
        var map = new DijkstraMap(kWidth, kHeight);
        map.SetOrigin(0, 0);
        EntityManager.SetComponentData(e, map);
    }

    unsafe protected override void OnUpdate()
    {
        Dependency = Entities.ForEach((Entity e, ref DijkstraMap map, ref DynamicBuffer<MapCell> wallMap, ref DynamicBuffer<DistCell> distMap) => 
        {
            var length = map.Width * map.Height;
            if (distMap.Length != length)
                distMap.ResizeUninitialized(length);

// Temp code to init the wall data to no walls.
if (wallMap.Length != length)
{
    wallMap.ResizeUninitialized(length);
    for (var i = 0; i < length; ++i)
        wallMap[i] = new MapCell(){ Value = 0 };  
}
            var positions = new NativeList<int2>(length, Allocator.Temp);

            // Init all distances to max distance. This can be parallelized.
            DistCell* pDistCell = (DistCell*)distMap.GetUnsafePtr();
            for (var i = 0; i < distMap.Length; ++i)
                pDistCell->Value = int.MaxValue;

            var sourceIndex = map.Origin.y * map.Width + map.Origin.x;
            pDistCell[sourceIndex].Value = 0;
            positions.AddNoResize(map.Origin);

            while (positions.Length > 0)
            {
                var last = positions.Length - 1;
                var current = positions[last];
                positions.RemoveAt(last);

                var index = current.y * map.Width + current.x;
                var dist  = 1 + distMap[index].Value;
                var walls = (WallBits)wallMap[index].Value;

                if ((walls & WallBits.Left) == 0)
                    VisitNeighbor(current + Constants.kInt2Left,  dist, ref map, ref wallMap, pDistCell, ref positions);
                if ((walls & WallBits.Bottom) == 0)
                    VisitNeighbor(current + Constants.kInt2Down,  dist, ref map, ref wallMap, pDistCell, ref positions);
                if ((walls &  WallBits.Top) == 0)
                    VisitNeighbor(current + Constants.kInt2Up,    dist, ref map, ref wallMap, pDistCell, ref positions);
                if ((walls & WallBits.Right) == 0)
                    VisitNeighbor(current + Constants.kInt2Right, dist, ref map, ref wallMap, pDistCell, ref positions);
            }

            positions.Dispose();
        })
        .Schedule(JobHandle.CombineDependencies(Dependency, MazeDependency));
    }

    unsafe static void VisitNeighbor(int2 next, int distance, ref DijkstraMap map, ref DynamicBuffer<MapCell> wallMap, DistCell* pDistCell, ref NativeList<int2> positions)
    {
        // If the next position is valid, and hasn't been visited yet.
        if (next.x >= 0 && next.x < map.Width && next.y >= 0 && next.y < map.Height)
        {
            var index = next.y * map.Width + next.x;
            if (pDistCell[index].Value == int.MaxValue)
            {
                pDistCell[index].Value = distance;
                positions.AddNoResize(next);
            }
        }
    }
}
