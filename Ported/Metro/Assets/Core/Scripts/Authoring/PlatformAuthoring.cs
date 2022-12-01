using Unity.Entities;
using Unity.Mathematics;

class PlatformAuthoring : UnityEngine.MonoBehaviour
{
}
 
class PlatformBaker : Baker<PlatformAuthoring>
{
    public override void Bake(PlatformAuthoring authoring)
    {
        AddComponent<PlatformTag>();
        var queues = AddBuffer<PlatformQueue>();
        //queues.Capacity = 9;
        //queues.ResizeUninitialized(9);
    }
}
