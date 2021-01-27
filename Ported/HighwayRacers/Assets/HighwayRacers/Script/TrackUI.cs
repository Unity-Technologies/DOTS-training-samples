using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;

public class TrackUI : MonoBehaviour
{
	public InputField InputField;
	private int TrackSize = 128;

	public int GetTrackSize()
	{
		return TrackSize;
	}


public  void Hello()
	{
		Debug.Log("hello world");
	}

	public void SetSize(float setSize)
	{
		

		int NewTrackSize = TrackSize;
		bool didparse =int.TryParse(InputField.text, out NewTrackSize);
		if (didparse)
		{
			TrackSize = Mathf.RoundToInt(Mathf.Clamp(NewTrackSize, 36, 2048));
			Debug.Log(string.Format("size : {0}", TrackSize));
		}
		
		
	}
	
	
}
