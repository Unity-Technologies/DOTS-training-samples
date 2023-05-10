using UnityEngine;
using Unity.Entities;

public class ExecuteAuthoring : MonoBehaviour
{
    public bool SpawnBase;
    public bool SpawnFood;
    public bool ShowTimeScaleControls;

    class Baker : Baker<ExecuteAuthoring>
    {
        public override void Bake(ExecuteAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            if (authoring.SpawnBase) AddComponent<AntSpawnerExecution>(entity);
            if (authoring.SpawnFood) AddComponent<FoodSpawnerExecution>(entity);
            if (authoring.ShowTimeScaleControls) AddComponent<TimeScaleControls>(entity);
        }
    }
}

public struct AntSpawnerExecution : IComponentData
{
}

public struct FoodSpawnerExecution : IComponentData
{
}

public struct TimeScaleControls : IComponentData
{
}
