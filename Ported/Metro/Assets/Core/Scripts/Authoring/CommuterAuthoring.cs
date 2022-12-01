using Unity.Entities;
using Unity.Mathematics;

class CommuterAuthoring : UnityEngine.MonoBehaviour
{
}
 
class CommuterBaker : Baker<CommuterAuthoring>
{
    public override void Bake(CommuterAuthoring authoring)
    {
        AddComponent<CommuterTag>();

    }
}