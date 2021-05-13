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
}
