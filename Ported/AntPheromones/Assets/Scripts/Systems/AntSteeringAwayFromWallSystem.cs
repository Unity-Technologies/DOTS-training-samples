using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class AntSteeringAwayFromWallSystem : SystemBase
{
    const float mapSize = 128f;
    const int bucketResolution = 32;

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity bucketIndicesEntity;
        if (!TryGetSingletonEntity<ObstacleBucketEntity>(out bucketIndicesEntity))
        {
            return;
        }

        var bucketIndices = GetBuffer<ObstacleBucketIndices>(bucketIndicesEntity);
        var buckets = GetBuffer<ObstacleBucket>(bucketIndicesEntity);

        Entities
            .WithAny<Ant>()
            .ForEach((ref FacingAngle facingAngle, in Translation translation) =>
            {
                int output = 0;
                for (int i = -1; i <= 1; i += 2)
                {
                    float angle = facingAngle.Value + i * math.PI * .25f;
                    float testX = translation.Value.x + math.cos(angle) * 1.5f;
                    float testY = translation.Value.y + math.sin(angle) * 1.5f;

                    if (testX < 0 || testY < 0 || testX >= mapSize || testY >= mapSize) { }
                    else
                    {

                        int x = (int)(testX / mapSize * bucketResolution);
                        int y = (int)(testY / mapSize * bucketResolution);
                        if (x < 0 || y < 0 || x >= bucketResolution || y >= bucketResolution)
                        { }
                        else
                        {
                            int bucketIndex = x * bucketResolution + y;
                            Entity bucketEntity = bucketIndices[bucketIndex];
                            var obstacleBucket = GetBuffer<ObstacleBucket>(bucketEntity);
                            if (obstacleBucket.Length > 0)
                            {
                                output -= i;
                            }
                        }
                    }
                }

                //facingAngle.Value += output;
            }).Run();


        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
