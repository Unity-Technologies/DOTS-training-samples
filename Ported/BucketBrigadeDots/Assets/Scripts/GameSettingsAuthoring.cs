using Unity.Entities;
using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour
{
    // Number of rows and columns: it's a square.
    public int GridWidth = 100;
    public int StartingFires = 4;

    class Baker : Baker<GameSettingsAuthoring>
    {
        public override void Bake(GameSettingsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameSettings
            {
                RowsAndColumns = authoring.GridWidth,
                StartingFires = authoring.StartingFires
            });
        }
    }
}

public struct GameSettings : IComponentData
{
    public int RowsAndColumns;
    public int StartingFires;

    public int Size => RowsAndColumns * RowsAndColumns;
}