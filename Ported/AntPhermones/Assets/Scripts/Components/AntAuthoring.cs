using UnityEngine;
using Unity.Entities;
using Unity.Rendering;

public class AntAuthoring : MonoBehaviour
{
    public Ant ant;
    public Position position;
    public Speed speed;
    public Direction direction;
    class Baker : Baker<AntAuthoring>
    {
        public override void Bake(AntAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent<Ant>(entity, authoring.ant);
            AddComponent<Position>(entity, authoring.position);
            AddComponent<Speed>(entity, authoring.speed);
            AddComponent<Direction>(entity, authoring.direction);
            AddComponent(entity, new URPMaterialPropertyBaseColor());
        }
    }
}
