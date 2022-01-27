using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public Text[] ScoreDisplays;
    public Graphic[] VictoryPanels;
    public Image[] Cursors;
    public Text TimerDisplay;
    public Text IntroDisplay;
    public Camera Camera;
    public GameObject PlayerTransparentArrow;
}
