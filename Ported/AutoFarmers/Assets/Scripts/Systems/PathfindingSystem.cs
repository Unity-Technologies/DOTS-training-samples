
using System.Text;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum PathfindingDestination
{
    None=0,
    Silo,
    UntilledGround,
    Rock,
    Plant,
}

public enum NavigatorType
{
    Farmer,
    Drone,
}

[BurstCompile]
partial struct PathfindingSystem : ISystem
{
    private int frame;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ground>();
        state.RequireForUpdate<GameConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (frame++ % 180 != 0)
        {
            return;
        }
        
        // Get game config to get map size
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        
        // Allocator for native arrays to use in job
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;

        // Dylan TODO: logic for determining which enums to pass in (ie. What kind of Navigator (drone or farmer), and What is the destination)
        NavigatorType navigatorType = NavigatorType.Farmer;
        PathfindingDestination destinationType = PathfindingDestination.Plant; 
        
        // Get ground tiles buffer from ground singleton
        BufferFromEntity<GroundTile> groundData = state.GetBufferFromEntity<GroundTile>(true);
        Entity groundEntity = SystemAPI.GetSingletonEntity<Ground>();

        
                
        if (groundData.TryGetBuffer(groundEntity, out DynamicBuffer<GroundTile> groundBuffer))
        {
            // Initialize the job
            var findPathJob = new FindPath
            {
                VisitedTiles = CollectionHelper.CreateNativeArray<int>(gameConfig.MapSize.x * gameConfig.MapSize.y, allocator),
                ActiveTiles = new NativeList<int>(allocator),
                NextTiles = new NativeList<int>(allocator),
                OutputTiles = new NativeList<int>(allocator),

                DirsX = new int4(0, 0, 1, -1),
                DirsY = new int4(1, -1, 0, 0),
                Ground = groundBuffer,
                MapSize = gameConfig.MapSize,
                Range = gameConfig.PathfindingAcquisitionRange,
            };

            // Schedule execution in a single thread, and do not block main thread.
            state.Dependency = findPathJob.Schedule(state.Dependency);

            //var testJob = new printJob();
            //testJob.Schedule();
        }
    }
}

[BurstCompile]
partial struct FindPath : IJobEntity
{
    public int4 DirsX;
    public int4 DirsY;

    public DynamicBuffer<GroundTile> Ground;
    public int2 MapSize;
    public int Range;

    public NativeArray<int> VisitedTiles;
    public NativeList<int> ActiveTiles;
    public NativeList<int> NextTiles;
    public NativeList<int> OutputTiles;

    void Execute(ref PathfindingAspect pathfinder)
    {
        NavigatorType navigatorType = pathfinder.PathfindingIntent.ValueRO.navigatorType;
        PathfindingDestination destinationType = pathfinder.PathfindingIntent.ValueRO.destinationType;
        RectInt requiredZone = pathfinder.PathfindingIntent.ValueRO.RequiredZone;

        if (pathfinder.PathfindingIntent.ValueRO.destinationType == PathfindingDestination.None)
        {
            return;
        }

        int mapWidth = MapSize.x;
        int mapHeight = MapSize.y;
        
        if (!GroundUtilities.TryGetTileCoords(pathfinder.Translation.ValueRO, mapWidth, mapHeight, out int2 start))
        {
            return;
        }
        
        for (int x=0;x<mapWidth;x++) {
            for (int y = 0; y < mapHeight; y++) {
                VisitedTiles[Hash(x,y)] = -1;
            }
        }
        OutputTiles.Clear();
        VisitedTiles[Hash(start.x, start.y)] = 0;
        ActiveTiles.Clear();
        NextTiles.Clear();
        NextTiles.Add(Hash(start.x, start.y));
        
        int steps = 0;
        
        while (NextTiles.Length > 0 && (steps<Range || Range==0)) {
            NativeList<int> temp = ActiveTiles;
            ActiveTiles = NextTiles;
            NextTiles = temp;
            NextTiles.Clear();
        
            steps++;
        
            for (int i=0;i<ActiveTiles.Length;i++) {
                int x, y;
                Unhash(ActiveTiles[i],out x,out y);

                for (int j = 0; j < 4; j++)
                {
                    int x2 = x + DirsX[j];
                    int y2 = y + DirsY[j];

                    if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapHeight)
                    {
                        continue;
                    }

                    int hash = Hash(x2, y2);
                    if (VisitedTiles[hash]==-1 || VisitedTiles[hash]>steps) {
                        
                        if (GetNavigable(navigatorType, hash, Ground, destinationType)) {
                            VisitedTiles[hash] = steps;
                            NextTiles.Add(hash);
                        }
                        if (x2 >= requiredZone.xMin && x2 <= requiredZone.xMax) {
                            if (y2 >= requiredZone.yMin && y2 <= requiredZone.yMax) {
                                if (CheckMatchingTile(destinationType, hash, Ground))
                                {
                                    AssignPathTo(OutputTiles, x2, y2, DirsX, DirsY, MapSize.x, MapSize.y, VisitedTiles);
                                    
                                    // Recreate waypoints with the calculated path
                                    pathfinder.ClearWaypoints();
                                    for (int outputIndex = 0; outputIndex < OutputTiles.Length; ++outputIndex)
                                    {
                                        pathfinder.AddWaypoint(new Waypoint()
                                        {
                                            TileIndex = OutputTiles[outputIndex]
                                        });
                                    }

                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    public void AssignPathTo(NativeList<int> pathTo,int endX, int endY, int4 dirsX, int4 dirsY,
        int mapWidth, int mapHeight, NativeArray<int> visitedTiles) {
        pathTo.Clear();

        int x = endX;
        int y = endY;

        pathTo.Add(Hash(x, y));

        int dist = int.MaxValue;
        while (dist>0) {
            int minNeighborDist = int.MaxValue;
            int bestNewX = x;
            int bestNewY = y;
            for (int i=0;i<4;i++) {
                int x2 = x + dirsX[i];
                int y2 = y + dirsY[i];
                if (x2 < 0 || y2 < 0 || x2 >= mapWidth || y2 >= mapHeight) {
                    continue;
                }

                int newDist = visitedTiles[Hash(x2, y2)];
                if (newDist !=-1 && newDist < minNeighborDist) {
                    minNeighborDist = newDist;
                    bestNewX = x2;
                    bestNewY = y2;
                }
            }
            x = bestNewX;
            y = bestNewY;
            dist = minNeighborDist;
            
            pathTo.Add(Hash(x, y));
        }
    }

    private bool CheckMatchingTile(PathfindingDestination pathfindingDestination, int tileIndex, DynamicBuffer<GroundTile> groundTiles)
    {
        switch (pathfindingDestination)
        {
            case PathfindingDestination.Plant:
                return groundTiles[tileIndex].tileState == GroundTileState.Planted;
            case PathfindingDestination.Rock:
                return groundTiles[tileIndex].tileState == GroundTileState.Unpassable;
            case PathfindingDestination.UntilledGround:
                return groundTiles[tileIndex].tileState == GroundTileState.Open;
            default:
                return false;
        }
    }

    private bool GetNavigable(NavigatorType navigatorType, int tileIndex, DynamicBuffer<GroundTile> groundTiles, PathfindingDestination destination)
    {
        switch (navigatorType)
        {
            case NavigatorType.Farmer:
                return groundTiles[tileIndex].tileState != GroundTileState.Unpassable || destination == PathfindingDestination.Rock;
            default:
                return true;
        }
    }

    private int Hash(int x, int y)
    {
        return MapSize.x * y + x;
    }

    private void Unhash(int index, out int x, out int y)
    {
        y = index / MapSize.x;
        x = index % MapSize.y;
    }
}

// Prints the Waypoints of every farmer
partial struct printJob : IJobEntity
{
    void Execute(in PathfindingAspect pathfinder)
    {
        var waypoints = pathfinder.Waypoints;
        
        StringBuilder sb = new StringBuilder();
        foreach (var waypoint in waypoints)
        {
            sb.Append($"[{waypoint.TileIndex}] ");
        }
    }
}
