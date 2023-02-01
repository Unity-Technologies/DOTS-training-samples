using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CarridgeInfo : MonoBehaviour
{
    public List<GameObject> leftDoors;
    public List<GameObject> rightDoors;
    public Vector3 closeLocalPos;
    public Vector3 openLocalPos;
}

