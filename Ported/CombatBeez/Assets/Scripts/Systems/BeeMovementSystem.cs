using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Profiling;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst.Intrinsics;

[BurstCompile]
partial struct BeeBehaviorJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;
    public Random rand;
    public Config config;

    private bool attack;
    public float frameCount;

    [ReadOnly] public ComponentDataFromEntity<Translation> transLookup;
    [ReadOnly] [NativeDisableContainerSafetyRestriction] public ComponentDataFromEntity<Bee> beeLookup;
    [ReadOnly] public ComponentDataFromEntity<FoodResource> foodResourceLookup;

    [ReadOnly] public NativeArray<Entity> enemyBeeEntities;
    [ReadOnly] public NativeArray<Entity> resourceEntities;
    [ReadOnly] public NativeArray<Translation> foodLocations;

    [BurstCompile] 
    public void ExecuteIdleState(ref BeeAspect beeAspect)
    {
        //This state picks one of the other states to go to...
        //This function picks a resource/bee to move towards and then moves to the next state
        //bee.beeState = BEESTATE.FORAGE;
        //this will need a foreach to get a resource to gather, then change states depending

        if (!attack && foodLocations.Length > 0)
        {
            //find nearest food resource instead of random?
            int foodResourceIndex = rand.NextInt(foodLocations.Length);
            Translation foodPoint = foodLocations[foodResourceIndex];

            beeAspect.BeeState = Bee.BEESTATE.FORAGE;

            beeAspect.TargetResource = resourceEntities[foodResourceIndex];
            beeAspect.Target = foodPoint.Value;
        }
        else if (attack && enemyBeeEntities.Length > 0)
        {
            int enemyBeeIndex = rand.NextInt(enemyBeeEntities.Length);
            Translation enemyBeeTrans = transLookup[enemyBeeEntities[enemyBeeIndex]];

            beeAspect.BeeState = Bee.BEESTATE.ATTACK;

            beeAspect.Target = enemyBeeTrans.Value;
            beeAspect.TargetBee = enemyBeeEntities[enemyBeeIndex];
        }
        else
        {
            if (frameCount % 15 == 0)
                beeAspect.Target = beeAspect.Position + rand.NextFloat3(-0.25f, 0.25f);
        }

        //Calculate whether this bee's next move is to attack or not, based on the config.AttackChance value
        float atkPercent = rand.NextFloat(0f, 1f);
        attack = (atkPercent <= config.AttackChance);
    }

    [BurstCompile]
    public void ExecuteForageState(int chunkIndex, ref BeeAspect beeAspect)
    {
        FoodResource foodRes;

        if (foodResourceLookup.TryGetComponent(beeAspect.TargetResource, out foodRes))
        {
            //check to see if resource is still valid, if not go back to idle
            if (foodRes.State != FoodState.SETTLED)
            {
                beeAspect.BeeState = Bee.BEESTATE.IDLE;
            }
            else if (beeAspect.AtBeeTarget)
            {
                beeAspect.BeeState = Bee.BEESTATE.CARRY;
                beeAspect.Target = beeAspect.SpawnPoint;

                foodRes.State = FoodState.CARRIED;
                ECB.SetComponent(chunkIndex, beeAspect.TargetResource, foodRes);
            }
        }
        else
        {
            beeAspect.BeeState = Bee.BEESTATE.IDLE;
        }
    }

    [BurstCompile]
    public void ExecuteCarryState(int chunkIndex, ref BeeAspect beeAspect)
    {
        FoodResource foodRes = foodResourceLookup[beeAspect.TargetResource];

        //update food location in this state
        if (beeAspect.AtBeeTarget)
        {
            beeAspect.BeeState = Bee.BEESTATE.IDLE;
            foodRes.State = FoodState.FALLING;

            ECB.SetComponent(chunkIndex, beeAspect.TargetResource, foodRes);
        }
        else
        {
            Translation foodPos = transLookup[beeAspect.TargetResource];
            foodPos.Value = beeAspect.Position + new float3(0, -2, 0);

            ECB.SetComponent(chunkIndex, beeAspect.TargetResource, foodPos);
        }
    }

    [BurstCompile]
    public void ExecuteAttackState(int chunkIndex, ref BeeAspect beeAspect)
    {
        if (beeAspect.AtBeeTarget)
        {
            //Reached the target bee and kill it, then return to the idle state
            beeAspect.BeeState = Bee.BEESTATE.IDLE;
            Bee enemyBee;

            if (beeLookup.TryGetComponent(beeAspect.TargetBee, out enemyBee))
            {
                if (enemyBee.beeState == Bee.BEESTATE.CARRY)
                {
                    FoodResource foodRes = foodResourceLookup[beeLookup[beeAspect.TargetBee].TargetResource];
                    foodRes.State = FoodState.FALLING;

                    ECB.SetComponent<FoodResource>(chunkIndex, beeLookup[beeAspect.TargetBee].TargetResource, foodRes);
                }

                Entity blood = ECB.Instantiate(chunkIndex, config.BloodPrefab);
                ECB.SetComponent<Translation>(chunkIndex, blood, transLookup[beeAspect.TargetBee]);
                ECB.SetComponent<Blood>(chunkIndex, blood, new Blood() { State = BloodState.FALLING });

                ECB.DestroyEntity(chunkIndex, beeAspect.TargetBee);
            }
        }
        else
        {
            Translation enemyBeeTrans;

            if (transLookup.TryGetComponent(beeAspect.TargetBee, out enemyBeeTrans))
            {
                beeAspect.Target = transLookup[beeAspect.TargetBee].Value;
            }
            else
            {
                //Target bee died before we got there
                beeAspect.BeeState = Bee.BEESTATE.IDLE;
            }
        }
    }

    void Execute([ChunkIndexInQuery] int chunkIndex, ref BeeAspect beeAspect)
    {
        switch (beeAspect.Bee.beeState)
        {
            case Bee.BEESTATE.IDLE:
                //s_PreparePerfMarker_IDLE.Begin();
                ExecuteIdleState(ref beeAspect);
                //s_PreparePerfMarker_IDLE.End();
                break;
            case Bee.BEESTATE.FORAGE:
                //s_PreparePerfMarker_FORAGE.Begin();
                ExecuteForageState(chunkIndex, ref beeAspect);
                //s_PreparePerfMarker_FORAGE.End();
                break;
            case Bee.BEESTATE.CARRY:
                //s_PreparePerfMarker_CARRY.Begin();
                ExecuteCarryState(chunkIndex, ref beeAspect);
                //s_PreparePerfMarker_CARRY.End();
                break;
            case Bee.BEESTATE.ATTACK:
                //s_PreparePerfMarker_ATTACK.Begin();
                ExecuteAttackState(chunkIndex, ref beeAspect);
                //s_PreparePerfMarker_ATTACK.End();
                break;
            default:
                break;
        }
    }
}

[BurstCompile]
partial struct MoveBeeJob : IJobEntity
{
    public float DeltaTime;
    public float beeSpeed;
    public float fCount;
    public float oMag;
    public Random rand;

    void Execute([ChunkIndexInQuery] int chunkIndex, ref BeeAspect beeAspect)
    {
        var pos = beeAspect.Position;
        var dir = beeAspect.Target - pos;
        float mag = (dir.x * dir.x) + (dir.y * dir.y) + (dir.z * dir.z);
        float dist = math.sqrt(mag);
        float localMoveSpeed = beeSpeed;

        beeAspect.AtBeeTarget = false;

        beeAspect.Rotation = quaternion.LookRotation(dir, new float3(0, 1, 0));
        dir += beeAspect.OcillateOffset;

        if (dist <= 8f && dist > 2f)
            localMoveSpeed *= 1.5f;

        if (dist > 0)
            beeAspect.Position += (dir / dist) * DeltaTime * localMoveSpeed;


        if (dist <= 2f)
        {
            beeAspect.AtBeeTarget = true;
        }

        if (fCount % 30 == 0)
        {
            float absOcillate = oMag * mag;
            float ocillateAmount = math.min(absOcillate, 10);
            beeAspect.OcillateOffset = rand.NextFloat3(-ocillateAmount, ocillateAmount);
        }

        //Apply bee stretch
        float3 stretchDir = math.abs(dir / dist);
        stretchDir = math.clamp(stretchDir + 0.5f, 0.5f, 1.5f);
        beeAspect.Scale = new float3(beeAspect.Scale.x, beeAspect.Scale.y, stretchDir.z + 0.5f);

        //Ensure bees don't leave their enclosure
        float3 clampPos = beeAspect.Position;

        clampPos.y = math.clamp(beeAspect.Position.y, 0.01f, 20f);
        clampPos.z = math.clamp(beeAspect.Position.z, -15f, 15f);
        clampPos.x = math.clamp(beeAspect.Position.x, -50f, 50f);

        beeAspect.Position = clampPos;
    }
}

[BurstCompile]
public partial struct BeeMovementSystem : ISystem
{
    //static ProfilerMarker s_PreparePerfMarker_IDLE;
    //static ProfilerMarker s_PreparePerfMarker_CARRY;
    //static ProfilerMarker s_PreparePerfMarker_FORAGE;
    //static ProfilerMarker s_PreparePerfMarker_ATTACK;
    //static ProfilerMarker s_PreparePerfMarker_MOVINGTOTARGET;
    //static ProfilerMarker s_PreparePerfMarker_ASSIGNMOVE_1;
    //static ProfilerMarker s_PreparePerfMarker_ASSIGNMOVE_2;

    private float moveSpeed;
    private float ocilateMag;

    private ComponentDataFromEntity<Translation> transLookup;
    private ComponentDataFromEntity<Bee> beeLookup;
    private ComponentDataFromEntity<FoodResource> foodResourceLookup;

    private EntityQuery foodEntityQuery;
    private EntityQuery yellowEnemyBeeQuery;
    private EntityQuery blueEnemyBeeQuery;

    //Modified each update frame
    private float frameCount;
    private float dt;

    private Random rand;
    private Config config;

    private bool attack;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<Config>();
        //s_PreparePerfMarker_IDLE = new ProfilerMarker("ExecuteIdleState");
        //s_PreparePerfMarker_CARRY = new ProfilerMarker("ExecuteCarryState");
        //s_PreparePerfMarker_FORAGE = new ProfilerMarker("ExecuteForageState");
        //s_PreparePerfMarker_ATTACK = new ProfilerMarker("ExecuteAttackState");
        //s_PreparePerfMarker_MOVINGTOTARGET = new ProfilerMarker("MovingToTarget");
        //s_PreparePerfMarker_ASSIGNMOVE_1 = new ProfilerMarker("AssigningMovement1");
        //s_PreparePerfMarker_ASSIGNMOVE_2 = new ProfilerMarker("AssigningMovement2");

        ocilateMag = 20f;

        //Resource entities
        var queryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        queryBuilder.AddAll(ComponentType.ReadWrite<Translation>());
        queryBuilder.AddAll(ComponentType.ReadWrite<FoodResource>());
        queryBuilder.FinalizeQuery();

        //Blue bee entities
        var blueBeeQueryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        blueBeeQueryBuilder.AddAll(ComponentType.ReadWrite<BlueBee>());
        blueBeeQueryBuilder.FinalizeQuery();

        blueEnemyBeeQuery = state.GetEntityQuery(blueBeeQueryBuilder);
        //---

        //Yellow bee entities
        var yellowBeeQueryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        yellowBeeQueryBuilder.AddAll(ComponentType.ReadWrite<YellowBee>());
        yellowBeeQueryBuilder.FinalizeQuery();
        
        yellowEnemyBeeQuery = state.GetEntityQuery(yellowBeeQueryBuilder);
        //---

        foodEntityQuery = state.GetEntityQuery(queryBuilder);
        transLookup = state.GetComponentDataFromEntity<Translation>();
        beeLookup = state.GetComponentDataFromEntity<Bee>();
        foodResourceLookup = state.GetComponentDataFromEntity<FoodResource>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    

    [BurstCompile]
    public void MoveToTarget(TransformAspect transform, ref Bee beeData, RefRW<NonUniformScale> scale)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // The Entities.ForEach below is Burst compiled (implicitly).
        // And time is a member of SystemBase, which is a managed type (class).
        // This means that it wouldn't be possible to directly access Time from there.
        // So we need to copy the value we need (DeltaTime) into a local variable.
        dt = state.Time.DeltaTime;

        config = SystemAPI.GetSingleton<Config>();
        moveSpeed = config.BeeSpeed;
        rand = Random.CreateFromIndex((uint)UnityEngine.Time.frameCount);
        frameCount = UnityEngine.Time.frameCount;
        transLookup.Update(ref state);
        beeLookup.Update(ref state);
        foodResourceLookup.Update(ref state);

        //worldupdateallocator - anything allocated to it will get passed to jobs, but you don't have to manually deallocate, they will
        //dispose of themselves with the world/every 2 frames
        var foodTranslations = foodEntityQuery.ToComponentDataArray<Translation>(state.WorldUpdateAllocator);
        var foodEntities = foodEntityQuery.ToEntityArray(Allocator.Temp);

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb_yellow = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var ecb_blue = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var yellowBeeEntities = yellowEnemyBeeQuery.ToEntityArray(Allocator.Temp);
        var blueBeeEntities = blueEnemyBeeQuery.ToEntityArray(Allocator.Temp);

        var yellowBeeBehaviorJob = new BeeBehaviorJob
        {
            foodLocations = foodTranslations,
            foodResourceLookup = foodResourceLookup,
            enemyBeeEntities = blueBeeEntities,
            ECB = ecb_yellow.AsParallelWriter(),
            rand = rand,
            config = config,
            frameCount = frameCount,
            beeLookup = beeLookup,
            resourceEntities = foodEntities,
            transLookup = transLookup
        };
        yellowBeeBehaviorJob.ScheduleParallel();

        var blueBeeBehaviorJob = new BeeBehaviorJob
        {
            foodLocations = foodTranslations,
            foodResourceLookup = foodResourceLookup,
            enemyBeeEntities = yellowBeeEntities,
            ECB = ecb_blue.AsParallelWriter(),
            rand = rand,
            config = config,
            frameCount = frameCount,
            beeLookup = beeLookup,
            resourceEntities = foodEntities,
            transLookup = transLookup
        };
        //blueBeeBehaviorJob.ScheduleParallel();

        //Call the move to target IJob
        //s_PreparePerfMarker_MOVINGTOTARGET.Begin();
        var moveBeeJob = new MoveBeeJob
        {
            DeltaTime = state.Time.DeltaTime,
            beeSpeed = moveSpeed,
            fCount = frameCount,
            oMag = ocilateMag,
            rand = rand,
        };
        moveBeeJob.ScheduleParallel();
        //s_PreparePerfMarker_MOVINGTOTARGET.End();
    }
}