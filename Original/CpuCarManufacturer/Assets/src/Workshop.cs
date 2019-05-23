using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;

public class Workshop : MonoBehaviour
{
    [HideInInspector] public FactoryMode factoryMode;
    public int workshopIndex;
    public float width, height;
    public Storage L2, L1, REG;
    [HideInInspector] public int freeSpace, usedSpace, completedVehicles;

    [HideInInspector] public WorkshopTask currentTask = null;
    [HideInInspector] public VehiclePart_Config currentTaskPart;
    [HideInInspector] public List<VehiclePart_CHASSIS> REG_viableChassis;
    [HideInInspector] public List<VehiclePart_CHASSIS> workshop_viableChassis;
    private VehicleChassiRequest CurrentChassisRequest;
    [HideInInspector] public List<VehiclePart> REG_viableParts;
    public bool purgingPartsToSharedStorage = false;

    private void Awake()
    {
        completedVehicles = 0;
        workshop_viableChassis = new List<VehiclePart_CHASSIS>();
    }

    public void Init(FactoryMode _factoryMode)
    {
        factoryMode = _factoryMode;
        Debug.Log(workshopIndex + " MODE: " + _factoryMode);
    }



    public void Tick()
    {
        if (currentTask != null)
        {

            freeSpace = REG.freeSpace + L1.freeSpace + L2.freeSpace;
            usedSpace = REG.usedSpace + L1.usedSpace + L2.usedSpace;
            Perform_task();
        }
    }

    public void Set_current_task(WorkshopTask _task)
    {
        if (_task != null)
        {
            currentTask = _task;
            if (factoryMode == FactoryMode.DOD)
            {
                currentTaskPart = currentTask.requiredParts.First().Key;
            }
        }
    }

    // - - - - - - - - - - - - - - - - - PERFORM WORKSHOP TASK
    void Perform_task()
    {
        if (purgingPartsToSharedStorage)
        {
            Do_purge();
            return;
        }

        CurrentChassisRequest = new VehicleChassiRequest(
            currentTask.design.chassisType.partConfig,
            currentTask.design.chassisType.partConfig.partVersion,
            currentTask.requiredParts,
            REG,
            factoryMode);

        if (
            REG.currentState == StorageState.IDLE ||
            (REG.currentState == StorageState.WAITING && L1.currentState == StorageState.IDLE))
        {
            if (REG.currentState == StorageState.IDLE && L1.currentState == StorageState.WAITING)
            {
                L1.Clear_all_requests();
                L1.Change_state(StorageState.IDLE);
            }
            if (L2.currentState == StorageState.WAITING && Factory.INSTANCE.L3.currentState == StorageState.IDLE)
            {
                L2.Clear_all_requests();
                L2.Change_state(StorageState.IDLE);
            }
            // REG is able to request
            REG_viableChassis = Get_chassis(REG);

            if (REG_viableChassis.Count > 0)
            {
                // HAS CHASSIS
                Update_REG_ViableParts();

                if (REG_viableParts.Count > 0)
                {
                    // HAS PARTS
                    Attach_parts();
                    Remove_completed_chassis();
                }
                else
                {
                    // NO PARTS
                    bool REG_IS_FULL = REG.freeSpace == 0;
                    switch (factoryMode)
                    {

                        case FactoryMode.OOP:

                            if (REG_IS_FULL)
                            {
                                REG.Dump_from_line_exceptType(0, Vehicle_PartType.CHASSIS, 1);
                            }
                            else
                            {
                                Request_viable_parts(REG_viableChassis[0]);
                            }

                            break;

                        case FactoryMode.DOD:

                            if (REG_IS_FULL)
                            {
                                REG.Dump_nonViable_chassis(CurrentChassisRequest, 0);
                            }
                            else
                            {
                                Request_currentTask_part(REG);
                            }
                            break;
                    }
                }
            }
            else
            { // NO CHASSIS

                if (REG.freeSpace > 0)
                {
                    REG.waitingForPartType = CurrentChassisRequest.part;
                    REG.Change_state(StorageState.WAITING);
                    L1.Request_chassis(CurrentChassisRequest);
                }
                else
                { // NO ROOM, DUMP
                    REG.Dump_from_line_exceptType(0, Vehicle_PartType.CHASSIS, 1);
                }
            }
        }

        if (factoryMode == FactoryMode.DOD)
        {
            Push_nonViable_chassis_to_shared_storage();
            Push_nonViable_parts_to_shared_storage();

            // if we're waiting for occupied parts, try to purge nonViable parts
            if (REG.currentState == StorageState.WAITING &&
               L1.currentState == StorageState.WAITING &&
               L2.currentState == StorageState.WAITING
              )
            {
                REG.Change_state(StorageState.IDLE);
                L1.Change_state(StorageState.IDLE);
                L2.Change_state(StorageState.IDLE);
            }

            if (L2.currentState == StorageState.IDLE)
            {
                Request_currentTask_part(L2);
            }
        }
    }

    private void Push_nonViable_chassis_to_shared_storage()
    {
        if (REG.Has_nonViable_chassis(CurrentChassisRequest))
        {
            REG.Dump_first_nonViable_chassis(CurrentChassisRequest);
        }

        else if (L1.Has_nonViable_chassis(CurrentChassisRequest))
        {
            L1.Dump_first_nonViable_chassis(CurrentChassisRequest);
        }

        else if (L2.Has_nonViable_chassis(CurrentChassisRequest))
        {
            L2.Dump_first_nonViable_chassis(CurrentChassisRequest);
        }
    }
    private void Push_nonViable_parts_to_shared_storage()
    {
        if (REG.Contains_nonViablePart(currentTaskPart))
        {
            REG.Dump_first_nonViable_part(currentTaskPart);
        }

        else if (L1.Contains_nonViablePart(currentTaskPart))
        {
            L1.Dump_first_nonViable_part(currentTaskPart);
        }

        else if (L2.Contains_nonViablePart(currentTaskPart))
        {
            L2.Dump_first_nonViable_part(currentTaskPart);
        }
    }

    #region PARTS

    private void Update_REG_ViableParts()
    {

        REG_viableParts = new List<VehiclePart>();
        List<VehiclePart_Config> _TASK_PARTS = currentTask.requiredParts.Keys.ToList();

        // If part is NOT a chassis and is used in the current TASK - add it to VIABLE_PARTS
        for (int _slotIndex = 0; _slotIndex < REG.lineLength; _slotIndex++)
        {
            var _PART = REG.Get_data_slot(0, _slotIndex);
            if (_PART != null)
            {
                if (_PART.partConfig.partType != Vehicle_PartType.CHASSIS)
                {
                    if (_TASK_PARTS.Contains(_PART.partConfig))
                    {
                        foreach (VehiclePart_CHASSIS _CHASSIS in REG_viableChassis)
                        {
                            if (!_CHASSIS.partsFitted.ContainsKey(_PART.partConfig))
                            {
                                REG_viableParts.Add(_PART);
                            }
                            else if (_CHASSIS.partsFitted[_PART.partConfig] < _CHASSIS.design.quantities[_PART.partConfig])
                            {
                                REG_viableParts.Add(_PART);
                            }
                        }
                    }
                }
            }
        }
    }



    public void Request_viable_parts(VehiclePart_CHASSIS _chassis)
    {
        foreach (KeyValuePair<VehiclePart_Config, int> _PAIR in currentTask.requiredParts)
        {
            VehiclePart_Config _PART = _PAIR.Key;
            int _QUANTITY = _PAIR.Value;
            if (_PART.partType != Vehicle_PartType.CHASSIS)
            {
                if (currentTask.requiredParts.ContainsKey(_PAIR.Key))
                {
                    if (factoryMode == FactoryMode.OOP)
                    {
                        if (_chassis.partsFitted.ContainsKey(_PART))
                        {
                            if (_chassis.partsFitted[_PART] >= _QUANTITY)
                            {
                                continue;
                            }
                        }
                    }
                    L1.Request_part(new VehiclePartRequest(_PAIR.Key, REG));
                    REG.waitingForPartType = _chassis.partsNeeded[0].partConfig;
                    if (!purgingPartsToSharedStorage)
                    {
                        REG.Change_state(StorageState.WAITING);
                    }
                    return;
                }
            }
        }
    }
    public void Request_currentTask_part(Storage _storage)
    {
        _storage.Change_state(StorageState.WAITING);
        _storage.getsPartsFrom.Request_part(new VehiclePartRequest(currentTaskPart, _storage));
    }



    void Attach_parts()
    {
        for (int _slotIndex = 0; _slotIndex < REG.lineLength; _slotIndex++)
        {
            var _PART = REG.storageLines[0].slots[_slotIndex];
            if (REG_viableParts.Contains(_PART))
            {
                for (int _chassisIndex = 0; _chassisIndex < REG_viableChassis.Count; _chassisIndex++)
                {
                    if (REG_viableChassis[_chassisIndex].AttachPart(_PART.partConfig, _PART.gameObject))
                    {
                        _PART.ClearDestination();
                        REG_viableParts.Remove(_PART);
                        Factory.INSTANCE.PartAttached(_PART.partConfig, this);
                        REG.Clear_slot(0, _slotIndex);
                        break;
                    }
                }
            }
        }
    }

    #endregion
    #region CHASSIS

    public List<VehiclePart_CHASSIS> Get_chassis(Storage _storage)
    {
        List<VehiclePart_CHASSIS> _result = new List<VehiclePart_CHASSIS>();
        // Iterate through LINES

        for (int _slotIndex = 0; _slotIndex < _storage.lineLength; _slotIndex++)
        {
            if (_storage.Is_chassis_viable(0, _slotIndex, CurrentChassisRequest))
            {
                _result.Add(_storage.storageLines[0].slots[_slotIndex] as VehiclePart_CHASSIS);
            }
        }
        return _result;
    }

    void Remove_completed_chassis()
    {
        foreach (VehiclePart_CHASSIS _CHASSIS in REG_viableChassis)
        {
            if (_CHASSIS.vehicleIsComplete)
            {
                int indexOfCompletedChassis = REG.storageLines[0].slots.IndexOf(_CHASSIS);
                REG.Clear_slot(0, indexOfCompletedChassis);
                Factory.INSTANCE.VehicleComplete(_CHASSIS);
                completedVehicles++;

                _CHASSIS.transform.position = transform.position + new Vector3(completedVehicles % 10, 0f, -1 - Mathf.FloorToInt(completedVehicles / 10));
            }
        }
    }
    #endregion
    #region PURGE
    public void Purge_parts_to_shared_storage()
    {
        purgingPartsToSharedStorage = true;
        Debug.Log("PURGE WORKSHOP: " + workshopIndex);
        Factory.INSTANCE.L3.Await_purged_data();
        L2.Await_purged_data();
        L1.Await_purged_data();
        REG.Await_purged_data();

    }
    public void Cancel_purge()
    {
        purgingPartsToSharedStorage = false;
        Debug.Log("CANCEL PURGE: " + workshopIndex);
        Factory.INSTANCE.L3.Change_state(StorageState.IDLE);
        Clear_all_requests_then_idle();
    }
    public void Clear_all_requests_then_idle()
    {
        L2.Clear_all_requests();
        L1.Clear_all_requests();
        REG.Clear_all_requests();

        REG.Change_state(StorageState.IDLE);
        L1.Change_state(StorageState.IDLE);
        L2.Change_state(StorageState.IDLE);
    }

    public void Do_purge()
    {
        if (REG.usedSpace > 0)
        {
            REG.Dump_line();
        }
        else if (L1.usedSpace > 0)
        {
            Factory.INSTANCE.L3.Await_purged_data();
            L1.Dump_first_line_with_data();

        }
        else if (L2.usedSpace > 0)
        {
            Factory.INSTANCE.L3.Await_purged_data();
            L2.Dump_first_line_with_data();
            if (L2.usedSpace == 0)
            {
                // purge complete
                // reset all workshops - start searching fresh for parts
                foreach (var _WORKSHOP in Factory.INSTANCE.workshops)
                {
                    _WORKSHOP.Clear_all_requests_then_idle();
                }
            }
        }
        if (REG.currentState == StorageState.IDLE &&
                   L1.currentState == StorageState.IDLE &&
                   L2.currentState == StorageState.IDLE
                 )
        {
            Cancel_purge();
        }
    }
    #endregion
    #region GIZMOS
    private void OnDrawGizmos()
    {
        //if (L2 && L1 && REG)
        //{
        //    string myName = "CORE [" + workshopIndex + "]";
        //    if (factoryMode == FactoryMode.DOD)
        //    {
        //        if (currentTask != null)
        //        {
        //            myName += " >> " + currentTask.requiredParts.First().Key.partType;
        //        }
        //    }
        //    if(purgingPartsToSharedStorage){
        //        myName += "\n PURGING";
        //    }
        //    GizmoHelpers.DrawRect(Color.cyan, transform.position, width, height, myName);
        //}
    }
    #endregion
}

public class WorkshopTask
{
    public VehicleDesign design;
    public Dictionary<VehiclePart_Config, int> requiredParts;
    public float ratio_chassis_to_parts;
    public int totalRequired;
    public int tasksCompleted;

    public WorkshopTask(VehicleDesign _design, Dictionary<VehiclePart_Config, int> _requiredParts, int _totalRequired = 0)
    {
        design = _design;
        requiredParts = _requiredParts;
        totalRequired = _totalRequired;
        tasksCompleted = 0;
        int partCount = 0;
        foreach (KeyValuePair<VehiclePart_Config, int> _PAIR in requiredParts)
        {
            if (_PAIR.Key.partType != Vehicle_PartType.CHASSIS)
            {
                partCount++;
            }
        }

        ratio_chassis_to_parts = 1 / (float)partCount;
    }
}