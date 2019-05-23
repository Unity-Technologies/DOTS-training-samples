using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "VehiclePart_X", menuName = "Factory/Vehicle Part")]
public class VehiclePart_Config : ScriptableObject
{   
    public GameObject prefab_part;
    public Vehicle_PartType partType;
    public int partVersion;
    [Range(1,10)]
    public int size;
}