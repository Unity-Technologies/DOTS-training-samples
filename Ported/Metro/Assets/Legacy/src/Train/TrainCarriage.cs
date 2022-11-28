using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainCarriage : MonoBehaviour
{
    public const float CARRIAGE_LENGTH = 5f;
    public const int CARRIAGE_CAPACITY = 10;
    public const float CARRIAGE_SPACING = 0.25f;

    public float positionOnRail;
    public List<Commuter> passengers;
    public List<CommuterNavPoint> seats_FREE;
    public List<CommuterNavPoint> seats_TAKEN;
    public int passengerCount;
    public TrainCarriage_door door_LEFT;
    public TrainCarriage_door door_RIGHT;
    public GameObject[] RecolouredObjects;


    private Transform t;
    private Material mat;

    private void Start()
    {
        t = transform;
        seats_FREE = GetComponentsInChildren<CommuterNavPoint>().ToList();
        seats_TAKEN = new List<CommuterNavPoint>();
    }

    public void UpdateCarriage(float _newPositionOnRail, Vector3 _newPos, Vector3 _newRotation)
    {
        positionOnRail = _newPositionOnRail;
        t.position = _newPos;
        t.LookAt(t.position - _newRotation);
    }

    public bool Doors_OPEN(bool _rightSide = true)
    {
        TrainCarriage_door _DOOR = (_rightSide) ? door_RIGHT : door_LEFT;
        return _DOOR.DoorsOpen();
    }

    public bool Doors_CLOSED(bool _rightSide = true)
    {
        TrainCarriage_door _DOOR = (_rightSide) ? door_RIGHT : door_LEFT;
        return _DOOR.DoorsClosed();
    }

    public void SetColour(Color _lineColour)
    {
        foreach (GameObject _GO in RecolouredObjects)
        {
            foreach (Renderer _R in _GO.GetComponentsInChildren<Renderer>())
            {
                _R.material.color = _lineColour;
            }
        }
    }

    public CommuterNavPoint AssignSeat()
    {
        if (seats_FREE.Count > 0)
        {
            CommuterNavPoint _seat = seats_FREE[(Random.Range(0, seats_FREE.Count - 1))];
            seats_TAKEN.Add(_seat);
            seats_FREE.Remove(_seat);
            return _seat;
        }
        else
        {
            return null;
        }
    }

    public void VacateSeat(CommuterNavPoint _seat)
    {
        seats_TAKEN.Remove(_seat);
    }
}