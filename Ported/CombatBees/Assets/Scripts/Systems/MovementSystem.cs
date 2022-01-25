using Unity.Entities;
using Unity.Transforms;

public partial class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation /*, in Velocity velocity */) => {
        }).Schedule();
    }
}
