using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BeamCreate : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in BeamData beam, in LocalToWorld ltw) =>
            {
                for (int x = 0; x < beam.CountX; ++x)
                    for (int z = 0; z < beam.CountZ; ++z)
                    {
                    //Random.Range(-45f, 45f),0f,Random.Range(-45f, 45f))
                        var posX = 2 * (x - (beam.CountX - 1) / 2);
                        var posY = .5f;
                        var posZ = 2 * (z - (beam.CountZ - 1) / 2);
                        //var posX = Random.Range(-45f, 45f);
                        //var posY = Random.Range(.5f, 45f);
                        //var posZ = Random.Range(-45f, 45f);
                        var instance = EntityManager.Instantiate(beam.Prefab);
                        SetComponent(instance, new Translation
                        {
                            Value = ltw.Position + new float3(posX, posY, posZ)
                        });
                    }

                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}
