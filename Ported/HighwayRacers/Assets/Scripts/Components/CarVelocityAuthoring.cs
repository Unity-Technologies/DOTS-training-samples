using Unity.Entities;
using UnityEngine;

public class CarVelocityAuthoring : MonoBehaviour
{
    class CarVelocityBaker : Baker<CarVelocityAuthoring>
    {
        public override void Bake(CarVelocityAuthoring authoring)
        {
            // By default, components are zero-initialized.
            // So in this case, the Speed field in CannonBall will be float3.zero.
            AddComponent<CarVelocity>(new CarVelocity {VelX = 0.0f, VelY = 2.0f});
        }
    }
}


struct CarVelocity : IComponentData
{
    public float VelX, VelY;
}