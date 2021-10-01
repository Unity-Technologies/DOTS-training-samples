using UnityEngine;
using System.Collections;

public class DisablePhysics : MonoBehaviour
{
    void Start()
    {
        Physics.autoSimulation = false;
    }
}
