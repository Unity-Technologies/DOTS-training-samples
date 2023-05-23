using Unity.Entities;
using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour
{
    // TODO: enforce square, remove Columns and leave only Rows
    public int Rows = 100;
    public int Columns = 100;
    public int StartingFires = 4;

    class Baker : Baker<GameSettingsAuthoring>
    {
        public override void Bake(GameSettingsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameSettings
            {
                Rows = authoring.Rows,
                Columns = authoring.Columns,
                StartingFires = authoring.StartingFires
            });
        }
    }
}

public struct GameSettings : IComponentData
{
    public int Rows;
    public int Columns;
    public int StartingFires;

    public int Size => Rows * Columns;
}