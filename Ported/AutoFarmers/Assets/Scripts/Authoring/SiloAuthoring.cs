using Unity.Entities;

class SiloAuthoring : UnityEngine.MonoBehaviour
{
    public int Cash;
    public int FarmerCost;
    public int DroneCost;
    public byte HireType;

    class SiloBaker : Baker<SiloAuthoring>
    {
        public override void Bake(SiloAuthoring authoring)
        {
            AddComponent(new Silo
            {
                Cash = authoring.Cash,
                FarmerCost = authoring.FarmerCost,
                DroneCost = authoring.DroneCost,
                HireType = HireTypes.HIRE_FARMER
            });
        }
    }
}

struct Silo : IComponentData
{
    public int Cash;
    public int FarmerCost;
    public int DroneCost;
    public byte HireType;
}

public static class HireTypes
{
    public const byte HIRE_FARMER = 0;
    public const byte HIRE_DRONE = 1;
}