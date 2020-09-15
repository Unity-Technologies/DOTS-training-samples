using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities.WithStructuralChanges().ForEach((Entity entity, in Spawner spawner, in LocalToWorld ltw) =>
        {
            Random random = new Random(1337);
            random.NextInt();
            random.NextInt();
            random.NextInt();

            for (int i = 0; i < spawner.NumberOfAnts; i++)
            {
                var instance = EntityManager.Instantiate(spawner.Ant);
                float initYaw = random.NextFloat(-math.PI, math.PI);
                SetComponent(instance, new Translation{ Value = ltw.Position });
                SetComponent(instance, new Yaw { CurrentYaw =  initYaw });
                SetComponent(instance, new SteeringComponent { DesiredYaw = initYaw });
            }

            EntityManager.RemoveComponent<Spawner>(entity);
        }).Run();
    }
}
