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
                moveSpeed = 5.0f,
                moveTarget = new float3(50, 0, 50),
                farmerState = FarmerStates.FARMER_STATE_ROCKDESTROY
            });
        }
    }
}

struct Farmer : IComponentData
{
    public float3 moveTarget;
    public float moveSpeed;
    public byte farmerState;
}

public static class FarmerStates
{
    public const byte FARMER_STATE_IDLE = 0;
    public const byte FARMER_STATE_ROCKDESTROY = 1;
    public const byte FARMER_STATE_CREATEPLOT = 2;
    public const byte FARMER_HARVEST = 3;
}