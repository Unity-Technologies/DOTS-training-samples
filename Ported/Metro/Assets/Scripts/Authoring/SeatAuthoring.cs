using Unity.Entities;
using UnityEngine;

public class SeatAuthoring : MonoBehaviour
{
    class Baker : Baker<SeatAuthoring>
    {
        public override void Bake(SeatAuthoring authoring)
        {
            AddComponent<Seat>();
        }
    }
}

public struct Seat : IComponentData
{
    public bool IsTaken;
}
