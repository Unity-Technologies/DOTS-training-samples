using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class ActorGoalSystem : SystemBase
{
    private Random mRand;
    protected override void OnCreate()
    {
        GetEntityQuery(ComponentType.ReadWrite<Actor>(), ComponentType.Exclude<Destination>());
        mRand = new Random(1337);
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var rand = mRand;
        Entities
            .WithName("Actor_Goals")
            .WithAll<Actor>()
            .WithNone<Destination>()
            .ForEach((Entity actor, in Translation currentPos) =>
            {
                var randomCircle = (rand.NextFloat2() - 1) * 2;
                randomCircle *= rand.NextFloat() * 5;
                Destination dest = new Destination()
                {
                    position = currentPos.Value + new float3(randomCircle.x, 0f, randomCircle.y)
                };

                ecb.AddComponent(actor, dest);
            }).Run();

        mRand = rand;
        ecb.Playback(EntityManager);
    }
}
