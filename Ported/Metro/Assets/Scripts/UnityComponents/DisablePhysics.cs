using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePhysics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
    }
}
