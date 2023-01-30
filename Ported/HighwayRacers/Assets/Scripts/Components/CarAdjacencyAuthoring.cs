using Unity.Entities;
using UnityEngine;

public class CarAdjacencyAuthoring : MonoBehaviour
{
    class CarAdjacencyBaker : Baker<CarAdjacencyAuthoring>
    {
        public override void Bake(CarAdjacencyAuthoring authoring)
        {
            // By default, components are zero-initialized.
            // So in this case, the Speed field in CannonBall will be float3.zero.
            AddComponent<CarAdjacency>(new CarAdjacency {Left = false, Right = false, Front = false});
        }
    }
}




struct CarAdjacency : IComponentData
{
    public bool Left, Right, Front;

}