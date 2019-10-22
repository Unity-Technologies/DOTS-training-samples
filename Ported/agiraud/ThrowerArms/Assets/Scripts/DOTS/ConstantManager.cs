using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantManager : ScriptableObject
{
    public static Vector3 DestroyBoxMin = new Vector3(-100, -5, -100);
    public static Vector3 DestroyBoxMax = new Vector3(10, 50, 100);
}
