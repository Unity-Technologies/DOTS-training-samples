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
    float timePlanted; //to track the growth
    bool isReadyToPick; //true once fully grown
    bool pickedAndHeld; //true once picked by a farmer
}
