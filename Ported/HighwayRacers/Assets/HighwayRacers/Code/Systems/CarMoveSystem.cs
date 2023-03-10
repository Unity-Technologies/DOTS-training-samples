using System;
using System.Collections.Generic;
using HighwayRacers;
using Unity.Burst;
using Unity.Entities;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(CarSpawnSystem))]
public partial struct CarMoveSystem : ISystem
{
#if USE_DOTS_STAGES
    private EntityQuery allCarsQuery;
    private ComponentTypeHandle<CarData> carHandle;
#endif
    
#if USE_HIGHWAY
    private NativeArray<float3> highwayPieces;

    private void mockHighwayPieces()
    {
        highwayPieces[0] = new float3() {x = 0f, z = 0f, y = 0f};
        highwayPieces[1] = new float3() {x = 0f, z = 33.89f, y = 0f};
        highwayPieces[2] = new float3() {x = 31.46f, z = 65.35f, y = 1.570796f};
        highwayPieces[3] = new float3() {x = 65.34999f, z = 65.35f, y = 1.570796f};
        highwayPieces[4] = new float3() {x = 96.80999f, z = 33.89f, y = 3.141593f};
        highwayPieces[5] = new float3() {x = 96.80999f, z = 3.814697E-06f, y = 3.141593f};
        highwayPieces[6] = new float3() {x = 65.34999f, z = -31.45999f, y = 4.712389f};
        highwayPieces[7] = new float3() {x = 31.46f, z = -31.45999f, y = 4.712389f};
    }
#endif
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

#if USE_HIGHWAY
        /*
        // Don't have access to managed pieces until we convert Highway to Entities
        // Burst error BC1016: The managed function `HighwayRacers.Highway.get_instance()` is not supported
        //var pieces = Highway.instance.pieces;
        // Translate highway to HighwayPieceStruct
        for (int i = 0; i < pieces.Length; i++)
        {
            highwayPieces[i] = new float3()
            {
                x = pieces[i].startX,
                z = pieces[i].startZ,
                y = pieces[i].startRotation, // y is rotation
            };
        }
        */
        
        // Mock set Highway size for now
        highwayPieces = new NativeArray<float3>(8, Allocator.Persistent);
        mockHighwayPieces();
#endif
        
#if USE_DOTS_STAGES
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<CarData>();
        allCarsQuery = state.GetEntityQuery(builder);
        
        carHandle = state.GetComponentTypeHandle<CarData>();
#endif
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (state.Dependency != null)
        {
            state.Dependency.Complete();
        }

#if USE_DOTS_STAGES
        /*
	       	CarMoveSystem
        */
        
		// Type handles must be updated before use in each update.
        carHandle.Update(ref state);

        NativeArray<CarData> allCarsArray = allCarsQuery.ToComponentDataArray<CarData>(Allocator.Temp);
        //allCarsQuery.ToEntityArray<Car>()
        // Sample Sort
        //allCarsArray.Sort();

        //Query for all cars.        
        foreach (var (carData, entity) in
                 SystemAPI.Query<RefRW<CarData>>()
	                 .WithAll<CarData>().WithEntityAccess())
        {
            //If car in front (tailgating)
            CarData nearestFrontCar = default;
            float distanceToFrontCar = float.MaxValue;
            
            CarData other;
            for (int i = 0; i < allCarsArray.Length; i++)
            {
                other = allCarsArray[i];

                float angle = Vector3.Angle(Vector3.forward, carData.Position - allCarTransforms[i].Position);
                if (Mathf.Abs(angle) < 90)
                {
                    var distToOtherCarInLane = Vector3.Distance(allCarTransforms[i].Position, car.Position);

                    if (distToOtherCarInLane < distanceToFrontCar)
                    {
                        distanceToFrontCar = distToOtherCarInLane;
                        nearestFrontCar = other;
                    }
                }
            }
            
            //ENABLE 'ChangeLaneState' with desired Lane if not already tagged
            //    ENABLE 'OvertakeTimerState'
            //Else (no car in front)
            //DISABLE 'ChangeLaneState'
            //Move cars forward at desired speed.
            //    If 'OvertakeTimerState' ENABLED
            //    Move at Overtake speed
            //    Else
            //Move at normal speed
        }
#else
        var carquery = SystemAPI.QueryBuilder().WithAll<CarData, LocalTransform>().Build();
        var cars = carquery.ToComponentDataArray<CarData>(state.WorldUpdateAllocator);
        var carTransforms = carquery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);
        
        var config = SystemAPI.GetSingleton<Config>();
        var testJob = new CarMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            config = config,
            frameCount = UnityEngine.Time.frameCount,
            allCars = cars,
            allCarTransforms = carTransforms,
#if USE_HIGHWAY
            allHighwayPieces = highwayPieces,
#endif
        };
        JobHandle jobHandle = testJob.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;
#endif
    }
}