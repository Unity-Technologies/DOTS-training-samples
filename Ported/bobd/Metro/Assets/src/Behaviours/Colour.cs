using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colour {

	public static void Set(ref Material _material, Color _newColour)
	{
		_material.color = _newColour;
	}

	public static void RecolourChildren(Transform _target, Color _newColour)
	{
		Renderer[] renderers = _target.GetComponentsInChildren<Renderer>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", _newColour);

		for (int i = 0; i < renderers.Length; i++)
		{
            renderers[i].SetPropertyBlock(mpb);
			//renderers[i].material.color = _newColour;
		}
	}
}
