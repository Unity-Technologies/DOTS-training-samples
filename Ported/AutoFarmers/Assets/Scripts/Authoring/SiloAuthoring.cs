using Unity.Entities;

class SiloAuthoring : UnityEngine.MonoBehaviour
{
    public int Cash;
    public int FarmerCost;
    public int FarmersSpawned;
    public int DroneCost;
    public int DroneUnlockLevel;
    public byte HireType;

    class SiloBaker : Baker<SiloAuthoring>
    {
        public override void Bake(SiloAuthoring authoring)
        {
            AddComponent(new Silo
            {
                Cash = 0,
                FarmerCost = authoring.FarmerCost,
                DroneCost = authoring.DroneCost,
                HireType = HireTypes.HIRE_FARMER,
                FarmersSpawned = 0,
                DronesSpawned = 0,
                DroneUnlockLevel = authoring.DroneUnlockLevel
            });
        }
    }
}

struct Silo : IComponentData
{
    public int Cash;
    public int FarmerCost;
    public int FarmersSpawned;
    public int DroneCost;
    public int DronesSpawned;
    public int DroneUnlockLevel;
    public byte HireType;
}

public static class HireTypes
{
    public const byte HIRE_FARMER = 0;
    public const byte HIRE_DRONE = 1;
}