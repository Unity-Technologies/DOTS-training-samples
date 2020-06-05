using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStatusDisplayManager : MonoBehaviour
{
    static public CarStatusDisplayManager Instance;
    
    public Material BlockedStatusMaterial;
    public Material DefaultSpeedStatusMaterial;
    public Material AccelerationStatusMaterial;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
}
