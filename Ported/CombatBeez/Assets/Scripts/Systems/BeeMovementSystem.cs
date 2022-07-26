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

    //Modified each update frame
    private float frameCount;
    private float dt;
    private Random rand;
    private EntityQuery foodEntities;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        moveSpeed = 8f;
        ocilateMag = 20f;

        var queryBuilder = new EntityQueryDescBuilder(Allocator.Temp);
        queryBuilder.AddAll(ComponentType.ReadWrite<Translation>());
        queryBuilder.AddAll(ComponentType.ReadWrite<FoodResource>());
        queryBuilder.FinalizeQuery();

        foodEntities = state.GetEntityQuery(queryBuilder);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void ExecuteIdleState(NativeArray<Translation> foodLocations, ref Bee bdata)
    {
        //This state picks one of the other states to go to...
        //This function picks a resource to move towards and then moves to the execute forage state
        //bee.beeState = BEESTATE.FORAGE;
        //this will need a foreach to get a resource to gather, then change states depending

        Translation foodPoint = foodLocations[rand.NextInt(foodLocations.Length - 1)];
        bdata.Target = foodPoint.Value;
        bdata.beeState = Bee.BEESTATE.FORAGE;
    }

    [BurstCompile]
    public void ExecuteForageState(TransformAspect t, ref Bee bd)
    {
        if (MoveToTarget(t, ref bd))
            bd.beeState = Bee.BEESTATE.CARRY;
    }

    [BurstCompile]
    public void ExecuteCarryState()
    {

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
            //if (beeData.Target.x == 0 &&
            //    beeData.Target.y == 0 &&
            //    beeData.Target.z == 0)
            //{
            //    beeData.Target = rand.NextFloat3(-50, 50);
            //}
            //else
            //{
            //    beeData.Target = float3.zero;
            //}
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

        //worldupdateallocator - anything allocated to it will get passed to jobs, but you don't have to manually deallocate, they will
        //dispose of themselves with the world/every 2 frames
        var foodTranslations = foodEntities.ToComponentDataArray<Translation>(state.WorldUpdateAllocator);
        //foodEntities.ToEntityArray(Allocator.Temp);

        foreach (var(transform, bee) in SystemAPI.Query<TransformAspect, RefRW<Bee>>().WithAny<BlueBee, YellowBee>())
        {
            switch (bee.ValueRW.beeState)
            {
                case Bee.BEESTATE.IDLE:
                    ExecuteIdleState(foodTranslations, ref bee.ValueRW);
                    break;
                case Bee.BEESTATE.FORAGE:
                    ExecuteForageState(transform, ref bee.ValueRW);
                    break;
                case Bee.BEESTATE.CARRY:
                    break;
                case Bee.BEESTATE.ATTACK:
                    break;
                default:
                    break;
            }
        }
    }
}