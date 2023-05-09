using Unity.Entities;
using UnityEngine;

public class BotAuthoring : MonoBehaviour
{
    class Baker : Baker<BotAuthoring>
    {
        public override void Bake(BotAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Bot>(entity);
        }
    }
}

public struct Bot : IComponentData
{

}