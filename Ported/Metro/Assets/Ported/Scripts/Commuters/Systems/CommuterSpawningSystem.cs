#if ENABLE_COMMUTERS
using MetroECS;
using MetroECS.Comuting;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateBefore(typeof(PlatformGeneration))]
public class CommuterSpawningSystem : SystemBase
{
    private bool firstFrameComplete;
    
    protected override void OnUpdate()
    {
        var commuterPrefab = GetSingleton<MetroData>().CommuterPrefab;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var random = new Random(1234);

        Entities.ForEach((in LocalToWorld localToWorld, in Translation translation, in PlatformQueueData zone) =>
        {
            for (var c = 0; c < 5; c++)
            {
                var queuePosition = localToWorld.Position;
                var commuterEntity = ecb.Instantiate(commuterPrefab);
                var commuterTranslation = new Translation {Value = queuePosition};
                ecb.SetComponent(commuterEntity, commuterTranslation);

                var commuter = new Commuter {movementSpeed = random.NextFloat(.5f, 1.5f)};
                ecb.SetComponent(commuterEntity, commuter);

                var platformData = GetComponent<PlatformData>(zone.Platform);
                // var connections = GetBuffer<PlatformConnection>(zone.Platform);
                
                var fStationBottom = GetComponent<LocalToWorld>(platformData.FrontStairsBottom);
                var fStationTop = GetComponent<LocalToWorld>(platformData.FrontStairsTop);

                var moveBuffer = ecb.AddBuffer<MoveTarget>(commuterEntity);
                moveBuffer.Add(new MoveTarget {Position = fStationBottom.Position});
                moveBuffer.Add(new MoveTarget {Position = fStationTop.Position});
                // if (connections.Length > 0)
                // {
                //     var connection = connections[0].Value;
                //     var connectionPathData = GetComponent<PlatformData>(connection);
                //     moveBuffer.Add(new MoveTarget {Position = GetComponent<Translation>(connectionPathData.BackStairsTop).Value});
                //     moveBuffer.Add(new MoveTarget {Position = GetComponent<Translation>(connectionPathData.BackStairsBottom).Value});
                // }

                var movingTag = new MovingTag();
                ecb.AddComponent(commuterEntity, movingTag);
            }
        }).Run();

        ecb.Playback(EntityManager);

        if (firstFrameComplete)
            Enabled = false;
        
        firstFrameComplete = true;
    }
}
#endif