using Unity.Entities;
using Unity.Mathematics;

class FarmerAuthoring : UnityEngine.MonoBehaviour
{
    class FarmerBaker : Baker<FarmerAuthoring>
    {
        public override void Bake(FarmerAuthoring authoring)
        {
            AddComponent(new Farmer()
            {
                moveSpeed = 5.0f
            });
        }
    }
}

struct Farmer : IComponentData
{
    public float3 moveTarget;
    public float moveSpeed;
}