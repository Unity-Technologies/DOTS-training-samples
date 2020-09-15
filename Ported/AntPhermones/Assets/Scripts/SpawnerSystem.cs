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

            for (int i = 0; i < spawner.NumberOfAnts; i++)
            {
                var instance = EntityManager.Instantiate(spawner.Ant);
                SetComponent(instance, new Translation{ Value = ltw.Position });
                SetComponent(instance, new Yaw { Value = random.NextFloat(0.0f, math.PI * 2.0f) });
            }

            EntityManager.RemoveComponent<Spawner>(entity);
        }).Run();
    }
}
