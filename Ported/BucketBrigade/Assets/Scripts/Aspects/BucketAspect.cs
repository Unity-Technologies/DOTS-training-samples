using Unity.Entities;

readonly partial struct BucketAspect : IAspect<BucketAspect>
{
    //Buckets have a few properties. Transform and WaterLevel. The latter goes from 0 to 1, a float. 
    private readonly RefRO<Bucket> bucket;

    public float fillLevel => bucket.ValueRO.fillLevel;
}