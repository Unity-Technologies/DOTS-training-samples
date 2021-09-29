using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePhys : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
    }
}
