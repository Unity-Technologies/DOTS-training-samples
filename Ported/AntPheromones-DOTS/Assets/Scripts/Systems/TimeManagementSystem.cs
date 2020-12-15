using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TimeManagementSystem : SystemBase
{

    protected override void OnUpdate()
    {
        var systemSpeed = GetSingletonEntity<TimeMultiplier>();
        var timeMultiplier = GetComponent<TimeMultiplier>(systemSpeed);

        if (Input.GetKeyDown(KeyCode.Alpha1)) timeMultiplier.SimulationSpeed = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) timeMultiplier.SimulationSpeed = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) timeMultiplier.SimulationSpeed = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4)) timeMultiplier.SimulationSpeed = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5)) timeMultiplier.SimulationSpeed = 5;
        if (Input.GetKeyDown(KeyCode.Alpha6)) timeMultiplier.SimulationSpeed = 6;
        if (Input.GetKeyDown(KeyCode.Alpha7)) timeMultiplier.SimulationSpeed = 7;
        if (Input.GetKeyDown(KeyCode.Alpha8)) timeMultiplier.SimulationSpeed = 8;
        if (Input.GetKeyDown(KeyCode.Alpha9)) timeMultiplier.SimulationSpeed = 9;

        SetComponent(systemSpeed, timeMultiplier);
    }
}
