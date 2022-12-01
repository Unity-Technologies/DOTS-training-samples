using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace Authoring
{
    public class CarriageAuthoring : MonoBehaviour
    {
        public BoxCollider collider;
        public GameObject[] seats;
    }

    public class CarriageBaker : Baker<CarriageAuthoring>
    {
        public override void Bake(CarriageAuthoring authoring)
        {
            AddComponent(new CarriageBounds
            {
                Width = authoring.collider.size.z + 0.5f
            });
            AddComponent<URPMaterialPropertyBaseColor>();

            var position = authoring.transform.position;
            var seats = new NativeArray<float3>(authoring.seats.Length, Allocator.Persistent);
            for (int i = 0; i < seats.Length; i++)
            {
                var worldSeatPosition = authoring.seats[i].transform.position;
                seats[i] = worldSeatPosition - position;
            }
            AddComponent(new CarriageSeatsPositions
            {
                Seats = seats
            });
            AddBuffer<CarriagePassengers>();
        }
    }
}