using Unity.Entities;

[UpdateAfter(typeof(FarmerIntentionSystem))]
public class SmashRocksSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        /*Entities.WithAll<Farmer>().WithNone<Searching>()
            .ForEach((Entity entity, ref SmashRocks smashRocks) =>
            {
                var rock = EntityManager.GetComponentData<Rock>(smashRocks.TargetRock);
                rock.Health -= 0.1f * deltaTime;
            }).WithoutBurst().Run();*/
    }
}