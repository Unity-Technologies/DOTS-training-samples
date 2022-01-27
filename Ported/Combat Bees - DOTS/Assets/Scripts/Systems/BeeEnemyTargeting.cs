using Unity.Entities;
using Unity.Transforms;

public partial class BeeEnemyTargeting : SystemBase
{
    protected override void OnUpdate()
    {

        
        Entities.ForEach((ref Translation translation, in Rotation rotation) => {

        }).Schedule();
    }
}
