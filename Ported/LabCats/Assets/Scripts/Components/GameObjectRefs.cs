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
    public Camera Camera;
    public Text GameTime;
    public Text IntroText;
    public Text Player1ScoreText;
    public Text Player2ScoreText;
    public Text Player3ScoreText;
    public Text Player4ScoreText;
    public GameObject GameOverPanel; // James 
    public Text GameOverText; // James
    public GameObject Player1Cursor;
    public GameObject Player2Cursor;
    public GameObject Player3Cursor;
    public GameObject Player4Cursor;
    public GameObject BoardQuad;
    public GameObject PlayerArrowShadow;
}
