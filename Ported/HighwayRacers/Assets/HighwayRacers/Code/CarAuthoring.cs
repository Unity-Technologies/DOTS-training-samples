using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CarAuthoring : MonoBehaviour
{
    class Baker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent<Car>();
        }
    }
}

public struct Car : IComponentData
{
}