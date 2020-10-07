using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour {

	public static void Set(ref Material _material, UnityEngine.Color _newColour)
	{
		_material.color = _newColour;
	}

	public static void RecolourChildren(Transform _target, UnityEngine.Color _newColour)
	{
		Renderer[] renderers = _target.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.color = _newColour;
		}
	}
}
