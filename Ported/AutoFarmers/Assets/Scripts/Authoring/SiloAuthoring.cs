using Unity.Entities;

class SiloAuthoring : UnityEngine.MonoBehaviour
{
    public int Cash;
    public int FarmerCost;
    public int DroneCost;
    public UnityEngine.Transform FarmerSpawn;
    public UnityEngine.Transform DroneSpawn;

    class SiloBaker : Baker<SiloAuthoring>
    {
        public override void Bake(SiloAuthoring authoring)
        {
            AddComponent(new Silo
            {
                Cash = authoring.Cash,
                FarmerCost = authoring.FarmerCost,
                DroneCost = authoring.DroneCost,
                FarmerSpawn = GetEntity(authoring.FarmerSpawn),
                DroneSpawn = GetEntity(authoring.DroneSpawn)
            }) ;
        }
    }
}

struct Silo : IComponentData
{
    public int Cash;
    public int FarmerCost;
    public int DroneCost;
    public Entity FarmerSpawn;
    public Entity DroneSpawn;
}