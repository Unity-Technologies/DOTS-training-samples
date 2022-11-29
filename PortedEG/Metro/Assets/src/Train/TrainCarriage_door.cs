using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCarriage_door : MonoBehaviour
{
    private const float DOOR_ACCELERATION = 0.0015f;
    private const float DOOR_FRICTION = 0.9f;
    private const float DOOR_ARRIVAL_THRESHOLD = 0.001f;
    
    public Transform door_LEFT, door_RIGHT;
    private float left_OPEN_X, left_CLOSED_X;
    private float door_SPEED = 0f;
    public CommuterNavPoint door_navPoint;

    private void Start()
    {
        left_CLOSED_X = door_LEFT.localPosition.x;
        left_OPEN_X = left_CLOSED_X - door_LEFT.localScale.x;
    }

    public bool DoorsOpen()
    {
        Vector3 _DOOR_POS = door_LEFT.localPosition;
        bool arrived = Approach.Apply(ref _DOOR_POS.x, ref door_SPEED, left_OPEN_X, DOOR_ACCELERATION,
            DOOR_ARRIVAL_THRESHOLD, DOOR_FRICTION);
        door_LEFT.localPosition = _DOOR_POS;
        door_RIGHT.localPosition = new Vector3(-_DOOR_POS.x, _DOOR_POS.y, _DOOR_POS.z);
        return arrived;
    }

    public bool DoorsClosed()
    {
        Vector3 _DOOR_POS = door_LEFT.localPosition;
        bool arrived = Approach.Apply(ref _DOOR_POS.x, ref door_SPEED, left_CLOSED_X, DOOR_ACCELERATION,
            DOOR_ARRIVAL_THRESHOLD, DOOR_FRICTION);
        door_LEFT.localPosition = _DOOR_POS;
        door_RIGHT.localPosition = new Vector3(-_DOOR_POS.x, _DOOR_POS.y, _DOOR_POS.z);
        return arrived;
    }
}
