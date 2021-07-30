using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class AntSteeringAwayFromWallSystem : SystemBase
{
    const int bucketResolution = 50;
    protected override void OnCreate()
    {
        var query = GetEntityQuery(typeof(ObstacleBucketEntity));
        RequireForUpdate(query);
    }

    protected override void OnUpdate()
    {
        Entity obstacleBucketEntity;
        bool success = TryGetSingletonEntity<ObstacleBucketEntity>(out obstacleBucketEntity);
        var mapSize = GetComponent<MapSetting>(GetSingletonEntity<MapSetting>()).WorldSize;

        if (!success)
        {
            return;
        }

        using (var ecb = new EntityCommandBuffer(Allocator.Temp))
        {
            Entities
                .WithStructuralChanges()
                .WithAny<Ant>()
                .ForEach((ref FacingAngle facingAngle, in Translation translation) =>
                {
                    var bucketIndicesBuffer = GetBuffer<ObstacleBucketIndices>(obstacleBucketEntity);
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
                                int bucketEntityIndex = x * bucketResolution + y;
                                var bucketEntity = bucketIndicesBuffer[bucketEntityIndex];
                                var buffer = GetBuffer<ObstacleBucket>(bucketEntity);
                                if (buffer.Length > 0)
                                {
                                    output -= i;
                                }

                                //var lookup = GetBufferFromEntity<ObstacleBucket>();
                                //var bucketEntity = bucketIndicesBuffer[bucketEntityIndex];

                                //Entity bucketEntity = bucketIndices[bucketIndex].Value;
                                //var obstacleBucket = GetBuffer<ObstacleBucket>(bucketEntity);
                                //if (obstacleBucket.Length > 0)
                                //{
                                //    output -= i;
                                //}
                            }
                        }
                    }

                    facingAngle.Value += output;
                }).Run();
            ecb.Playback(EntityManager);
        }

    }
}
