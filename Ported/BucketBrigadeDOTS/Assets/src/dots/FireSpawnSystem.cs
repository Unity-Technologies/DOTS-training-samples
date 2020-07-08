using Unity.Entities;

public class FireSpawnSystem : SystemBase
{
    protected override void OnCreate()
    {
        var fireGridSetting = GetSingleton<FireGridSettings>();
        var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        var fireGrid = EntityManager.AddBuffer<FireCell>(fireGridEntity);
        fireGrid.Capacity = fireGridSetting.FireGridResolution.x * fireGridSetting.FireGridResolution.y;
    }

    protected override void OnUpdate()
    {
             
    }
}
