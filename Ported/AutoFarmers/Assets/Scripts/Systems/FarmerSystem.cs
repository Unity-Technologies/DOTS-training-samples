using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
partial struct FarmerSystem : ISystem
{
    Unity.Mathematics.Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Unity.Mathematics.Random(1234);
        state.RequireForUpdate<WorldGrid>();
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

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float moveOffset = 1.5f;
        float moveOffsetExtra = moveOffset + 0.5f;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var worldGrid = SystemAPI.GetSingleton<WorldGrid>();

        foreach (var farmer in SystemAPI.Query<FarmerAspect>())
        {

            if (farmer.HoldingEntity)
            {
                float3 attachPos = farmer.Transform.LocalPosition + farmer.Farmer.ValueRW.backpackOffset;
                //TODO: Why tf does this disappear??? You can see that it's being set to the right pos in the editor but it's invisible.
                ecb.SetComponent<LocalTransform>(farmer.HeldEntity, new LocalTransform() { Position = attachPos, Scale= 1.0f });
            }

            float closestSqrMag = math.INFINITY;
            switch (farmer.FarmerState)
            {
                case FarmerStates.FARMER_STATE_IDLE:
                    ChooseNewTask(farmer);
                    break;

                case FarmerStates.FARMER_STATE_ROCKDESTROY:
                    #region Rock Destruction

                    #region Trying to avoid redundant code...
                    //RockAspect closestRock = GetClosest<RockAspect>(SystemAPI.Query<RockAspect,TransformAspect>(), farmer);

                    //float3 rockDiff = farmer.Transform.WorldPosition - closestRock.Transform.WorldPosition;
                    //farmer.MoveTarget = closestRock.Transform.WorldPosition + moveOffset * math.normalize(rockDiff);
                    #endregion

                    RockAspect closestRock = new RockAspect();
                    bool foundRock = false;

                    foreach (var rock in SystemAPI.Query<RockAspect>())
                    {
                        //Let's find closest rock
                        float3 diff = rock.Transform.WorldPosition - farmer.Transform.WorldPosition;
                        float sqrMag = math.lengthsq(diff);
                        if (sqrMag < closestSqrMag)
                        {
                            closestRock = rock;
                            closestSqrMag = sqrMag;
                            foundRock = true;
                        }
                    }

                    //TODO: How the hell do we tell if there are plants needing to be cleaned up?
                    //We should probably have a FarmerManager system that goes through all farmers and sets their
                    //states depending on the state of the world.  That way we can more intelligently issue commands based on what needs to be done.

                    //if(SystemAPI.Query<PlantAspect>().WithAll<PlantFinishedGrowing>().Count() > 0)
                    //{
                    //    farmer.FarmerState = FarmerStates.FARMER_STATE_HARVEST;
                    //}

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

    //public T GetClosest<T>(QueryEnumerable<T,TransformAspect> query, FarmerAspect farmer) where T : new()
    //{
    //    float closestSqrMag = math.INFINITY;
    //    T closestObj = new T();
    //    TransformAspect closestTransform = new TransformAspect();


    //    foreach (var (obj,transform) in query)
    //    {
    //        //Let's find closest rock
    //        float3 diff = transform.WorldPosition - closestTransform.WorldPosition;
    //        float sqrMag = math.lengthsq(diff);
    //        if (sqrMag < closestSqrMag)
    //        {
    //            closestTransform = transform;
    //            closestObj = obj;
    //            closestSqrMag = sqrMag;
    //        }
    //    }
    //    return closestObj;
    //}
}
