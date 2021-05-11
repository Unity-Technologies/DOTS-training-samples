using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

[GenerateAuthoringComponent]
public class GameObjectRefs : IComponentData
{
    public Text player0Score;
    public Text player1Score;
    public Text player2Score;
    public Text player3Score;

    public Text timerLabel;
    public GameObject gameOverOverlay;
    public Text introLabel;

}
