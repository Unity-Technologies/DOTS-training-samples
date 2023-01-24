using Unity.Entities;

class PlantAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject plantPrefab;

    class PlantBaker : Baker<PlantAuthoring>
    {
        public override void Bake(PlantAuthoring authoring)
        {
            AddComponent(new Plant
            {
                plantPrefab = GetEntity(authoring.plantPrefab)
            });
        }
    }
}

struct Plant : IComponentData
{
    public Entity plantPrefab;
    float timePlanted; //to track the growth
    bool isReadyToPick; //true once fully grown
    bool pickedAndHeld; //true once picked by a farmer
}
