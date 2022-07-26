using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class CarBootstrapAuthoring : MonoBehaviour
    {
        public int CarsOnLaneToInitialise;

        public GameObject CarPrefab;
    }

    public class CarBootstrapBaker : Baker<CarBootstrapAuthoring>
    {
        public override void Bake(CarBootstrapAuthoring authoring)
        {
            AddComponent(new CarBootstrap
            {
                CarPrefab = GetEntity(authoring.CarPrefab),
                CarsToInitialise = authoring.CarsOnLaneToInitialise
            });
        }
    }
}
