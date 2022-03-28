using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Approach {

	public static bool Apply(ref float _current, ref float _speed, float _target, float _acceleration, float _arrivalThreshold, float _friction)
	{
		_speed *= _friction;
		if (_current < (_target - _arrivalThreshold))
		{
			_speed += _acceleration;
			_current += _speed;
			return false;
		}else if (_current > (_target + _arrivalThreshold))
		{
			_speed -= _acceleration;
			_current += _speed;
			return false;
		}
		else
		{
			return true;
		}
	}

	public static bool Apply(ref Transform _transform, ref Vector3 _speed, Vector3 _destination, float _acceleration,
		float _arrivalThreshold, float _friction)
	{
		Vector3 _POS = _transform.position;
		
		bool arrivedX = Approach.Apply(ref _POS.x, ref _speed.x, _destination.x, _acceleration, _arrivalThreshold, _friction);
		bool arrivedY = Approach.Apply(ref _POS.y, ref _speed.y, _destination.y, _acceleration, _arrivalThreshold, _friction);
		bool arrivedZ = Approach.Apply(ref _POS.z, ref _speed.z, _destination.z, _acceleration, _arrivalThreshold, _friction);

		_transform.position = _POS;
		
		return (arrivedX && arrivedY && arrivedZ);
	}
}
