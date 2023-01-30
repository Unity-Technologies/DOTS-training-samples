using Unity.Entities;
using UnityEngine;

public class CarIsOvertakingAuthoring : MonoBehaviour
{
    class CarIsOvertakingBaker : Baker<CarIsOvertakingAuthoring>
    {
        public override void Bake(CarIsOvertakingAuthoring authoring)
        {
            // By default, components are zero-initialized.
            // So in this case, the Speed field in CannonBall will be float3.zero.
            AddComponent<CarIsOvertaking>(new CarIsOvertaking {IsOvertaking = false});
        }
    }
}




struct CarIsOvertaking : IComponentData
{
    public bool IsOvertaking;

}