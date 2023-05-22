using Unity.Entities;
using UnityEngine;

public class GameSettingsAuthoring : MonoBehaviour
{
    public int Rows;
    public int Columns;

    class Baker : Baker<GameSettingsAuthoring>
    {
        public override void Bake(GameSettingsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameSettings
            {
                Rows = authoring.Rows,
                Columns = authoring.Columns
            });
        }
    }
    
}

public struct GameSettings : IComponentData
{
    public int Rows;
    public int Columns;
}