using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    public static Camera Instance;
    void Awake()
    {
        Instance = GetComponent<Camera>();
    }
}
