using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePart : MonoBehaviour
{

	public VehiclePart_Config partConfig;

	public int age;
	public int id;
    public int temp_score;

    public bool moveToDest = false;
    public Vector3 destination;
    private Transform t;

    private void Awake()
    {
        t = transform;    
    }

    private void Update()
    {
        if(moveToDest){
            t.position += (destination - t.position) / 10;
            moveToDest = Mathf.Abs(Vector3.Distance(t.position,destination)) > 0.1f;
        }
    }
    public void SetDestination(Vector3 _dest){
        destination = _dest;
        moveToDest = true;
    }
    public void ClearDestination(){
        moveToDest = false;
        destination = t.position;
    }
}


public enum Vehicle_PartType
{
	CHASSIS,
	ENGINE,
	WHEEL,
	STEERING,
	SEAT,
	DOOR,
	GLASS,
	EXHAUST
}

// A group of these make up a VehicleDesign - (4 wheels, one engine, 2 seats <-- 7 components)
public class VehiclePart_Assignment
{
	public string name;
	public VehiclePart_Config partConfig;
	public Vector3 position;
	public Quaternion rotation;

	public VehiclePart_Assignment (string _name, VehiclePart_Config _partConfig, Vector3 _position, Quaternion _rotation)
	{
		name = _name;
		partConfig = _partConfig;
		position = _position;
		rotation = _rotation;
	}
}