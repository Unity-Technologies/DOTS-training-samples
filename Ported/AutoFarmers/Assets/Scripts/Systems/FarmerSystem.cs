using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[BurstCompile]
partial struct FarmerSystem : ISystem
{
    Unity.Mathematics.Random random;
    EntityQuery _rockQuery;
    EntityTypeHandle _entityTypeHandle;
    ComponentTypeHandle<LocalTransform> _localTransformTypeHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Unity.Mathematics.Random(1234);
        state.RequireForUpdate<WorldGrid>();
        state.RequireForUpdate<Config>();
        _rockQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform,Rock>().Build();
        _entityTypeHandle = SystemAPI.GetEntityTypeHandle();
        _localTransformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>();

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void ChooseNewTask(FarmerAspect farmer, ref SystemState state)
    {
        //Cooldown?
        if((SystemAPI.Time.ElapsedTime - farmer.LastStateChangeTime) > farmer.StateChangeCooldown)
        {
            farmer.LastStateChangeTime = (float)SystemAPI.Time.ElapsedTime;
            farmer.StateChangeCooldown = random.NextFloat(0.5f, 1.0f);
            farmer.FarmerState = (byte)random.NextInt(0, FarmerStates.FARMER_TOTAL_STATES + 1);
        }

        
        //UnityEngine.Debug.Log("Farmer state: " + farmer.FarmerState);
    }

    [BurstCompile]
    public void ChooseSpecificTask(FarmerAspect farmer, byte task, ref SystemState state)
    {
        //Cooldown?
            farmer.LastStateChangeTime = (float)SystemAPI.Time.ElapsedTime;
            farmer.StateChangeCooldown = random.NextFloat(0.5f, 1.0f);
            farmer.FarmerState = task;
        //UnityEngine.Debug.Log("Farmer state: " + farmer.FarmerState);
    }

    //Searches for a specific type within a set range
    #region Search Grid BASIC
    [BurstCompile]
    public int2 SearchGridForType(int type, int2 farmerGridPosition, int searchRange, WorldGrid worldGrid)
    {
        int2 closestPosition = farmerGridPosition;

        int width = worldGrid.gridSize.x;
        int height = worldGrid.gridSize.y;
        int halfRange = searchRange / 2;
        float closestRange = Mathf.Infinity;
        bool foundClosest = false;

        for (int x = -halfRange; x < halfRange; x++)
        {
            for (int y = -halfRange; y < halfRange; y++)
            {
                int2 offset = new int2(x, y);
                int2 positionCheck = farmerGridPosition + offset;
                
                if (positionCheck.x < 0) continue;
                if (positionCheck.y < 0) continue;
                if (positionCheck.x > width-1) continue;
                if (positionCheck.y > height-1) continue;

                if (worldGrid.GetTypeAt(positionCheck) == type)
                {
                    float length = math.lengthsq(offset);
                    if (length < closestRange)
                    {
                        closestPosition = positionCheck;
                        closestRange = length;
                        foundClosest = true;
                    }
                }
            }
        }

        if (foundClosest)
            return closestPosition;
        else
            return -1;
    }
    #endregion

    #region Search Grid OLD
    //[BurstCompile]
    //public int2 SearchGridForType(int type, int2 farmerGridPosition, int searchRange, WorldGrid worldGrid)
    //{
    //    int2 closestPosition = farmerGridPosition;
    //    for (int x = 0; x < searchRange / 2; x++)
    //    {
    //        bool foundClosest = false;
    //        for (int y = 0; y < searchRange/2; y++)
    //        {
    //            int2 right = farmerGridPosition + new int2(x, 0);
    //            int2 left = farmerGridPosition + new int2(-x, 0);
    //            int2 up = farmerGridPosition + new int2(0, y);
    //            int2 down = farmerGridPosition + new int2(0, -y);

    //            int rightType = worldGrid.GetTypeAt(right);
    //            int leftType = worldGrid.GetTypeAt(left);
    //            int upType = worldGrid.GetTypeAt(up);
    //            int downType = worldGrid.GetTypeAt(down);

    //            if (rightType == type)
    //            {
    //                closestPosition = right;
    //                foundClosest = true;
    //                break;
    //            }

    //            if (leftType == type)
    //            {
    //                closestPosition = left;
    //                foundClosest = true;
    //                break;
    //            }

    //            if (upType == type)
    //            {
    //                closestPosition = up;
    //                foundClosest = true;
    //                break;
    //            }

    //            if (downType == type)
    //            {
    //                closestPosition = down;
    //                foundClosest = true;
    //                break;
    //            }
    //        }
    //        if (foundClosest)
    //            return closestPosition;
    //        else
    //            return -1;
    //    }
    //    return -1;
    //}
    #endregion

    #region Spiral Search
    //[BurstCompile]
    //public int2 SearchGridForType(int type, int2 start, int searchRange, WorldGrid worldGrid)
    //{
    //    int[] dx = { 1, 0, -1, 0 };
    //    int[] dy = { 0, 1, 0, -1 };
    //    int x = start.x, y = start.y, d = 0;
    //    int m = worldGrid.gridSize.x, n = worldGrid.gridSize.y;
    //    int[,] visited = new int[m, n];
    //    for (int i = 0; i < m; i++)
    //    {
    //        for (int j = 0; j < n; j++)
    //        {
    //            visited[i, j] = 0;
    //        }
    //    }

    //    while (true)
    //    {
    //        var point = new int2(x, y);
    //        if (worldGrid.GetTypeAt(new int2(x, y)) == type)
    //        {
    //            return point;
    //        }
    //        visited[x, y] = 1;
    //        int a = x + dx[d], b = y + dy[d];
    //        if (a >= 0 && a < m && b >= 0 && b < n && visited[a, b] == 0)
    //        {
    //            x = a;
    //            y = b;
    //        }
    //        else
    //        {
    //            d = (d + 1) % 4;
    //            x += dx[d];
    //            y += dy[d];
    //        }

    //        if (visited.Cast<int>().Sum() == m * n || (Math.Abs(x - start.x) > searchRange) || (Math.Abs(y - start.y) > searchRange))
    //        {
    //            return new int2(-1, -1);
    //        }
    //    }
    //}
    #endregion

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float moveOffset = 1.5f;
        float moveOffsetExtra = moveOffset + 0.5f;

        //have to update every frame to use handles
        _entityTypeHandle.Update(ref state);
        _localTransformTypeHandle.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var worldGrid = SystemAPI.GetSingleton<WorldGrid>();
        var config = SystemAPI.GetSingleton<Config>();

        //var = SystemAPI.Query<FarmerAspect>();
        int searchRange = 50;


        //var BrainJob = new FarmerBrainJob
        //{

        //};
        //BrainJob.ScheduleParallel();

        var rockChunks = _rockQuery.ToArchetypeChunkArray(state.WorldUpdateAllocator);


        foreach (var farmer in SystemAPI.Query<FarmerAspect>())
        {

            if (farmer.HoldingEntity)
            {
                float3 attachPos = farmer.Transform.LocalPosition + farmer.Farmer.ValueRW.backpackOffset;
                //TODO: Why tf does this disappear??? You can see that it's being set to the right pos in the editor but it's invisible.
                ecb.SetComponent<LocalTransform>(farmer.HeldEntity, new LocalTransform() { Position = attachPos, Scale= 1.0f });
            }

            int2 farmerGridPosition = worldGrid.WorldToGrid(farmer.Transform.LocalTransform.Position);
            int2 startPosition = farmerGridPosition - searchRange / 2;
            int2 endPosition = farmerGridPosition + searchRange / 2;



            float closestSqrMag = math.INFINITY;
            switch (farmer.FarmerState)
            {
                case FarmerStates.FARMER_STATE_IDLE:
                    ChooseNewTask(farmer,ref state);
                    break;

                case FarmerStates.FARMER_STATE_ROCKDESTROY:
                    #region Rock Destruction

                    RockAspect closestRock = new RockAspect();

                    bool foundRock = false;
                    #region Using Jobs + Grid (Not done)
                    //int2 closestPosition = farmerGridPosition;
                    //var findClosestJob = new ClosestIndexFinderJob
                    //{
                    //    worldGrid = worldGrid,
                    //    //This allocator allows us to not have to dispose, it waits 2 frames and then disposes
                    //    closestPosition = new NativeReference<int2>(state.WorldUpdateAllocator),
                    //    startingPosition = farmerGridPosition


                    //};


                    //var jobHandle = findClosestJob.Schedule();
                    //jobHandle.Complete();
                    //closestPosition = findClosestJob.closestPosition.Value;
                    //we would have to do this without state.WorldUpdateAllocator
                    //findClosestJob.closestPosition.Dispose();
                    #endregion

                    #region Grid Search (Not done)

                    //int2 closestPosition = farmerGridPosition;
                    //for (int x = 0; x < searchRange/2; x++)
                    //{
                    //    bool foundClosest = false;
                    //    for (int y = 0; y < searchRange/2; y++)
                    //    {
                    //        int2 right = farmerGridPosition + new int2(x, y);
                    //        int2 left = farmerGridPosition + new int2(-x, y);

                    //        int rightType = worldGrid.GetTypeAt(right.x,right.y);
                    //        int leftType = worldGrid.GetTypeAt(left.x, left.y);

                    //        if(rightType == Rock.type)
                    //        {
                    //            closestPosition = right;
                    //            foundClosest = true;
                    //            break;
                    //        }
                    //        if (leftType == Rock.type)
                    //        {
                    //            closestPosition = left;
                    //            foundClosest = true;
                    //            break;
                    //        }
                    //    }
                    //    if (foundClosest) break;
                    //}

                    //var closestRockEntity = worldGrid.GetEntityAt(closestPosition.x, closestPosition.y);

                    //closestRock = state.EntityManager.GetAspect<RockAspect>(closestRockEntity);
                    //foundRock = true;

                    #endregion

                    #region Trying to avoid redundant code (Working)
                    //Entity closestRockEntity = GetClosest(rockChunks, ref _entityTypeHandle, ref _localTransformTypeHandle, farmer);
                    //closestRock = state.EntityManager.GetAspect<RockAspect>(closestRockEntity);
                    //foundRock = (closestRockEntity != Entity.Null);
                    #endregion

                    #region Basic Search (Working)

                    //foreach (var rock in SystemAPI.Query<RockAspect>())
                    //{
                    //    //Let's find closest rock
                    //    float3 diff = rock.Transform.WorldPosition - farmer.Transform.WorldPosition;
                    //    float sqrMag = math.lengthsq(diff);
                    //    if (sqrMag < closestSqrMag)
                    //    {
                    //        closestRock = rock;
                    //        closestSqrMag = sqrMag;
                    //        foundRock = true;
                    //    }
                    //}

                    #endregion

                    #region Grid Search (USING THIS!)

                    int2 rockLoc = SearchGridForType(Rock.type, farmerGridPosition, searchRange, worldGrid);
                    if (rockLoc.x == -1 && rockLoc.y == -1)
                    {
                        //No Rocks Found
                        ChooseNewTask(farmer,ref state);
                        break;
                    }
                    Entity rockEntity = worldGrid.GetEntityAt(rockLoc.x, rockLoc.y);
                    if (rockEntity == Entity.Null)
                    {
                        //Something wrong with the grid to world conversion
                        ChooseNewTask(farmer,ref state);
                        break;
                    }
                    foundRock = true;
                    closestRock = SystemAPI.GetAspectRW<RockAspect>(rockEntity);
                    #endregion

                    #region Acting Upon a Found Rock

                    if (foundRock)
                    {
                        float3 rockDiff = farmer.Transform.WorldPosition - closestRock.Transform.WorldPosition;
                        farmer.MoveTarget = MoveTowards(farmer.Transform.LocalPosition, closestRock.Transform.LocalPosition,moveOffset);

                        if (math.lengthsq(rockDiff) <= (moveOffsetExtra * moveOffsetExtra))
                        {
                            //Let's hurt the rock.
                            closestRock.Health -= 1;
                            if(closestRock.Health <= 0)
                            {
                                ChooseNewTask(farmer,ref state);
                            }
                        }
                    }
                    else
                        ChooseNewTask(farmer,ref state);
                    #endregion

                    #endregion
                    break;

                case FarmerStates.FARMER_STATE_HARVEST:
                    #region Harvesting Plants
                    closestSqrMag = math.INFINITY;
                    PlantAspect closestPlant = new PlantAspect();
                    bool foundPlant = false;

                    #region Query search
                    //foreach (var plant in SystemAPI.Query<PlantAspect>().WithAll<PlantFinishedGrowing>())
                    //{
                    //    if (plant.PickedAndHeld /*|| plant.BeingTargeted*/) continue;
                    //    //Let's find closest plant
                    //    float3 diff = plant.Transform.WorldPosition - farmer.Transform.WorldPosition;
                    //    float sqrMag = math.lengthsq(diff);
                    //    if (sqrMag < closestSqrMag)
                    //    {
                    //        closestPlant = plant;
                    //        closestSqrMag = sqrMag;
                    //        foundPlant = true;
                    //    }
                    //}
                    #endregion

                    #region Grid Search
                    int2 plantLoc = SearchGridForType(PlantFinishedGrowing.type, farmerGridPosition, searchRange, worldGrid);
                    if (plantLoc.x == -1 && plantLoc.y == -1)
                    {
                        //No plants Found
                        ChooseNewTask(farmer, ref state);
                        break;
                    }
                    Entity plantEntity = worldGrid.GetEntityAt(plantLoc.x, plantLoc.y);
                    if (plantEntity == Entity.Null)
                    {
                        //If here, something wrong with the grid to world conversion
                        ChooseNewTask(farmer, ref state);
                        break;
                    }
                    foundPlant = true;
                    closestPlant = SystemAPI.GetAspectRW<PlantAspect>(plantEntity);
                    #endregion

                    if (foundPlant)
                    {
                        float3 plantDiff = farmer.Transform.WorldPosition - closestPlant.Transform.WorldPosition;

                        farmer.MoveTarget = MoveTowards(farmer.Transform.LocalPosition, closestPlant.Transform.LocalPosition, moveOffset);

                        closestPlant.BeingTargeted = true;

                        //TODO: Control all this in a Manager script.. including targeting and detargeting of stuff.
                        //farmer.TargetEntity(closestPlant.Self);

                        if (math.lengthsq(plantDiff) <= (moveOffsetExtra * moveOffsetExtra))
                        {
                            //Let's pickup the plant
                            farmer.AttachEntity(closestPlant.Self);
                            //closestPlant.BeingTargeted = true;
                            closestPlant.PickedAndHeld = true;

                            if (closestPlant.HasPlot)
                            {
                                var plotAspect = SystemAPI.GetAspectRW<PlotAspect>(closestPlant.Plot);
                                plotAspect.Harvest();
                                closestPlant.HasPlot = false;
                                worldGrid.SetTypeAt(plantLoc, Plot.type);
                                worldGrid.SetEntityAt(plantLoc, closestPlant.Plot);
                            }

                            ChooseSpecificTask(farmer, FarmerStates.FARMER_STATE_PLACEINSILO, ref state);
                        }
                    }
                    else
                        ChooseNewTask(farmer,ref state);

                    #endregion
                    break;
                case FarmerStates.FARMER_STATE_PLACEINSILO:
                    #region Placing In Silo
                    closestSqrMag = math.INFINITY;
                    SiloAspect closestSilo = new SiloAspect();
                    bool foundSilo = false;

                    #region Query search
                    //foreach (var silo in SystemAPI.Query<SiloAspect>())
                    //{
                    //    //Let's find closest silo
                    //    float3 diff = silo.Transform.WorldPosition - farmer.Transform.WorldPosition;
                    //    float sqrMag = math.lengthsq(diff);
                    //    if (sqrMag < closestSqrMag)
                    //    {
                    //        closestSilo = silo;
                    //        closestSqrMag = sqrMag;
                    //        foundSilo = true;
                    //    }
                    //}
                    #endregion

                    #region Grid search
                    int2 siloLoc = SearchGridForType(Silo.type, farmerGridPosition, searchRange, worldGrid);
                    if (siloLoc.x == -1 && siloLoc.y == -1)
                    {
                        //No plants Found
                        ChooseNewTask(farmer, ref state);
                        break;
                    }
                    Entity siloEntity = worldGrid.GetEntityAt(siloLoc.x, siloLoc.y);
                    if (siloEntity == Entity.Null)
                    {
                        //If here, something wrong with the grid to world conversion
                        ChooseNewTask(farmer, ref state);
                        break;
                    }
                    foundSilo = true;
                    closestSilo = SystemAPI.GetAspectRW<SiloAspect>(siloEntity);
                    #endregion

                    if (foundSilo)
                    {
                        float3 siloDiff = farmer.Transform.WorldPosition - closestSilo.Transform.WorldPosition;

                        farmer.MoveTarget = MoveTowards(farmer.Transform.LocalPosition, closestSilo.Transform.LocalPosition, moveOffset);

                        if (math.lengthsq(siloDiff) <= (moveOffsetExtra * moveOffsetExtra))
                        {
                            //Let's pickup the plant
                            closestSilo.Cash += 25;
                            ecb.DestroyEntity(farmer.HeldEntity);
                            farmer.DetachEntity();
                            ChooseSpecificTask(farmer, FarmerStates.FARMER_STATE_ROCKDESTROY, ref state);
                        }
                    }
                    else
                        ChooseNewTask(farmer,ref state);
                    #endregion
                    break;
                case FarmerStates.FARMER_STATE_CREATEPLOT:
                    #region Creating a Plot
                    
                    bool foundTile = false;
                    #region Grid Search (USING THIS!)

                    // type of 0 is empty
                    int2 emptyGridPos = SearchGridForType(0, farmerGridPosition, searchRange, worldGrid);
                    if (emptyGridPos.x == -1 && emptyGridPos.y == -1)
                    {
                        //Can't find any empty lots
                        ChooseNewTask(farmer,ref state);
                        break;
                    }
                    foundTile = true;
                    #endregion

                    if (foundTile)
                    {
                        float3 emptyPos = worldGrid.GridToWorld(emptyGridPos);
                        //Let's move to it
                        farmer.MoveTarget = emptyPos;

                        float3 diff = farmer.Transform.LocalPosition - emptyPos;
                        if(math.lengthsq(diff) < 1.0f)
                        {
                            Entity plot = state.EntityManager.Instantiate(config.PlotPrefab);
                            var newLocal = new LocalTransform { Position = emptyPos, Scale = 1.0f };

                            ecb.SetComponent<LocalTransform>(config.PlotPrefab, newLocal);
                            worldGrid.SetTypeAt(emptyGridPos, Plot.type);
                            worldGrid.SetEntityAt(emptyGridPos, plot);
                            
                            ChooseNewTask(farmer, ref state);
                        }
                    }
                    #endregion
                    break;
                case FarmerStates.FARMER_STATE_PLANTCROP:
                    #region Plant a Crop in a Plot
                    closestSqrMag = math.INFINITY;
                    PlotAspect closestPlot = new PlotAspect();
                    bool foundEmptyPlot = false;

                    //UnityEngine.Debug.Log("Farmer plant crop");

                    #region Query
                    //foreach (var plot in SystemAPI.Query<PlotAspect>())
                    //{
                    //    if (plot.HasSeed())
                    //        continue;

                    //    //Let's find closest plot
                    //    float3 diff = plot.Transform.WorldPosition - farmer.Transform.WorldPosition;
                    //    float sqrMag = math.lengthsq(diff);
                    //    if (sqrMag < closestSqrMag)
                    //    {
                    //        closestPlot = plot;
                    //        closestSqrMag = sqrMag;
                    //        foundEmptyPlot = true;
                    //    }
                    //}
                    #endregion Query

                    #region Grid search
                    int2 plotLoc = SearchGridForType(Plot.type, farmerGridPosition, searchRange, worldGrid);
                    if (plotLoc.x == -1 && plotLoc.y == -1)
                    {
                        //No plots Found
                        ChooseNewTask(farmer, ref state);
                        break;
                    }
                    Entity plotEntity = worldGrid.GetEntityAt(plotLoc.x, plotLoc.y);
                    if (plotEntity == Entity.Null)
                    {
                        //If here, something wrong with the grid to world conversion
                        ChooseNewTask(farmer, ref state);
                        break;
                    }
                    foundEmptyPlot = true;
                    closestPlot = SystemAPI.GetAspectRW<PlotAspect>(plotEntity);
                    #endregion

                    if (foundEmptyPlot)
                    {
                        float3 plotDiff = farmer.Transform.WorldPosition - closestPlot.Transform.WorldPosition;
                        farmer.MoveTarget = MoveTowards(farmer.Transform.LocalPosition, closestPlot.Transform.LocalPosition, moveOffset);

                        if (math.lengthsq(plotDiff) <= (moveOffsetExtra * moveOffsetExtra))
                        {
                            closestPlot.PlantSeed(plotLoc);
                            worldGrid.SetTypeAt(plotLoc, Plant.type);
                            worldGrid.SetEntityAt(plotLoc, closestPlot.Plant);
                            ChooseNewTask(farmer,ref state);
                        }
                    }
                    else
                        ChooseNewTask(farmer,ref state);
                    #endregion
                    break;

            }
        }
    }

    public Entity GetClosest(NativeArray<ArchetypeChunk> chunks,ref EntityTypeHandle eth, ref ComponentTypeHandle<LocalTransform> transform, FarmerAspect farmer)
    {
        float closestSqrMag = math.INFINITY;
        Entity closestEntity = Entity.Null;

        foreach(var chunk in chunks)
        {
            var entities = chunk.GetNativeArray(eth);
            var transforms = chunk.GetNativeArray(ref transform);

            int i = 0;

            foreach (var trans in transforms)
            {
                //Let's find closest rock
                float3 diff = farmer.Transform.LocalPosition - trans.Position;
                float sqrMag = math.lengthsq(diff);

                if (sqrMag < closestSqrMag)
                {
                    closestEntity = entities[i];
                    closestSqrMag = sqrMag;
                }
                i++;
            }
        }

        return closestEntity;
    }

    float3 MoveTowards(float3 from, float3 to, float moveOffset)
    {
        float3 diff = to - from;
        float3 point = from;

        if(math.lengthsq(diff) > 0.1f)
        {
            point = to + math.normalize(-diff) * moveOffset;
        }

        return point;
    }
}




partial struct FarmerBrainJob : IJobEntity
{
    public void Execute(FarmerAspect fa)
    {

    }
}

partial struct ClosestIndexFinderJob : IJob
{
    public int2 startingPosition;
    public int typeIndex;
    public int searchRange;
    public WorldGrid worldGrid;
    public NativeReference<int2> closestPosition;

    public void Execute()
    {
        for (int x = 0; x < searchRange / 2; x++)
        {
            bool foundClosest = false;
            for (int y = 0; y < searchRange / 2; y++)
            {
                int2 right = startingPosition + new int2(x, y);
                int2 left = startingPosition + new int2(-x, y);

                int rightType = worldGrid.GetTypeAt(right.x, right.y);
                int leftType = worldGrid.GetTypeAt(left.x, left.y);

                if (rightType == Rock.type)
                {
                    closestPosition.Value = right;
                    foundClosest = true;
                    break;
                }
                if (leftType == Rock.type)
                {
                    closestPosition.Value = left;
                    foundClosest = true;
                    break;
                }
            }
            if (foundClosest) break;
        }
    }
}