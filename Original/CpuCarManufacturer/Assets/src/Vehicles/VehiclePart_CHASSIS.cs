using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePart_CHASSIS : VehiclePart
{
    public VehicleDesign design;
    public Vehicle_ChassisType chassisType;
    public int totalpartsFitted, tempCriteriaMet;
    public List<VehiclePart_Assignment> partsNeeded;
    public Dictionary<VehiclePart_Config, int> partsFitted;
    public bool vehicleIsComplete;

    private void Start()
    {
        totalpartsFitted = 0;
        tempCriteriaMet = 0;
        partsFitted = new Dictionary<VehiclePart_Config, int>();
        partsNeeded = new List<VehiclePart_Assignment>();
        vehicleIsComplete = false;
        foreach (VehiclePart_Assignment _REQUIRED_PART in design.requiredParts)
        {
            if (_REQUIRED_PART.partConfig.partType != Vehicle_PartType.CHASSIS)
            {
                partsNeeded.Add(_REQUIRED_PART);
            }
        }
    }

    public bool AttachPart(VehiclePart_Config _part, GameObject _obj)
    {
        VehiclePart_Assignment _ASSIGNMENT = null;
        //Debug.Log("Trying to attach " + _part);
        foreach (VehiclePart_Assignment _PART_NEEDED in partsNeeded)
        {
            if (_PART_NEEDED.partConfig == _part)
            {
                _ASSIGNMENT = _PART_NEEDED;
                break;
            }
        }

        if (_ASSIGNMENT != null)
        {
            Transform _T = _obj.transform;
            _T.SetParent(transform);
            _T.localPosition = _ASSIGNMENT.position;
            _T.localRotation = _ASSIGNMENT.rotation;
            partsNeeded.Remove(_ASSIGNMENT);
            vehicleIsComplete = (partsNeeded.Count == 0);
            if (!partsFitted.ContainsKey(_ASSIGNMENT.partConfig))
            {
                partsFitted[_ASSIGNMENT.partConfig] = 1;
            }
            else
            {
                partsFitted[_ASSIGNMENT.partConfig]++;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}

public enum Vehicle_ChassisType
{
    CAR_2_DOOR,
    CAR_4_DOOR,
    BIKE,
    TRUCK,
    VAN
}