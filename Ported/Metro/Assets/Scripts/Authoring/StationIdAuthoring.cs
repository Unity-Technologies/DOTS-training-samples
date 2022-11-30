using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class StationIdAuthoring : MonoBehaviour
{
    //public int Value;
}
class StationIdBaker : Baker<StationIdAuthoring>
{
    public override void Bake(StationIdAuthoring authoring)
    {
        AddComponent(new StationId{ Value = 0 });
    }
}