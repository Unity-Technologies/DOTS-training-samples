using Unity.Entities;
using UnityEngine.UI;
using UnityCamera = UnityEngine.Camera;

// Pay attention that this component is a class, in other words it's a managed component.
// Please check the package documentation to understand the implications of this.
[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public UnityCamera camera;
    public Text timerText;
    public Text player0Score;
    public Text player1Score;
    public Text player2Score;
    public Text player3Score;
}