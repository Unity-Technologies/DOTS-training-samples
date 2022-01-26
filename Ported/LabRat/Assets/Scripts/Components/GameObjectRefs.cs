using Unity.Entities;

[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public UnityEngine.UI.Text[] ScoreDisplays;
    public UnityEngine.UI.Image[] Cursors;
}
