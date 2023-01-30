using Unity.Entities;
using UnityEngine;

public class CarOvertakeStateAuthoring : MonoBehaviour
{
    class CarOvertakeStateBaker : Baker<CarOvertakeStateAuthoring>
    {
        public override void Bake(CarOvertakeStateAuthoring authoring)
        {
            // By default, components are zero-initialized.
            // So in this case, the Speed field in CannonBall will be float3.zero.
            AddComponent<CarOvertakeState>(new CarOvertakeState {OvertakeStartTime = 0.0f, OriginalLane = 0, TargetLane = 0});
        }
    }
}




struct CarOvertakeState : IComponentData
{
    public float OvertakeStartTime;
    public int OriginalLane, TargetLane;
    
}