using Unity.Entities;
using Unity.Transforms;

public partial class PlatformGenerationSystem : SystemBase
{
    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        var config = SystemAPI.GetSingleton<Config>();
        var entityManager = World.EntityManager;
        
        Entities
            .WithStructuralChanges()
            .WithAll<PlatformEndTag>()
            .ForEach((in Translation translation, in Rotation rotation) =>
            {
                var platformInstance = entityManager.Instantiate(config.PlatformPrefab);
                var platformTranslation = new Translation {Value = translation.Value};
                var platformRotation = new Rotation {Value = rotation.Value};
                
                entityManager.SetComponentData(platformInstance, platformTranslation);
                entityManager.SetComponentData(platformInstance, platformRotation);
            }).Run();
        
        Enabled = false;
    }
}