using Unity.Entities;
using Unity.Mathematics;

public static class BucketBrigadeUtility
{
    public static bool IsVeryClose(float2 a, float2 b)
    {
        return math.distancesq(a, b) < 0.01f;
    }
    
    public static bool IsEntityVeryClose(EntityManager entityManager, float2 myPosition, Entity other)
    {
        var otherPosition = entityManager.GetComponentData<Position>(other);

        return IsVeryClose(myPosition, otherPosition.Value);
    }

    public static void PickUpBucket(EntityManager entityManager, ref BucketHeld bucketHeld, ref BucketToWant bucketWanted)
    {
        bucketHeld.Value = bucketWanted.Value;
        var bucketState = entityManager.GetComponentData<MyBucketState>(bucketHeld.Value);

        switch (bucketState.Value)
        {
            case BucketState.EmptyOnGround:
                bucketState.Value = BucketState.EmptyCarrried;
                break;

            case BucketState.FullOnGround:
                bucketState.Value = BucketState.FullCarried;
                break;
        }
        
        entityManager.SetComponentData(bucketHeld.Value, bucketState);
    }

    public static void DropBucket(EntityManager entityManager, ref BucketHeld bucketHeld)
    {
        var bucketState = entityManager.GetComponentData<MyBucketState>(bucketHeld.Value);

        switch (bucketState.Value)
        {
            case BucketState.EmptyCarrried:
                bucketState.Value = BucketState.EmptyOnGround;
                break;

            case BucketState.FullCarried:
                bucketState.Value = BucketState.FullOnGround;
                break;
        }
        
        entityManager.SetComponentData(bucketHeld.Value, bucketState);
    }
    
    public static bool IsBucketFull(EntityManager entityManager, BucketHeld bucketHeld)
    {
        var bucketState = entityManager.GetComponentData<MyBucketState>(bucketHeld.Value);

        switch (bucketState.Value)
        {
            case BucketState.FullCarried:
            case BucketState.FullOnGround:
                return true;
        }

        return false;
    }
    
    public static bool IsBucketFull(BucketState bucketState)
    {
        switch (bucketState)
        {
            case BucketState.FullCarried:
            case BucketState.FullOnGround:
                return true;
        }

        return false;
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
}