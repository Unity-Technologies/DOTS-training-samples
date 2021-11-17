using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class EggSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var beeData = GetComponentDataFromEntity<Bee>(false);

        Entities
            .WithNone<Gravity>()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Food food, in Translation translation) =>
        {
            if (translation.Value.x < -15 || translation.Value.x > 15)
            {
                if (food.CarriedBy != Entity.Null)
                {
                    beeData[food.CarriedBy] = new Bee { Carried = Entity.Null, Mode = Bee.ModeCategory.Searching };
                    food.CarriedBy = Entity.Null;
                }

                EntityManager.AddComponent<Gravity>(entity);
                EntityManager.RemoveComponent<Decay>(entity);
                EntityManager.AddComponentData<Velocity>(entity, new Velocity());
            }
        }).Run();
    }
}
