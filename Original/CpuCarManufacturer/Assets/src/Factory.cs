using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FactoryMode
{
    OOP,
    DOD
}

public class Factory : MonoBehaviour
{
    public static Factory INSTANCE;
    public FactoryMode factoryMode;
    public Storage PartsDeliveredTo;
    public GameObject PREFAB_StorageBot;
    private int tick = 0;
    [Range(0.00000001f, 1f)] public float tickRate = 0.1f;
    [Range(0.05f, 1f)] public float storageCellSize;
    private float t;

    [Header("Storage Times")]
    public int speed_REG = 1;
    public int speed_L1 = 5;
    public int speed_L2 = 15;
    public int speed_L3 = 35;
    public int speed_RAM = 105;
    public int speed_HD = 315;

    public Storage HD, RAM, L3;
    private Storage[] storage;
    [HideInInspector] public List<Workshop> workshops;
    private List<WorkshopTask> workshopTasks;

    private int storageCount;
    public static int SHARED_STORAGE_CAPACITY;
    public static int SHARED_STORAGE_CORE_SHARE;

    // ORDER
    // Quantity of each design to be built
    public VehicleDesign[] Order_designs;
    public int[] Order_quantities;
    public Dictionary<VehicleDesign, int> vehicleOrder;
    public Dictionary<VehiclePart_Config, int> attachedParts;
    public Dictionary<VehicleDesign, int> CompletedVehicles;
    private Dictionary<VehiclePart_Config, int> requiredParts;
    public bool orderComplete;

    private void Awake()
    {
        INSTANCE = this;
        t = tickRate;

        // setup storage
        storage = FindObjectsOfType<Storage>();
        foreach (Storage _STORAGE in storage)
        {
            _STORAGE.Init();
        }

        workshops = FindObjectsOfType<Workshop>().OrderBy(m => m.workshopIndex).ToList();
        storageCount = storage.Length;
        requiredParts = new Dictionary<VehiclePart_Config, int>();
        CompletedVehicles = new Dictionary<VehicleDesign, int>();
        attachedParts = new Dictionary<VehiclePart_Config, int>();
        orderComplete = false;
    }

    private void Start()
    {
        // set up storage times

        foreach (var _WORKSHOP in workshops)
        {
            _WORKSHOP.REG.taskDuration = speed_REG;
            _WORKSHOP.L1.taskDuration = speed_L2;
            _WORKSHOP.L2.taskDuration = speed_L2;
        }
        L3.taskDuration = speed_L3;
        RAM.taskDuration = speed_RAM;
        HD.taskDuration = speed_HD;

        SHARED_STORAGE_CAPACITY = L3.capacity;
        SHARED_STORAGE_CORE_SHARE = SHARED_STORAGE_CAPACITY / workshops.Count;
        Debug.Log("WORKSHOPS GET [" + SHARED_STORAGE_CORE_SHARE + "] of [" + SHARED_STORAGE_CAPACITY + "]");
        Get_RequiredParts();
        ScheduleTasks();
    }

    private void Get_RequiredParts()
    {
        // create "vehicle Order" dictionary
        vehicleOrder = new Dictionary<VehicleDesign, int>();
        for (int i = 0; i < Order_designs.Length; i++)
        {
            vehicleOrder.Add(Order_designs[i], Order_quantities[i]);
        }


        // get total number of parts required
        foreach (KeyValuePair<VehicleDesign, int> _DESIGN_PAIR in vehicleOrder)
        {
            VehicleDesign _DESIGN = _DESIGN_PAIR.Key;
            int _TOTAL = _DESIGN_PAIR.Value;

            // Set number of completed vehicles to zero
            CompletedVehicles[_DESIGN] = 0;

            Debug.Log(_DESIGN.designName + " -- " + _TOTAL);
            foreach (KeyValuePair<VehiclePart_Config, int> _PartCount in _DESIGN.quantities)
            {
                VehiclePart_Config _partType = _PartCount.Key;
                if (requiredParts.ContainsKey(_partType))
                {
                    Debug.Log("req: " + _partType + " x " + _TOTAL);
                    requiredParts[_partType] += _DESIGN.quantities[_partType] * _TOTAL;
                }
                else
                {
                    Debug.Log("req: " + _partType + " x " + _TOTAL);
                    requiredParts.Add(_partType, _DESIGN.quantities[_partType] * _TOTAL);
                    attachedParts.Add(_partType, 0);
                }
            }
        }

        // summarise order and sling it into RAM
        Debug.Log("Part totals...");
        foreach (KeyValuePair<VehiclePart_Config, int> _PAIR in requiredParts)
        {
            VehiclePart_Config _PART = _PAIR.Key;
            int _TOTAL = _PAIR.Value;
            GameObject _PART_PREFAB = _PART.prefab_part;
            Debug.Log(_PART.name + " x " + _TOTAL);
            List<VehiclePart_CHASSIS> _LIST_CHASSIS = new List<VehiclePart_CHASSIS>();
            List<VehiclePart> _LIST_PARTS = new List<VehiclePart>();
            for (int i = 0; i < _TOTAL; i++)
            {
                GameObject _part_OBJ =
                    (GameObject)Instantiate(_PART_PREFAB, Vector3.zero, Quaternion.identity);
                if (_PART.partType == Vehicle_PartType.CHASSIS)
                {
                    _LIST_CHASSIS.Add(_part_OBJ.GetComponent<VehiclePart_CHASSIS>());
                    var _TEMP_CHASSIS = _LIST_CHASSIS.Last();
                    _TEMP_CHASSIS.id = i;
                    _TEMP_CHASSIS.name = _TEMP_CHASSIS.partConfig.partType + "_" + _TEMP_CHASSIS.id;
                }
                else
                {
                    _LIST_PARTS.Add(_part_OBJ.GetComponent<VehiclePart>());
                    var _TEMP_PART = _LIST_PARTS.Last();
                    _TEMP_PART.id = i;
                    _TEMP_PART.name = _TEMP_PART.partConfig.partType + "_" + _TEMP_PART.id;
                }
            }

            // parts are instantiated - now lets force_quickSave them into "PartsDeliveredTo" (usually RAM)
            if (_LIST_CHASSIS.Count > 0)
            {
                PartsDeliveredTo.Force_data_into_storage(_LIST_CHASSIS.ToArray());
            }

            if (_LIST_PARTS.Count > 0)
            {
                PartsDeliveredTo.Force_data_into_storage(_LIST_PARTS.ToArray());
            }
        }
    }

    private void ScheduleTasks()
    {
        // STEP ONE - order all the required parts
        VehicleDesign _DESIGN = vehicleOrder.Keys.First();
        List<VehiclePart_Assignment> _PARTS = new List<VehiclePart_Assignment>();
        foreach (VehiclePart_Assignment _REQUIRED_PART in _DESIGN.requiredParts)
        {
            _PARTS.Add(_REQUIRED_PART);
        }

        workshopTasks = new List<WorkshopTask>();

        // STEP TWO - Depending on the approach (OOP / DOD), setup workshop tasks
        foreach (Workshop _WORKSHOP in workshops)
        {
            _WORKSHOP.Init(factoryMode);
        }

        switch (factoryMode)
        {
            case FactoryMode.OOP:
                // for each design - make a single workshop task to tackle it
                foreach (VehicleDesign _VEHICLE_DESIGN in vehicleOrder.Keys)
                {
                    workshopTasks.Add(new WorkshopTask(_VEHICLE_DESIGN, _VEHICLE_DESIGN.quantities));
                }

                foreach (Workshop _WORKSHOP in workshops)
                {
                    _WORKSHOP.Set_current_task(workshopTasks[0]);
                }

                break;
            case FactoryMode.DOD:
                // for each design, add tasks to a macro list
                List<VehiclePart_Config> _uniqueTasks = new List<VehiclePart_Config>();
                foreach (VehicleDesign _VEHICLE_DESIGN in vehicleOrder.Keys)
                {
                    foreach (var _DESIGN_PART in _VEHICLE_DESIGN.quantities.Keys)
                    {
                        if (_DESIGN_PART.partType != Vehicle_PartType.CHASSIS)
                        {
                            if (!_uniqueTasks.Contains(_DESIGN_PART))
                            {
                                _uniqueTasks.Add(_DESIGN_PART);
                                Dictionary<VehiclePart_Config, int> _targetPart = new Dictionary<VehiclePart_Config, int>();
                                _targetPart.Add(_DESIGN_PART, 1);
                                workshopTasks.Add(new WorkshopTask(_VEHICLE_DESIGN, _targetPart, requiredParts[_DESIGN_PART]));
                                Debug.Log("DOD: ADDED WORKSHOP TASK: " + _DESIGN_PART);
                            }
                        }
                    }
                }

                for (int _workshopIndex = 0; _workshopIndex < workshops.Count; _workshopIndex++)
                {
                    Workshop _W = workshops[_workshopIndex];
                    _W.Set_current_task(workshopTasks[_workshopIndex % workshopTasks.Count]);
                }

                break;
        }
    }

    private void Update()
    {
        if (Timer.TimerReachedZero(ref t))
        {
            Tick();
        }
    }

    private void Tick()
    {
        if (!orderComplete)
        {
            t = tickRate;
            tick++;

            for (int i = 0; i < storageCount; i++)
            {
                storage[i].Tick();
            }

            for (int i = 0; i < workshops.Count; i++)
            {
                workshops[i].Tick();
            }
        }
    }

    public void PartAttached(VehiclePart_Config _part, Workshop _workshop)
    {
        attachedParts[_part]++;
        //Debug.Log(_workshop.workshopIndex + " attached " + _part + "  " + attachedParts[_part] + " / " + requiredParts[_part]);
        if (attachedParts[_part] == requiredParts[_part])
        {
            if (factoryMode == FactoryMode.DOD)
            {
                workshopTasks.Remove(_workshop.currentTask);
                if (workshopTasks.Count > 0)
                {
                    WorkshopTask _NEXT_TASK = Get_inactive_workshop_task();
                    if (_NEXT_TASK != null)
                    {
                        _workshop.Set_current_task(_NEXT_TASK);
                    }
                    else
                    {
                        _workshop.gameObject.SetActive(false);
                    }
                    Debug.Log(_workshop.workshopIndex + " TASK COMPLETE: " + _part + ",   new task: " + _workshop.currentTask.requiredParts.First().Key);

                }
            }
        }
    }

    public void VehicleComplete(VehiclePart_CHASSIS _chassis)
    {
        vehicleOrder[_chassis.design]--;
        if (vehicleOrder[_chassis.design] == 0)
        {
            bool ordersStillPending = false;
            foreach (int _REMAINING in vehicleOrder.Values)
            {
                if (_REMAINING > 0)
                {
                    ordersStillPending = true;
                    break;
                }
            }

            if (!ordersStillPending)
            {
                orderComplete = true;
                Debug.Log("ORDER COMPLETE in " + tick + " ticks");
            }
            else
            {
                if (factoryMode == FactoryMode.OOP)
                {
                    workshopTasks.Remove(workshopTasks[0]);
                    Debug.Log("Next workshop task: " + workshopTasks[0].design.designName);
                    foreach (var _WORKSHOP in workshops)
                    {
                        _WORKSHOP.Set_current_task(workshopTasks[0]);
                        _WORKSHOP.purgingPartsToSharedStorage = false;
                    }
                }
            }
        }
    }

    public void ALERT_WorkshopPartUnavailable(VehiclePart_Config _part)
    {
        L3.Clear_all_requests();
        RAM.Clear_all_requests();
        HD.Clear_all_requests();

        L3.Change_state(StorageState.IDLE);
        RAM.Change_state(StorageState.IDLE);
        HD.Change_state(StorageState.IDLE);

        if (factoryMode == FactoryMode.OOP)
        {
            Workshop _purgeMe = null;
            int _leastUsedSpace = 999999;
            foreach (Workshop _WORKSHOP in workshops)
            {
                if (_WORKSHOP.usedSpace > 0 && _WORKSHOP.usedSpace < _leastUsedSpace)
                {
                    _purgeMe = _WORKSHOP;
                    _leastUsedSpace = _WORKSHOP.usedSpace;
                    _WORKSHOP.Clear_all_requests_then_idle();
                }
            }

            if (_purgeMe != null)
            {
                Debug.Log("PART SHORTAGE: " + _part);
                _purgeMe.Purge_parts_to_shared_storage();
            }
        }
    }
    WorkshopTask Get_inactive_workshop_task()
    {
        foreach (var _TASK in workshopTasks)
        {
            bool clashFound = false;
            foreach (var _WORKSHOP in workshops)
            {
                if (_WORKSHOP.currentTask == _TASK)
                {
                    clashFound = true;
                    break;
                }
            }
            if (!clashFound)
            {
                return _TASK;
            }
        }
        return null;
    }
}