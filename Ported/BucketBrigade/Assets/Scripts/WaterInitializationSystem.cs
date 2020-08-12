using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class WaterInitializationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((in Entity e, in WaterInitialization init, in Water water) =>
            {
                /*
                var instance = EntityManager.Instantiate(water.prefab);
                SetComponent(instance, new Translation()
                {
                    Value = water.prefab.  
                });
                EntityManager.AddComponentData(instance, new BotColor()
                {
                    Value = colors.emptyColor
                });

                // remove the setup data
                EntityManager.DestroyEntity(e);
                */
            }).Run();
    }
}
