using Unity.Entities;

public class PlantAuthoring : UnityEngine.MonoBehaviour
{
}

class PlantBaker : Baker<PlantAuthoring>
{
    public override void Bake(PlantAuthoring authoring)
    {
        AddComponent(new Plant
        {
        });
        AddComponent(new PlantHealth
        {
            Health = 0
        });

        AddComponent(new PlantGrowing
        {

        });
    }
}