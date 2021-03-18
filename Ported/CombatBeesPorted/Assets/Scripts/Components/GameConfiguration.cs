
using Unity.Entities;

public struct GameConfiguration: IComponentData
{
    public Entity BeeTeamAPrefab;
    public Entity BeeTeamBPrefab;
    public Entity FoodPrefab;
    public float HivePosition;
    public int BeeSpawnPerCollectedFood;
    public int BeeCount;
    public int FoodCount;
}
