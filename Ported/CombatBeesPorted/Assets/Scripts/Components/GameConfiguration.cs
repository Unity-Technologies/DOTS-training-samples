
using Unity.Entities;

[GenerateAuthoringComponent]
public struct GameConfiguration: IComponentData
{
    public Entity BeeTeamAPrefab;
    public Entity BeeTeamBPrefab;
}
