using Unity.Entities;
using Unity.Mathematics;

public static class BucketBrigadeUtility
{
    public const float FreeSpeed = 2f;
    public const float EmptyBucketSpeed = 2f;
    public const float FullBucketSpeed = 2f / 3f;

    private static int _frame;

    public static void IncrementFrame()
    {
        _frame++;
    }
    
    public static int GetCurrentFrame()
    {
        return _frame;
    }
    
    public static bool IsVeryClose(float2 a, float2 b)
    {
        return math.distancesq(a, b) < 0.01f;
    }
    
    public static bool IsEntityVeryClose(EntityManager entityManager, float2 myPosition, Entity other)
    {
        var otherPosition = entityManager.GetComponentData<Position>(other);

        return IsVeryClose(myPosition, otherPosition.Value);
    }

    public static void MarkCarriedBucketAsFull(EntityCommandBuffer ecb, ref BucketHeld bucketHeld, ref Speed speed, int frame)
    {
        MyBucketState bucketState;

        bucketState.Value = BucketState.FullCarried;
        bucketState.FrameChanged = frame;
        bucketHeld.IsFull = true;
        speed.Value = FullBucketSpeed;
        
        ecb.SetComponent(bucketHeld.Value, bucketState);
    }

    public static void MarkCarriedBucketAsEmpty(EntityCommandBuffer ecb, ref BucketHeld bucketHeld, ref Speed speed, int frame)
    {
        MyBucketState bucketState;

        bucketState.Value = BucketState.EmptyCarrried;
        bucketState.FrameChanged = frame;
        bucketHeld.IsFull = false;
        speed.Value = EmptyBucketSpeed;
        
        ecb.SetComponent(bucketHeld.Value, bucketState);
    }

    // requires sanity check to avoid conflict
    public static void PickUpBucket(EntityManager entityManager, ref BucketHeld bucketHeld, ref BucketToWant bucketWanted, ref Speed speed, int frame)
    {
        bucketHeld.Value = bucketWanted.Value;
        var bucketState = entityManager.GetComponentData<MyBucketState>(bucketHeld.Value);

        switch (bucketState.Value)
        {
            case BucketState.EmptyOnGround:
                bucketState.Value = BucketState.EmptyCarrried;
                bucketHeld.IsFull = false;
                speed.Value = EmptyBucketSpeed;
                break;

            case BucketState.FullOnGround:
                bucketState.Value = BucketState.FullCarried;
                bucketHeld.IsFull = true;
                speed.Value = FullBucketSpeed;
                break;
        }

        bucketState.FrameChanged = frame;
        
        entityManager.SetComponentData(bucketHeld.Value, bucketState);
    }

    public static void DropBucket(EntityCommandBuffer ecb, ref BucketHeld bucketHeld, ref Speed speed, int frame)
    {
        speed.Value = FreeSpeed;

        MyBucketState bucketState;

        if (bucketHeld.IsFull)
        {
            bucketState.Value = BucketState.FullOnGround;
        }
        else
        {
            bucketState.Value = BucketState.EmptyOnGround;
        }

        bucketState.FrameChanged = frame;

        ecb.SetComponent(bucketHeld.Value, bucketState);
        bucketHeld.Value = Entity.Null;
    }
    
    public static void GiveBucketByDrop(EntityCommandBuffer ecb, Entity worker, ref BucketHeld bucketHeld, ref Speed speed, int frame)
    {
        ecb.SetComponent(worker, new BucketToWant() { Value = bucketHeld.Value });
        DropBucket(ecb, ref bucketHeld, ref speed, frame);
    }
    
    public static void GiveBucketByDrop(EntityCommandBuffer ecb, DestinationWorker worker, ref BucketHeld bucketHeld, ref Speed speed, int frame)
    {
        GiveBucketByDrop(ecb, worker.Value, ref bucketHeld, ref speed, frame);
    }

    public static bool IsBucketCarried(BucketState bucketState)
    {
        switch (bucketState)
        {
            case BucketState.FullCarried:
            case BucketState.EmptyCarrried:
                return true;
        }

        return false;
    }
    
    public static (Entity entity, float2 position) FindClosestWater(float2 position, DynamicBuffer<WaterPoolInfo> waterInfo)
    {
        var closest = Entity.Null;
        var closestPosition = float2.zero;
        var distanceSq = float.PositiveInfinity;

        for (var i = 0; i < waterInfo.Length; i++)
        {
            var element = waterInfo[i];

            var candidateDistanceSq = math.distancesq(position, element.Position);

            if (candidateDistanceSq < distanceSq)
            {
                distanceSq = candidateDistanceSq;
                closest = element.WaterPool;
                closestPosition = element.Position;
            }
        }

        return (closest, closestPosition);
    }
    
    public static (Entity entity, float2 position) FindClosestBucket(float2 position, DynamicBuffer<FreeBucketInfo> bucketInfo, bool mustBeEmpty)
    {
        var closest = Entity.Null;
        var closestPosition = float2.zero;
        var distanceSq = float.PositiveInfinity;

        for (var i = 0; i < bucketInfo.Length; i++)
        {
            var element = bucketInfo[i];

            if (!mustBeEmpty || element.BucketState.Value == BucketState.EmptyOnGround)
            {
                var candidateDistanceSq = math.distancesq(position, element.BucketPosition.Value);

                if (candidateDistanceSq < distanceSq)
                {
                    distanceSq = candidateDistanceSq;
                    closest = element.BucketEntity;
                    closestPosition = element.BucketPosition.Value;
                }
            }
        }

        return (closest, closestPosition);
    }
    
    public static float2 CalculateLeftArc(float2 a, float2 b, float t)
    {
        var ab = b - a;

        return a + (ab * t) + (new float2(-ab.y, ab.x) * ((1f - t) * t * 0.3f));
    }
    
    public static DynamicBuffer<HeatMapTemperature> GetHeatmapBuffer(ComponentSystemBase systemBase) 
    {
        var heatmap = systemBase.GetSingletonEntity<HeatMapTemperature>();
        return systemBase.EntityManager.GetBuffer<HeatMapTemperature>(heatmap);
    }

    public static HeatMapData GetHeatmapData(ComponentSystemBase systemBase)
    {
        return systemBase.GetSingleton<HeatMapData>();
    }
}