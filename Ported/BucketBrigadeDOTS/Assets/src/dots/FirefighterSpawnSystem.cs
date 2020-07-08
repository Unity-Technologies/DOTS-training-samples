using System.ComponentModel.Design.Serialization;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FirefighterSpawnSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in FirefighterSpawner spawner, in LocalToWorld ltw) =>
            {
                int firefigherID = 0;
                int firefighterCount = spawner.CountX * spawner.CountZ;
                
                for (int x = 0; x < spawner.CountX; ++x)
                for (int z = 0; z < spawner.CountZ; ++z)
                {
                    var posX = 2 * (x - (spawner.CountX - 1) / 2);
                    var posZ = 2 * (z - (spawner.CountZ - 1) / 2);
                    var instance = EntityManager.Instantiate(spawner.Prefab);
                    SetComponent<Translation2D>(instance, new Translation2D { Value = ltw.Position.xz + new float2(posX, posZ) });
                    EntityManager.AddComponent<Firefighter>(instance);
                    EntityManager.AddComponentData<FirefighterPositionInLine>(instance, new FirefighterPositionInLine { Value = (float)firefigherID/firefighterCount });
                    firefigherID++;
                }

                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}
