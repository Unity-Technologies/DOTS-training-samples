using Unity.Entities;
using Unity.Transforms;

public partial class UpdateStateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // TODO: Use state related components here
        Entities.ForEach((ref Translation translation, in Rotation rotation) => {
        }).Schedule();
    }
}