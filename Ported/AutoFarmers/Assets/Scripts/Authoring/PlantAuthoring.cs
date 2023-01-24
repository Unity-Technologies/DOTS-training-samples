using Unity.Entities;

class PlantAuthoring : UnityEngine.MonoBehaviour
{
    class PlantBaker : Baker<PlantAuthoring>
    {
        public override void Bake(PlantAuthoring authoring)
        {
            AddComponent(new Plant
            {
                timeToGrow = 5
            });
        }
    }
}

struct Plant : IComponentData
{
    public float timeToGrow;
    public float timePlanted; //to track the growth
    public bool isReadyToPick; //true once fully grown
    public bool pickedAndHeld; //true once picked by a farmer
}

