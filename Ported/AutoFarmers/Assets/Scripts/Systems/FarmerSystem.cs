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
        _rockQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform,Rock>().Build();
        _entityTypeHandle = SystemAPI.GetEntityTypeHandle();
        _localTransformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>();

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void ChooseNewTask(FarmerAspect farmer)
    {
        //Cooldown?
        farmer.FarmerState = (byte)random.NextInt(0,FarmerStates.FARMER_TOTAL_STATES+1);
        //UnityEngine.Debug.Log("Farmer state: " + farmer.FarmerState);
    }

    //Searches for a specific type within a set range
    [BurstCompile]
    public int2 SearchGridForEmpty(int2 farmerGridPosition, int searchRange, WorldGrid worldGrid)
    {
        int2 closestPosition = farmerGridPosition;
        for (int x = 0; x < searchRange / 2; x++)
        {
            bool foundClosest = false;
            for (int y = 0; y < searchRange / 2; y++)
            {
                int2 right = farmerGridPosition + new int2(x, y);
                int2 left = farmerGridPosition + new int2(-x, y);

                int rightType = worldGrid.GetTypeAt(right.x, right.y);
                int leftType = worldGrid.GetTypeAt(left.x, left.y);

                if (rightType == 0)
                {
                    closestPosition = right;
                    foundClosest = true;
                    break;
                }

                if (leftType == 0)
                {
                    closestPosition = left;
                    foundClosest = true;
                    break;
                }
            }
            if (foundClosest)
                return closestPosition;
            else
                return 0;
        }
        return 0;
    }

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
                    ChooseNewTask(farmer);
                    break;

                case FarmerStates.FARMER_STATE_ROCKDESTROY:
                    #region Rock Destruction

                    RockAspect closestRock = new RockAspect();

                    bool foundRock = false;

                    #region Trying to avoid redundant code (Working)
                    Entity closestRockEntity = GetClosest(rockChunks, ref _entityTypeHandle, ref _localTransformTypeHandle, farmer);
                    closestRock = state.EntityManager.GetAspect<RockAspect>(closestRockEntity);
                    foundRock = (closestRockEntity != Entity.Null);
                    #endregion

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

                    #region Acting Upon a Found Rock

                    if (foundRock)
                    {
                        float3 rockDiff = farmer.Transform.WorldPosition - closestRock.Transform.WorldPosition;
                        farmer.MoveTarget = closestRock.Transform.WorldPosition + moveOffset * math.normalize(rockDiff);

                        if (math.lengthsq(rockDiff) <= (moveOffsetExtra * moveOffsetExtra))
                        {
                            //Let's hurt the rock.
                            closestRock.Health -= 1;
                            if(closestRock.Health <= 0)
                            {
                                ChooseNewTask(farmer);
                            }
                        }
                    }
                    else
                        ChooseNewTask(farmer);
                    #endregion

                    #endregion
                    break;

                case FarmerStates.FARMER_STATE_HARVEST:
                    #region Harvesting Plants
                    closestSqrMag = math.INFINITY;
                    PlantAspect closestPlant = new PlantAspect();
                    bool foundPlant = false;
                    foreach (var plant in SystemAPI.Query<PlantAspect>().WithAll<PlantFinishedGrowing>())
                    {
                        if (plant.PickedAndHeld /*|| plant.BeingTargeted*/) continue;
                        //Let's find closest plant
                        float3 diff = plant.Transform.WorldPosition - farmer.Transform.WorldPosition;
                        float sqrMag = math.lengthsq(diff);
                        if (sqrMag < closestSqrMag)
                        {
                            closestPlant = plant;
                            closestSqrMag = sqrMag;
                            foundPlant = true;
                        }
                    }

                    if (foundPlant)
                    {
                        float3 plantDiff = farmer.Transform.WorldPosition - closestPlant.Transform.WorldPosition;
                        farmer.MoveTarget = closestPlant.Transform.WorldPosition + moveOffset * math.normalize(plantDiff);
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
                                UnityEngine.Debug.Log("Harvesting a plant with a plot");
                                //TODO:why does the below line cause infinite plants?
                                plotAspect.Harvest();
                                closestPlant.HasPlot = false;
                            }

                            farmer.FarmerState = FarmerStates.FARMER_STATE_PLACEINSILO;
                        }
                    }
                    else
                        ChooseNewTask(farmer);

                    #endregion
                    break;
                case FarmerStates.FARMER_STATE_PLACEINSILO:
                    #region Placing In Silo
                    closestSqrMag = math.INFINITY;
                    SiloAspect closestSilo = new SiloAspect();
                    bool foundSilo = false;

                    foreach (var silo in SystemAPI.Query<SiloAspect>())
                    {
                        //Let's find closest silo
                        float3 diff = silo.Transform.WorldPosition - farmer.Transform.WorldPosition;
                        float sqrMag = math.lengthsq(diff);
                        if (sqrMag < closestSqrMag)
                        {
                            closestSilo = silo;
                            closestSqrMag = sqrMag;
                            foundSilo = true;
                        }
                    }

                    if (foundSilo)
                    {
                        float3 siloDiff = farmer.Transform.WorldPosition - closestSilo.Transform.WorldPosition;
                        farmer.MoveTarget = closestSilo.Transform.WorldPosition + moveOffset * math.normalize(siloDiff);

                        if (math.lengthsq(siloDiff) <= (moveOffsetExtra * moveOffsetExtra))
                        {
                            //Let's pickup the plant
                            closestSilo.Cash += 25;
                            //TODO: Deleting doesn't seem to work.. Can't tell if it's the same issue as RockSystem?
                            //Need to destroy all children as well maybe?
                            //How do I get children of an entity?
                            ecb.DestroyEntity(farmer.HeldEntity);
                            farmer.DetachEntity();
                            ChooseNewTask(farmer);
                        }
                    }
                    else
                        ChooseNewTask(farmer);
                    #endregion
                    break;
                case FarmerStates.FARMER_STATE_CREATEPLOT:
                    #region Creating a Plot
                    
                    bool foundTile = false;
                    // grab tile of current farmer

                    //TODO: Check for nearest open tile in grid that doesn't have a rock or silo next to it
                    // foreach tile in gid
                    // start with farmers current tile
                    // search in radius surrounding him for open tile
                    // if tile is open, search around it for rock or silo
                    // break if found
                    // if none work, expand radius
                    // once found, create plot
                    // check each spot around the plot just created to be able to expand and restart the search

                    ChooseNewTask(farmer);
                    #endregion
                    break;
                case FarmerStates.FARMER_STATE_PLANTCROP:
                    #region Plant a Crop in a Plot
                    closestSqrMag = math.INFINITY;
                    PlotAspect closestPlot = new PlotAspect();
                    bool foundPlot = false;
                    //UnityEngine.Debug.Log("Farmer plant crop");
                    
                    foreach (var plot in SystemAPI.Query<PlotAspect>())
                    {
                        if (plot.HasSeed())
                            continue;

                        //Let's find closest plot
                        float3 diff = plot.Transform.WorldPosition - farmer.Transform.WorldPosition;
                        float sqrMag = math.lengthsq(diff);
                        if (sqrMag < closestSqrMag)
                        {
                            closestPlot = plot;
                            closestSqrMag = sqrMag;
                            foundPlot = true;
                        }
                    }

                    if (foundPlot)
                    {
                        float3 plotDiff = farmer.Transform.WorldPosition - closestPlot.Transform.WorldPosition;
                        farmer.MoveTarget = closestPlot.Transform.WorldPosition + moveOffset * math.normalize(plotDiff);

                        if (math.lengthsq(plotDiff) <= (moveOffsetExtra * moveOffsetExtra))
                        {
                            closestPlot.PlantSeed();
                            ChooseNewTask(farmer);
                        }
                    }
                    else
                        ChooseNewTask(farmer);
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