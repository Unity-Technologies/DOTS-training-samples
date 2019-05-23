using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEditor;
using UnityEngine;

public class Storage : MonoBehaviour
{

    #region < FIELDS
    public static bool GIZMOS_DRAW_CELLS = false;
    public static Vector3 PART_ARRIVAL_OFFSET = new Vector3(-2, 0, 0);
    public static float PART_TRANSIT_SPACING = 0.02f;
    // animation settings
    public static float ANIM_RANGE_FETCH = 0.35f;
    public static float ANIM_RANGE_LOAD = 0.45f;
    public static float ANIM_RANGE_DELIVER = 0.8f;

    public string storageName;
    public Transform nav_parts_IN, nav_parts_OUT;
    public Storage getsPartsFrom;
    public Storage sendingLineTo;
    public Color colour;
    public bool isLastInChain = false;
    public bool isRegister = false;
    public StorageBot storageBot;

    [Range(1, 1000)] public int taskDuration;

    // Total number of clusters along XYZ
    [Range(1, 100)]
    public int linesX, linesY, linesZ;

    // How many cells make up a cluster?  XYZ
    [Range(1, 100)]
    public int lineLength, lines_groupBy_Y, lines_groupBy_Z;

    // How much space goes between clusters? XYZ

    [Range(0, 10)]
    public int gutterX, gutterY, gutterZ;

    private float width, depth;
    [HideInInspector] public int usedSpace, freeSpace, capacity, clusterCapacity, taskStep;
    [HideInInspector] public StorageState currentState;

    [HideInInspector] public List<Vector3> aisleSpaces;
    [HideInInspector] public float factor;
    [HideInInspector] public List<StorageLine> storageLines;
    [HideInInspector] public VehiclePart[] parts_IN, parts_OUT;
    [HideInInspector] public VehiclePartRequest current_PART_request, next_PART_request;
    [HideInInspector] public VehicleChassiRequest current_CHASSIS_request, next_CHASSIS_request;
    [HideInInspector] public VehiclePart_Config waitingForPartType;

    // Storage GRID layout
    private int cellsX, cellsY, cellsZ;

    // Movement of parts
    private int targetStorageLine;
    private Vector3 fetchLine_pos_START;

    #endregion FIELDS >
    #region < INIT
    public void Init()
    {
        DefineStorageLayout();
        if (!isRegister)
        {
            CreateStorageBot();
        }
    }

    private string Log()
    {
        return storageName + " \n(" + usedSpace + "/" + capacity + ")\n" + currentState;
    }

    private void DefineStorageLayout()
    {
        storageLines = new List<StorageLine>();
        aisleSpaces = new List<Vector3>();
        Vector3 _POS = transform.position;

        clusterCapacity = lineLength * lines_groupBy_Y * lines_groupBy_Z;
        capacity = clusterCapacity * (linesX * linesY * linesZ);

        freeSpace = capacity;
        usedSpace = 0;


        int lineIndex = 0;
        for (int stackY = 0; stackY < linesY; stackY++)
        {
            for (int stackZ = 0; stackZ < linesZ; stackZ++)
            {
                for (int lineX = 0; lineX < linesX; lineX++)
                {
                    AddStorageLine(lineIndex, _POS + new Vector3((lineX * lineLength) + (gutterX * lineX),
                                                  stackY + (gutterY * stackY),
                                                  stackZ + (gutterZ * stackZ)) * Factory.INSTANCE.storageCellSize);
                    lineIndex++;
                }
            }
        }
    }

    private void CreateStorageBot()
    {
        GameObject bot_OBJ = (GameObject)Instantiate(Factory.INSTANCE.PREFAB_StorageBot, nav_parts_OUT.position, transform.rotation);
        storageBot = bot_OBJ.GetComponent<StorageBot>();
        storageBot.Init();
    }

    void AddStorageLine(int _index, Vector3 _pos)
    {
        storageLines.Add(new StorageLine(_index, lineLength, _pos));
    }
    #endregion INIT >
    #region < State / Update
    public void Change_state(StorageState _newState)
    {
        if (currentState != _newState)
        {
            taskStep = 0;
            currentState = _newState;
            //Debug.Log(storageName + ": " + currentState);
            switch (currentState)
            {
                case StorageState.IDLE:
                    // grab chassis / part if a request is pending
                    if (current_CHASSIS_request != null)
                    {

                        Request_chassis(current_CHASSIS_request);
                    }
                    else if (current_PART_request != null)
                    {
                        Request_part(current_PART_request);
                    }
                    else if (next_CHASSIS_request != null)
                    {
                        Debug.Log(storageName + " found NEXT CHASSIS req");
                        current_CHASSIS_request = next_CHASSIS_request;
                        next_CHASSIS_request = null;
                        Request_chassis(current_CHASSIS_request);
                    }
                    else if (next_PART_request != null)
                    {
                        current_PART_request = next_PART_request;
                        next_PART_request = null;
                        Request_part(current_PART_request);


                    }

                    break;
                case StorageState.WAITING:
                    break;
                case StorageState.FETCHING:
                    break;
            }
        }
    }
    public void Tick()
    {
        if (!isLastInChain)
        {
            Redo_request_if_required();
        }
        if(!isRegister){
            storageBot.Destination = nav_parts_IN.position;
        }
        switch (currentState)
        {
            case StorageState.IDLE:
                break;
            case StorageState.WAITING:
                break;
            case StorageState.WAIT_FOR_PURGED_DATA:
                break;
            default:
                if (taskStep == taskDuration)
                {
                    Send_parts(currentState == StorageState.DUMP);
                }
                else
                {
                    if (!isRegister)
                    {
                        UpdateBot();
                    }
                    taskStep++;
                }
                break;
        }

        factor = (float)taskStep / (float)taskDuration;
    }
    public bool Has_viable_chassis(VehicleChassiRequest _request)
    {
        for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
        {
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                if (Is_chassis_viable(_lineIndex, _slotIndex, _request))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool Has_nonViable_chassis(VehicleChassiRequest _request)
    {
        for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
        {
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                VehiclePart _PART = Get_data_slot(_lineIndex, _slotIndex);
                if (_PART != null)
                {
                    if (_PART.partConfig.partType == Vehicle_PartType.CHASSIS)
                    {
                        if (!Is_chassis_viable(_lineIndex, _slotIndex, _request))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    public bool Contains_nonViablePart(VehiclePart_Config _targetPart)
    {
        for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
        {
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                VehiclePart _PART = Get_data_slot(_lineIndex, _slotIndex);
                if (_PART != null)
                {
                    if (_PART.partConfig.partType != Vehicle_PartType.CHASSIS && _PART.partConfig != _targetPart)
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }



    #endregion State / Update >
    #region < Send / Recieve

    // REQUESTS

    public void Request_part(VehiclePartRequest _request)
    {
        if (currentState == StorageState.IDLE)
        { // I am free to take orders

            sendingLineTo = _request.deliverTo;
            sendingLineTo.waitingForPartType = _request.part;
            current_PART_request = null;
            if (freeSpace > 0)
            { // do I have space for a new line?
                Do_part_request(_request);
            }
            else
            { // no room, DUMP a line
                current_PART_request = _request;
                Dump_line();
            }
        }
        else
        {
            next_PART_request = _request;
        }
    }
    public void Request_chassis(VehicleChassiRequest _request)
    {
        if (currentState == StorageState.IDLE)
        {
            sendingLineTo = _request.deliverTo;
            sendingLineTo.waitingForPartType = _request.part;

            current_CHASSIS_request = null;

            if (freeSpace > 0)
            {
                // HAS SPACE
                Do_chassis_request(_request);
            }
            else
            {
                // NO SPACE
                current_CHASSIS_request = _request;
                Dump_line();
            }
        }
    }

    // ATTEMPT TO EXECUTE REQUESTS
    private void Do_part_request(VehiclePartRequest _request)
    {
        sendingLineTo = _request.deliverTo;
        current_PART_request = _request;
        for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
        {
            StorageLine _LINE = storageLines[_lineIndex];
            var _SLOTS = _LINE.slots;
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                if (_SLOTS[_slotIndex] != null)
                {
                    if (_SLOTS[_slotIndex].partConfig == _request.part)
                    {
                        Set_outgoing_parts(_lineIndex, sendingLineTo);
                        Change_state(StorageState.FETCHING);
                        return;
                    }
                }
            }
        }

        if (!isLastInChain)
        {
            // IF YOU REACH THIS POINT - YOU DONT HAVE THE PARTS, request from the next storage in chain :)
            Change_state(StorageState.WAITING);
            getsPartsFrom.Request_part(new VehiclePartRequest(_request.part, this));
        }
        else
        {
            Factory.INSTANCE.ALERT_WorkshopPartUnavailable(_request.part);
        }
    }

    private void Do_chassis_request(VehicleChassiRequest _request)
    {
        sendingLineTo = _request.deliverTo;
        current_CHASSIS_request = _request;
        // for StorageLines
        for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
        {
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                if (Is_chassis_viable(_lineIndex, _slotIndex, _request))
                {
                    Set_outgoing_parts(_lineIndex, sendingLineTo);
                    Change_state(StorageState.FETCHING);
                    return;
                }
            }
        }

        if (!isLastInChain)
        {
            // IF YOU REACH THIS POINT - YOU DONT HAVE THE PARTS, request from the next storage in chain :)
            Change_state(StorageState.WAITING);
            getsPartsFrom.Request_chassis(new VehicleChassiRequest(_request.part, _request.chassisVersion, _request.requiredParts, this, _request.factoryMode));
        }
        else
        {
            Factory.INSTANCE.ALERT_WorkshopPartUnavailable(_request.part);
        }
    }

    public void Redo_request_if_required()
    {
        if (currentState == StorageState.IDLE ||
            (currentState == StorageState.WAITING && getsPartsFrom.currentState == StorageState.IDLE))
        {
            if (getsPartsFrom.currentState == StorageState.WAITING)
            {
                getsPartsFrom.Change_state(StorageState.IDLE);
            }

            if (current_PART_request != null)
            {
                //Debug.Log(workshopIndex + "_L2 was waiting for L3 - new req sent");
                getsPartsFrom.Clear_all_requests();
                getsPartsFrom.Request_part(new VehiclePartRequest(current_PART_request.part, this));
            }
            else if (current_CHASSIS_request != null)
            {
                //Debug.Log(workshopIndex + "_L2 was waiting for L3 - new req sent");
                getsPartsFrom.Clear_all_requests();
                getsPartsFrom.Request_chassis(new VehicleChassiRequest(
                    current_CHASSIS_request.part,
                    current_CHASSIS_request.chassisVersion,
                    current_CHASSIS_request.requiredParts,
                    this,
                    current_CHASSIS_request.factoryMode));
            }
        }

    }

    private void Set_outgoing_parts(int _lineIndex, Storage _sendTo)
    {
        List<VehiclePart> _partsToSend = new List<VehiclePart>();
        var _LINE = storageLines[_lineIndex];
        for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
        {
            if (_LINE.slots[_slotIndex] != null)
            {
                _partsToSend.Add(_LINE.slots[_slotIndex]);
            }
        }

        Set_outgoing_parts(_lineIndex, _partsToSend.ToArray(), _sendTo);
    }
    private void Set_outgoing_parts(int _lineIndex, VehiclePart[] _parts, Storage _sendTo)
    {
        targetStorageLine = _lineIndex;
        fetchLine_pos_START = storageLines[targetStorageLine].slotPositions[0];
        parts_OUT = _parts;
    }

    public bool Is_chassis_viable(int _lineIndex, int _slotIndex, VehicleChassiRequest _request)
    {
        VehiclePart _SLOT = Get_data_slot(_lineIndex, _slotIndex);
        VehiclePart_CHASSIS _CHASSIS = null;
        if (_SLOT != null)
        {
            if (_SLOT.partConfig.partType == Vehicle_PartType.CHASSIS)
            { // part IS a chassis
                if (_SLOT.partConfig.partVersion == _request.chassisVersion || _request.factoryMode == FactoryMode.DOD)
                { // is Correct chassis type
                    _CHASSIS = _SLOT as VehiclePart_CHASSIS;
                }
            }
        }

        if (_CHASSIS != null)
        {
            if (_CHASSIS.partsNeeded.Count > 0)
            {

                // If chassis has less a defecit of our required parts, grab it
                foreach (KeyValuePair<VehiclePart_Config, int> _PAIR in _request.requiredParts)
                {
                    VehiclePart_Config _REQ_PART = _PAIR.Key;
                    if (_CHASSIS.design.quantities.ContainsKey(_REQ_PART))
                    {
                        int _QUANTITY = _CHASSIS.design.quantities[_REQ_PART];
                        if (_REQ_PART.partType != Vehicle_PartType.CHASSIS)
                        {
                            if (_CHASSIS.partsFitted.ContainsKey(_REQ_PART))
                            {
                                if (_CHASSIS.partsFitted[_REQ_PART] < _QUANTITY)
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }

        }
        return false;
    }

    public List<VehiclePart> Recieve_parts(VehiclePart[] _parts)
    {
        if (_parts.Length > 0)
        {
            List<VehiclePart> stored = _parts.ToList();
            if (currentState == StorageState.IDLE || currentState == StorageState.WAITING || currentState == StorageState.WAIT_FOR_PURGED_DATA)
            {
                if (freeSpace > 0)
                {
                    if (freeSpace < lineLength)
                    {
                        // not enough room for everyone - check part viability
                        int targetPartsSaved = 0;
                        for (int _partIndex = 0; _partIndex < _parts.Length; _partIndex++)
                        {
                            if (_parts[_partIndex].partConfig.partType != waitingForPartType.partType)
                            {
                                stored.Remove(_parts[_partIndex]);
                            }
                            else
                            {
                                if (targetPartsSaved > freeSpace)
                                {
                                    stored.Remove(_parts[_partIndex]);
                                }
                                targetPartsSaved++;
                            }
                        }
                    }
                    if (stored.Count > 0)
                    {
                        stored = Attempt_store_new_data(stored.ToArray()).ToList();
                        Change_state(StorageState.IDLE);
                    }
                }
                else
                {
                    // no room available - ditch line zero
                    sendingLineTo = getsPartsFrom;
                    Set_outgoing_parts(0, sendingLineTo);
                    Change_state(StorageState.FETCHING);
                }
                return stored;
            }
            else
            {
                parts_IN = _parts;
                return null;
            }

        }
        return null;
    }

    private void Send_parts(bool _dumpingParts)
    {
        if (parts_OUT.Length > 0)
        {
            List<VehiclePart> _SENT_PARTS = sendingLineTo.Recieve_parts(parts_OUT);
            if (_SENT_PARTS != null)
            {
                for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
                {
                    for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
                    {
                        VehiclePart _SLOT = Get_data_slot(_lineIndex, _slotIndex);
                        if (_SLOT != null)
                        {
                            if (_SENT_PARTS.Contains(storageLines[_lineIndex].slots[_slotIndex]))
                            {
                                _SENT_PARTS.Remove(storageLines[_lineIndex].slots[_slotIndex]);
                                Clear_slot(_lineIndex, _slotIndex);
                            }
                            else
                            {
                                Position_part_in_storage(_SLOT, _lineIndex, _slotIndex);
                            }
                        }
                    }
                }
            }
        }
        if (!_dumpingParts)
        {
            current_PART_request = null;
            current_CHASSIS_request = null;
        }
        Change_state(StorageState.IDLE);
    }


    public void Await_purged_data()
    {
        Clear_all_requests();
        Change_state(StorageState.WAIT_FOR_PURGED_DATA);
    }
    public void Clear_all_requests()
    {
        current_PART_request = null;
        current_CHASSIS_request = null;
        next_PART_request = null;
        next_CHASSIS_request = null;
    }
    #endregion Send / Recieve >
    #region < Slot Management

    public VehiclePart Get_data_slot(int _lineIndex, int _slotIndex)
    {
        return storageLines[_lineIndex].slots[_slotIndex];
    }
    public bool Slot_contains_part_type(int _lineIndex, int _slotIndex, Vehicle_PartType _type)
    {
        VehiclePart _SLOT = Get_data_slot(_lineIndex, _slotIndex);
        if (_SLOT != null)
        {
            if (_SLOT.partConfig.partType == _type)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private VehiclePart[] Attempt_store_new_data(VehiclePart[] _parts)
    {
        if (_parts.Length > 0)
        {
            List<VehiclePart> _STORED_PARTS = new List<VehiclePart>();
            int _partIndex = 0;
            for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
            {
                StorageLine _LINE = storageLines[_lineIndex];

                for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
                {
                    VehiclePart _PART = _parts[_partIndex];
                    if (_LINE.slots[_slotIndex] == null && _PART != null)
                    {
                        Assign_slot(_lineIndex, _slotIndex, _PART);
                        _STORED_PARTS.Add(_parts[_partIndex]);
                        _partIndex++;
                    }
                    if (_partIndex >= _parts.Length)
                    {
                        break;
                    }
                }
                if (_partIndex >= _parts.Length)
                {
                    break;
                }
            }
            return _STORED_PARTS.ToArray();
        }
        else
        {
            return null;
        }
    }
    public void Force_data_into_storage(VehiclePart[] _parts)
    {
        Attempt_store_new_data(_parts);
        for (int _partIndex = 0; _partIndex < _parts.Length; _partIndex++)
        {
            VehiclePart _PART = _parts[_partIndex];
            _PART.transform.position = _PART.destination;
            _PART.ClearDestination();
        }
    }

    public void Clear_slot(int _lineIndex, int _slotIndex)
    {
        if (storageLines[_lineIndex].slots[_slotIndex] != null)
        {
            freeSpace++;
            usedSpace--;
        }
        storageLines[_lineIndex].slots[_slotIndex] = null;
    }

    private void Assign_slot(int _lineIndex, int _slotIndex, VehiclePart _part)
    {
        if (_part != null)
        {
            if (storageLines[_lineIndex].slots[_slotIndex] == null)
            {
                storageLines[_lineIndex].slots[_slotIndex] = _part;
                Position_part_in_storage(_part, _lineIndex, _slotIndex);
                freeSpace--;
                usedSpace++;
            }
        }
    }
    #endregion Slot Management >

    private void Position_part_in_storage(VehiclePart _part, int _lineIndex, int _slotIndex)
    {
        _part.SetDestination(storageLines[_lineIndex].slotPositions[_slotIndex]);
    }

    public void Dump_line(int _lineIndex = 0)
    {
        Dump_slots(lineLength, _lineIndex);
    }
    public void Dump_slots(int _count, int _lineIndex = 0)
    {
        if (currentState == StorageState.IDLE || currentState == StorageState.WAIT_FOR_PURGED_DATA)
        {
            Change_state(StorageState.DUMP);
            List<VehiclePart> dumpList = new List<VehiclePart>();
            StorageLine _LINE = storageLines[_lineIndex];
            for (int _slotIndex = 0; _slotIndex < _count; _slotIndex++)
            {
                if (_LINE.slots[_slotIndex] != null)
                {
                    dumpList.Add(_LINE.slots[_slotIndex]);
                }
            }
            sendingLineTo = getsPartsFrom;
            Set_outgoing_parts(_lineIndex, dumpList.ToArray(), sendingLineTo);
        }
    }
    public void Dump_first_line_with_data()
    {
        for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
        {
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                if (storageLines[_lineIndex].slots[_slotIndex] != null)
                {
                    Dump_line(_lineIndex);
                    return;
                }
            }
        }
    }

    public void Dump_from_line_exceptType(int _lineIndex, Vehicle_PartType _keepThisPart, int _maxKept)
    {
        if (currentState == StorageState.IDLE)
        {
            int partsKept = 0;
            Change_state(StorageState.DUMP);
            List<VehiclePart> dumpList = new List<VehiclePart>();
            StorageLine _LINE = storageLines[_lineIndex];
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                VehiclePart _PART = _LINE.slots[_slotIndex];

                if (_PART != null)
                {
                    if (_PART.partConfig.partType == _keepThisPart)
                    {
                        if (partsKept < _maxKept)
                        {
                            partsKept++;
                        }
                        else
                        {
                            dumpList.Add(_PART);
                        }
                    }
                    else
                    {
                        dumpList.Add(_PART);
                    }
                }
            }
            sendingLineTo = getsPartsFrom;
            Set_outgoing_parts(_lineIndex, dumpList.ToArray(), sendingLineTo);
        }
    }

    public void Dump_nonViable_chassis(VehicleChassiRequest _request, int _lineIndex)
    {
        if (currentState == StorageState.IDLE)
        {
            List<VehiclePart> dumpList = new List<VehiclePart>();
            StorageLine _LINE = storageLines[_lineIndex];
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                if (!Is_chassis_viable(_lineIndex, _slotIndex, _request))
                {
                    dumpList.Add(storageLines[_lineIndex].slots[_slotIndex]);
                }
            }
            sendingLineTo = getsPartsFrom;
            if (dumpList.Count > 0)
            {
                Change_state(StorageState.DUMP);
                Set_outgoing_parts(_lineIndex, dumpList.ToArray(), sendingLineTo);
            }
            else
            {
                Dump_from_line_exceptType(0, Vehicle_PartType.CHASSIS, 1);
            }
        }
    }
    public void Dump_first_nonViable_chassis(VehicleChassiRequest _request)
    {
        if (getsPartsFrom.currentState != StorageState.FETCHING)
        {
            bool foundOnLine = false;
            int targetLine = -1;
            List<VehiclePart> dumpList = new List<VehiclePart>();
            for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
            {
                for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
                {
                    VehiclePart _SLOT = Get_data_slot(_lineIndex, _slotIndex);
                    if (Slot_contains_part_type(_lineIndex, _slotIndex, Vehicle_PartType.CHASSIS))
                    {
                        if (!Is_chassis_viable(_lineIndex, _slotIndex, _request))
                        {
                            dumpList.Add(_SLOT);
                            targetLine = _lineIndex;
                            foundOnLine = true;
                        }
                    }
                }
                if (foundOnLine) break;
            }
            sendingLineTo = getsPartsFrom;
            if (dumpList.Count > 0)
            {
                Change_state(StorageState.DUMP);
                Set_outgoing_parts(targetLine, dumpList.ToArray(), sendingLineTo);
            }
        }
    }
    public void Dump_first_nonViable_part(VehiclePart_Config _targetPart)
    {
        if (getsPartsFrom.currentState != StorageState.FETCHING)
        {
            bool foundOnLine = false;
            int targetLine = -1;
            List<VehiclePart> dumpList = new List<VehiclePart>();
            for (int _lineIndex = 0; _lineIndex < storageLines.Count; _lineIndex++)
            {
                for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
                {
                    VehiclePart _SLOT = Get_data_slot(_lineIndex, _slotIndex);
                    if (_SLOT != null)
                    {
                        if (_SLOT.partConfig != _targetPart && _SLOT.partConfig.partType != Vehicle_PartType.CHASSIS)
                        {
                            dumpList.Add(_SLOT);
                            targetLine = _lineIndex;
                            foundOnLine = true;
                        }
                    }
                }
                if (foundOnLine) break;
            }
            sendingLineTo = getsPartsFrom;
            if (dumpList.Count > 0)
            {
                Change_state(StorageState.DUMP);
                Set_outgoing_parts(targetLine, dumpList.ToArray(), sendingLineTo);
            }
        }
    }

    #region STORAGE BOT

    private void UpdateBot()
    {
        Vector3 botDest;
        if (factor < ANIM_RANGE_FETCH)
        {
            botDest = Vector3.Lerp(nav_parts_IN.position, fetchLine_pos_START, factor / ANIM_RANGE_FETCH);
        }
        else if (factor < ANIM_RANGE_LOAD)
        {
            botDest = fetchLine_pos_START;
            // move parts to bot
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                VehiclePart _PART = Get_data_slot(targetStorageLine, _slotIndex);
                if(_PART !=null){
                    if(parts_OUT.Contains(_PART)){
                        Vector3 _PART_START_POS = storageLines[targetStorageLine].slotPositions[_slotIndex];
                        Vector3 _PART_END_POS = storageBot.slots[_slotIndex].position;
                        _PART.ClearDestination();
                        _PART.transform.position = Vector3.Lerp(_PART_START_POS, _PART_END_POS, (factor - ANIM_RANGE_FETCH) / (ANIM_RANGE_LOAD - ANIM_RANGE_FETCH));
                    }
                }
            }
        }
        else if (factor < ANIM_RANGE_DELIVER)
        {
            botDest = Vector3.Lerp(fetchLine_pos_START, nav_parts_OUT.position, (factor - ANIM_RANGE_LOAD) / (ANIM_RANGE_DELIVER - ANIM_RANGE_LOAD));
            // keep parts with bot
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                VehiclePart _PART = Get_data_slot(targetStorageLine, _slotIndex);
                if (_PART != null)
                {
                    if (parts_OUT.Contains(_PART))
                    {
                        _PART.ClearDestination();
                        _PART.transform.position  = storageBot.slots[_slotIndex].position;
                    }
                }
            }
        }
        else
        {
            botDest = nav_parts_OUT.position;
            // stack parts
            for (int _slotIndex = 0; _slotIndex < lineLength; _slotIndex++)
            {
                VehiclePart _PART = Get_data_slot(targetStorageLine, _slotIndex);
                if (_PART != null)
                {
                    if (parts_OUT.Contains(_PART))
                    {
                        Vector3 _PART_START_POS = storageBot.slots[_slotIndex].position;
                        Vector3 _PART_END_POS = sendingLineTo.nav_parts_IN.position + new Vector3(0f, (float)_slotIndex * 0.25f, 0f);

                        _PART.ClearDestination();
                        _PART.transform.position = Vector3.Lerp(_PART_START_POS, _PART_END_POS, (factor - ANIM_RANGE_DELIVER) / (1f - ANIM_RANGE_DELIVER));
                    }
                }
            }

        }
        storageBot.Destination = botDest;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Vector3 _POS = transform.position;
        if (Factory.INSTANCE == null)
        {
            Factory.INSTANCE = FindObjectOfType<Factory>();
        }

        float cellSize = Factory.INSTANCE.storageCellSize;
        Gizmos.color = colour;
        width = ((linesX * lineLength) + ((linesX - 1) * gutterX)) * cellSize;
        depth = ((linesZ * lines_groupBy_Z) + ((linesZ - 1) * gutterZ)) * cellSize;
        GizmoHelpers.DrawRect(colour, _POS, width, depth, Log());

        Vector3 _START = _POS + new Vector3(0f, 0f, (depth) * factor);
        GizmoHelpers.DrawLine(Color.white, _START, _START + new Vector3(width, 0f, 0f));

        // flash on action?
        if (taskStep == 0)
        {
        }

        if (GIZMOS_DRAW_CELLS)
        {
            clusterCapacity = lineLength * lines_groupBy_Y * lines_groupBy_Z;
            capacity = clusterCapacity * (linesX * linesY * linesZ);
        }
    }
    void Gizmos_DrawCell(Vector3 _cell_POS, Vector3 _cell_SIZE)
    {
        Gizmos.DrawCube(_cell_POS + _cell_SIZE * 0.5f, _cell_SIZE);
        Gizmos.DrawWireCube(_cell_POS, _cell_SIZE);
        GizmoHelpers.DrawRect(colour, _cell_POS, _cell_SIZE.x, _cell_SIZE.y);
    }
}

public enum StorageState
{
    IDLE,
    WAITING,
    FETCHING,
    DUMP,
    WAIT_FOR_PURGED_DATA
}

public class VehiclePartRequest
{
    public VehiclePart_Config part;
    public Storage deliverTo;

    public VehiclePartRequest(VehiclePart_Config _part, Storage _deliverTo)
    {
        part = _part;
        deliverTo = _deliverTo;
    }
}

public class VehicleChassiRequest : VehiclePartRequest
{
    public int chassisVersion;
    public Dictionary<VehiclePart_Config, int> requiredParts;
    public FactoryMode factoryMode;

    public VehicleChassiRequest(VehiclePart_Config _part, int _chassisVersion,
                                Dictionary<VehiclePart_Config, int> _requiredParts, Storage _deliverTo, FactoryMode _factoryMode)
        : base(_part, _deliverTo)
    {
        part = _part;
        chassisVersion = _chassisVersion;
        requiredParts = _requiredParts;
        deliverTo = _deliverTo;
        factoryMode = _factoryMode;
    }
}

public struct StorageLine
{
    public int index;
    public List<VehiclePart> slots;
    public Vector3[] slotPositions;
    public int lineLength;
    public bool empty, full;

    public StorageLine(int _index, int _lineLength, Vector3 _pos)
    {
        index = _index;
        lineLength = _lineLength;
        slots = new List<VehiclePart>(lineLength);
        slotPositions = new Vector3[lineLength];
        empty = true;
        full = false;
        for (int i = 0; i < lineLength; i++)
        {
            slots.Add(null);
            slotPositions[i] = _pos + new Vector3(i * Factory.INSTANCE.storageCellSize, 0f, 0f);
        }
    }
}