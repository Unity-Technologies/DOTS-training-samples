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
    //Modified each update frame
    private float frameCount;
    private float dt;
    private Random rand;
    private EntityQuery foodEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        moveSpeed = 8f;
        ocilateMag = 20f;

        var queryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        queryBuilder.AddAll(ComponentType.ReadWrite<Translation>());
        queryBuilder.AddAll(ComponentType.ReadWrite<FoodResource>());
        queryBuilder.FinalizeQuery();

        foodEntityQuery = state.GetEntityQuery(queryBuilder);

        transLookup = state.GetComponentDataFromEntity<Translation>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void ExecuteIdleState(NativeArray<Entity> resourceEntities, NativeArray<Translation> foodLocations, ref Bee bdata)
    {
        //This state picks one of the other states to go to...
        //This function picks a resource to move towards and then moves to the execute forage state
        //bee.beeState = BEESTATE.FORAGE;
        //this will need a foreach to get a resource to gather, then change states depending

        int foodResourceIndex = rand.NextInt(foodLocations.Length);
        Translation foodPoint = foodLocations[foodResourceIndex];

        bdata.TargetResource = resourceEntities[foodResourceIndex];
        bdata.Target = foodPoint.Value;
        bdata.beeState = Bee.BEESTATE.FORAGE;
    }

    [BurstCompile]
    public void ExecuteForageState(TransformAspect t, ref Bee bd, ref SystemState sState)
    {
        FoodResource foodRes = sState.EntityManager.GetComponentData<FoodResource>(bd.TargetResource);

        if (MoveToTarget(t, ref bd))
        {
            bd.beeState = Bee.BEESTATE.CARRY;
            bd.Target = bd.SpawnPoint;

            foodRes.State = FoodState.CARRIED;
            sState.EntityManager.SetComponentData<FoodResource>(bd.TargetResource, foodRes);
        }
        else
        {
            //check to see if resource is still valid, if not go back to idle
            if (foodRes.State != FoodState.SETTLED)
                bd.beeState = Bee.BEESTATE.IDLE;
        }
    }

    [BurstCompile]
    public void ExecuteCarryState(TransformAspect t, NativeArray<Translation> foodLocations, ref Bee bd, ref SystemState sState)
    {
        FoodResource foodRes = sState.EntityManager.GetComponentData<FoodResource>(bd.TargetResource);

        //update food location in this state
        if (MoveToTarget(t, ref bd))
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
    public bool MoveToTarget(TransformAspect transform, ref Bee beeData)
    {
        var pos = transform.Position;
        var dir = beeData.Target - pos;
        float mag = (dir.x * dir.x) + (dir.y * dir.y) + (dir.z * dir.z);
        float dist = math.sqrt(mag);
        bool arrivedAtTarget = false;

        dir += beeData.OcillateOffset;

        if (dist > 0)
            transform.Position += (dir / dist) * dt * moveSpeed;

        if (dist <= 2f)
        {
            arrivedAtTarget = true;
        }

        if (frameCount % 30 == 0)
            beeData.OcillateOffset = rand.NextFloat3(-ocilateMag, ocilateMag);

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
        rand = Random.CreateFromIndex((uint)UnityEngine.Time.frameCount);
        frameCount = UnityEngine.Time.frameCount;
        transLookup.Update(ref state);
        //worldupdateallocator - anything allocated to it will get passed to jobs, but you don't have to manually deallocate, they will
        //dispose of themselves with the world/every 2 frames
        var foodTranslations = foodEntityQuery.ToComponentDataArray<Translation>(state.WorldUpdateAllocator);
        var foodResources = foodEntityQuery.ToComponentDataArray<FoodResource>(state.WorldUpdateAllocator);
        var foodEntities = foodEntityQuery.ToEntityArray(Allocator.Temp);

        foreach (var(transform, bee) in SystemAPI.Query<TransformAspect, RefRW<Bee>>().WithAny<BlueBee, YellowBee>())
        {
            switch (bee.ValueRW.beeState)
            {
                case Bee.BEESTATE.IDLE:
                    ExecuteIdleState(foodEntities, foodTranslations, ref bee.ValueRW);
                    break;
                case Bee.BEESTATE.FORAGE:
                    ExecuteForageState(transform, ref bee.ValueRW, ref state);
                    break;
                case Bee.BEESTATE.CARRY:
                    ExecuteCarryState(transform, foodTranslations, ref bee.ValueRW, ref state);
                    break;
                case Bee.BEESTATE.ATTACK:
                    break;
                default:
                    break;
            }
        }
    }
}