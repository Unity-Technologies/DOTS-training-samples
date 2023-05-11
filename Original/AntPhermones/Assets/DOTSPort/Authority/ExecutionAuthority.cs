using UnityEngine;
using Unity.Entities;
using UnityEngine.Serialization;

public class ExecuteAuthoring : MonoBehaviour
{
    public bool SpawnBase;
    public bool SpawnFood;
    public bool ShowTimeScaleControls;
    public bool ObstacleSpawnerControls;
    public bool FoodMoveByMouse;

    class Baker : Baker<ExecuteAuthoring>
    {
        public override void Bake(ExecuteAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            if (authoring.SpawnBase) AddComponent<AntSpawnerExecution>(entity);
            if (authoring.SpawnFood) AddComponent<FoodSpawnerExecution>(entity);
            if (authoring.ShowTimeScaleControls) AddComponent<TimeScaleControlsExecution>(entity);
            if (authoring.ObstacleSpawnerControls) AddComponent<ObstacleSpawnerExecution>(entity);
            if (authoring.FoodMoveByMouse) AddComponent<FoodMoveByMouseExecution>(entity);
        }
    }
}

public struct AntSpawnerExecution : IComponentData
{
}

public struct FoodSpawnerExecution : IComponentData
{
}

public struct TimeScaleControlsExecution : IComponentData
{
}

public struct ObstacleSpawnerExecution : IComponentData
{
}

public struct FoodMoveByMouseExecution: IComponentData
{
}
