using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(FarmerIntentionSystem))]
public class SmashRocksSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = Time.DeltaTime;

        //Testing
        var rocks = GetEntityQuery(typeof(Rock)).ToEntityArray(Allocator.Temp);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SmashRock added");
            Entities.WithAll<Farmer>()
                .ForEach((Entity entity, ref SmashRocks smashRocks) => { smashRocks.TargetRock = rocks[0]; })
                .WithoutBurst().Run();
        }

        var entityManager = EntityManager;

        Entities.WithAll<Farmer>().WithNone<Searching>()
            .ForEach((Entity entity, in SmashRocks smashRocks) =>
            {
                if (entityManager.Exists(smashRocks.TargetRock))
                {
                    var rock = GetComponent<Rock>(smashRocks.TargetRock);
                    // var rock = entityManager.GetComponentData<Rock>(smashRocks.TargetRock);
                    var health = rock.Health - (0.5f * deltaTime);
                    ecb.SetComponent(smashRocks.TargetRock,
                        new Rock {Health = health, Position = rock.Position, Size = rock.Size});
    
                    if (rock.Health <= 0)
                    {
                        ecb.RemoveComponent<SmashRocks>(entity);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}