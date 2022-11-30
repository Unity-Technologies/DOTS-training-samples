using Unity.Entities;
using UnityEngine;

public struct StationId : IComponentData
{
    public int Value;
}

[DisallowMultipleComponent]
public class StationIdAuthoring : MonoBehaviour
{
    [RegisterBinding(typeof(StationId), "Value", true)]
    public int Value;

    class StationIdBaker : Baker<StationIdAuthoring>
    {
        public override void Bake(StationIdAuthoring authoring)
        {
            AddComponent(new StationId{ Value = authoring.Value});
        }
    }
}