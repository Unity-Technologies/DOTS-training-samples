using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public partial class PlatformGenerationSystem : SystemBase
{
    private EntityQuery platformEnds;

    protected override void OnCreate()
    {
        platformEnds = GetEntityQuery(typeof(PlatformEndTag));
    }

    protected override void OnUpdate()
    {
        var config = SystemAPI.GetSingleton<Config>();
        var entityManager = World.EntityManager;

        Entities
            .WithStructuralChanges()
            .WithAll<PlatformEndTag>()
            .ForEach((in RailMarker railMarker, in Translation translation, in Rotation rotation) =>
            {
                var platformInstance = entityManager.Instantiate(config.PlatformPrefab);
                
                var platformTranslation = new Translation {Value = translation.Value};
                SetComponent(platformInstance, platformTranslation);
                
                var platformRotation = new Rotation {Value = rotation.Value};
                SetComponent(platformInstance, platformRotation);
                
                var platformData = GetComponent<Platform>(platformInstance);
                platformData.Line = railMarker.Line;
                SetComponent(platformInstance, platformData);
            }).Run();
        
        Enabled = false;
    }
}