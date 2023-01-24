using Unity.Entities;

class PlantAuthoring : UnityEngine.MonoBehaviour
{
    class PlantBaker : Baker<PlantAuthoring>
    {
        public override void Bake(PlantAuthoring authoring)
        {
            AddComponent<Plant>();
        }
    }
}

struct Plant : IComponentData
{
     
}
