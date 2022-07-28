using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct BeeMovementSystem : ISystem
{
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
        state.RequireForUpdate<Config>();

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
    public void ExecuteIdleState(
        NativeArray<Entity> enemyBeeEntities,
        NativeArray<Entity> resourceEntities,
        NativeArray<Translation> foodLocations,
        TransformAspect beeTransformAspect,
        ref Bee bdata,
        ref SystemState sState,
        RefRW<NonUniformScale> beeScale)
    {
        //This state picks one of the other states to go to...
        //This function picks a resource to move towards and then moves to the execute forage state
        //bee.beeState = BEESTATE.FORAGE;
        //this will need a foreach to get a resource to gather, then change states depending

        if (!attack && foodLocations.Length > 0)
        {
            int foodResourceIndex = rand.NextInt(foodLocations.Length);
            Translation foodPoint = foodLocations[foodResourceIndex];

            bdata.beeState = Bee.BEESTATE.FORAGE;

            bdata.TargetResource = resourceEntities[foodResourceIndex];
            bdata.Target = foodPoint.Value;
        }
        else if (attack && enemyBeeEntities.Length > 0)
        {
            int enemyBeeIndex = rand.NextInt(enemyBeeEntities.Length);
            Translation enemyBeeTrans = transLookup[enemyBeeEntities[enemyBeeIndex]];

            bdata.beeState = Bee.BEESTATE.ATTACK;

            bdata.Target = enemyBeeTrans.Value;
            bdata.TargetBee = enemyBeeEntities[enemyBeeIndex];
        }
        else
        {
            if (frameCount % 15 == 0)
                bdata.Target = beeTransformAspect.Position + rand.NextFloat3(-0.25f, 0.25f);

            MoveToTarget(beeTransformAspect, ref bdata, beeScale);
        }

        //Calculate whether this bee's next move is to attack or not, based on the config.AttackChance value
        float atkPercent = rand.NextFloat(0f, 1f);
        attack = (atkPercent <= config.AttackChance);
    }

    [BurstCompile]
    public void ExecuteForageState(
        TransformAspect t,
        ref Bee bd,
        ref SystemState sState,
        RefRW<NonUniformScale> beeScale)
    {
        FoodResource foodRes;

        if (foodResourceLookup.TryGetComponent(bd.TargetResource, out foodRes))
        {
            //check to see if resource is still valid, if not go back to idle
            if (foodRes.State != FoodState.SETTLED)
            {
                bd.beeState = Bee.BEESTATE.IDLE;
                MoveToTarget(t, ref bd, beeScale);
            }
            else if (MoveToTarget(t, ref bd, beeScale))
            {
                bd.beeState = Bee.BEESTATE.CARRY;
                bd.Target = bd.SpawnPoint;

                foodRes.State = FoodState.CARRIED;
                sState.EntityManager.SetComponentData<FoodResource>(bd.TargetResource, foodRes);
            }
        }
        else
        {
            bd.beeState = Bee.BEESTATE.IDLE;
        }
    }

    [BurstCompile]
    public void ExecuteCarryState(
        TransformAspect t,
        NativeArray<Translation> foodLocations,
        ref Bee bd,
        ref SystemState sState,
        RefRW<NonUniformScale> beeScale)
    {
        FoodResource foodRes = sState.EntityManager.GetComponentData<FoodResource>(bd.TargetResource);

        //update food location in this state
        if (MoveToTarget(t, ref bd, beeScale))
        {
            bd.beeState = Bee.BEESTATE.IDLE;
            foodRes.State = FoodState.FALLING;

            sState.EntityManager.SetComponentData<FoodResource>(bd.TargetResource, foodRes);
        }
        else
        {
            Translation foodPos = transLookup[bd.TargetResource];
            foodPos.Value = t.Position + new float3(0, -2, 0);

            transLookup[bd.TargetResource] = foodPos;
        }
    }

    [BurstCompile]
    public void ExecuteAttackState(
        EntityCommandBuffer entityCommandBuffer,
        TransformAspect t,
        ref Bee bd,
        ref SystemState sState,
        RefRW<NonUniformScale> beeScale)
    {
        if (MoveToTarget(t, ref bd, beeScale))
        {
            //Reached the target bee and kill it, then return to the idle state
            bd.beeState = Bee.BEESTATE.IDLE;
            Bee enemyBee;

            if (beeLookup.TryGetComponent(bd.TargetBee, out enemyBee))
            {
                if (enemyBee.beeState == Bee.BEESTATE.CARRY)
                {
                    FoodResource foodRes = foodResourceLookup[beeLookup[bd.TargetBee].TargetResource];
                    foodRes.State = FoodState.FALLING;

                    foodResourceLookup[beeLookup[bd.TargetBee].TargetResource] = foodRes;
                }

                Entity blood = entityCommandBuffer.Instantiate(config.BloodPrefab);
                entityCommandBuffer.SetComponent<Translation>(blood, transLookup[bd.TargetBee]);
                entityCommandBuffer.SetComponent<Blood>(blood, new Blood() { State = BloodState.FALLING });

                entityCommandBuffer.DestroyEntity(bd.TargetBee);
            }
        }
        else
        {
            Translation enemyBeeTrans;

            if (transLookup.TryGetComponent(bd.TargetBee, out enemyBeeTrans))
            {
                bd.Target = transLookup[bd.TargetBee].Value;
            }
            else
            {
                //Target bee died before we got there
                bd.beeState = Bee.BEESTATE.IDLE;
            }
        }
    }

    [BurstCompile]
    public bool MoveToTarget(TransformAspect transform, ref Bee beeData, RefRW<NonUniformScale> scale)
    {
        var pos = transform.Position;
        var dir = beeData.Target - pos;
        float mag = (dir.x * dir.x) + (dir.y * dir.y) + (dir.z * dir.z);
        float dist = math.sqrt(mag);
        float localMoveSpeed = moveSpeed;
        bool arrivedAtTarget = false;

        transform.Rotation = quaternion.LookRotation(dir, new float3(0, 1, 0));
        dir += beeData.OcillateOffset;

        if (dist <= 8f && dist > 2f)
            localMoveSpeed *= 1.5f;

        if (dist > 0)
            transform.Position += (dir / dist) * dt * localMoveSpeed;

        if (dist <= 2f)
        {
            arrivedAtTarget = true;
        }

        if (frameCount % 30 == 0)
        {
            float absOcillate = ocilateMag * mag;
            float ocillateAmount = math.min(absOcillate, 10);
            beeData.OcillateOffset = rand.NextFloat3(-ocillateAmount, ocillateAmount);
        }

        //Apply bee stretch
        float3 stretchDir = math.abs(dir / dist);
        stretchDir = math.clamp(stretchDir + 0.5f, 0.5f, 1.5f);
        scale.ValueRW.Value.z = stretchDir.z + 0.5f;

        //Ensure bees don't leave their enclosure
        float3 clampPos = transform.Position;

        clampPos.y = math.clamp(transform.Position.y, 0.01f, 20f);
        clampPos.z = math.clamp(transform.Position.z, -15f, 15f);
        clampPos.x = math.clamp(transform.Position.x, -50f, 50f);

        transform.Position = clampPos;

        return arrivedAtTarget;
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

        var yellowBeeEntities = yellowEnemyBeeQuery.ToEntityArray(Allocator.Temp);
        var blueBeeEntities = blueEnemyBeeQuery.ToEntityArray(Allocator.Temp);

        //create command buffer here to pass into attack state to properly destroy target enemy bees
        EntityCommandBuffer cmdBuffer = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var(transform, bee, beeScale) in SystemAPI.Query<TransformAspect, RefRW<Bee>, RefRW<NonUniformScale>>().WithAny<BlueBee, YellowBee>())
        {
            var tempEnemyBeeEntities = yellowBeeEntities;

            //Test if we are a yellow bee, if so set enemy bees to blue
            if (bee.ValueRW.beeTeam == Team.YELLOW)
                tempEnemyBeeEntities = blueBeeEntities;

            switch (bee.ValueRW.beeState)
            {
                case Bee.BEESTATE.IDLE:
                    ExecuteIdleState(tempEnemyBeeEntities, foodEntities, foodTranslations, transform, ref bee.ValueRW, ref state, beeScale);
                    break;
                case Bee.BEESTATE.FORAGE:
                    ExecuteForageState(transform, ref bee.ValueRW, ref state, beeScale);
                    break;
                case Bee.BEESTATE.CARRY:
                    ExecuteCarryState(transform, foodTranslations, ref bee.ValueRW, ref state, beeScale);
                    break;
                case Bee.BEESTATE.ATTACK:
                    ExecuteAttackState(cmdBuffer, transform, ref bee.ValueRW, ref state, beeScale);
                    break;
                default:
                    ExecuteIdleState(tempEnemyBeeEntities, foodEntities, foodTranslations, transform, ref bee.ValueRW, ref state, beeScale);
                    break;
            }
        }

        cmdBuffer.Playback(state.EntityManager);
        cmdBuffer.Dispose();
    }
}