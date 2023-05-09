using UnityEngine;
using Unity.Entities;

public class HomeAuthoring : MonoBehaviour
{
    public Home home;
    public Position position;

    class Baker : Baker<HomeAuthoring>
    {
        public override void Bake(HomeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent<Home>(entity, authoring.home);
            AddComponent<Position>(entity, authoring.position);
        }
    }
}
