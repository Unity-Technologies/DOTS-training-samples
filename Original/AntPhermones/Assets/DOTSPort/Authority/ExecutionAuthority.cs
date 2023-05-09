using UnityEngine;
using Unity.Entities;

public class ExecuteAuthoring : MonoBehaviour
{
    public bool SpawnBase;

    class Baker : Baker<ExecuteAuthoring>
    {
        public override void Bake(ExecuteAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            if (authoring.SpawnBase) AddComponent<AntSpawnerExecution>(entity);
        }
    }
}

public struct AntSpawnerExecution : IComponentData
{
}
