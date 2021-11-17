using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class KillAllBees : SystemBase
{
    Random random = new Random(3045);

    protected override void OnCreate()
    {
        this.RequireSingletonForUpdate<Globals>();
    }

    protected override void OnUpdate()
    {
        if (Time.ElapsedTime < 6.0)
            return;

        //var globals = GetSingletonEntity<Globals>();
        //var globalsComponent = GetComponent<Globals>(globals);

        //Entities
        //    .WithStructuralChanges()
        //    .ForEach((Entity entity, in Bee bee, in Translation translation, in Velocity velocity) =>
        //{
        //    EntityManager.AddComponentData(entity, new Gravity());

        //    int totalGiblets = random.NextInt(5, 10);
        //    for (int i = 0; i < totalGiblets; ++i)
        //    {
        //        var giblet = EntityManager.Instantiate(spawnerComponent.GibletPrefab);
        //        EntityManager.SetComponentData<Translation>(giblet, translation);
        //        EntityManager.SetComponentData<Velocity>(giblet, new Velocity
        //        {
        //            Value = velocity.Value + random.NextFloat3Direction() * 2.0f
        //        });
        //    }

        //}).Run();

        Enabled = false;
    }
}
