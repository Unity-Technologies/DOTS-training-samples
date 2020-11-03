/*
// 	Author: DC Turner  
//	www.dcturner.net
// 	
//	Send question or comments to: mrdcturner@gmail.com
// 	Twitter:  @DcTurner
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using WorkFast;

public class AlignAndDistribute_Logic
{
	

	#if UNITY_EDITOR
	public static Transform[] selectedObjects;
	public enum Align {lowest, center, highest};
	public enum Axis {x,y,z};


	// VALIDATION - whether or not to grey out menu items, based on how many objects are selected
	static bool validate_align()
	{
	return Selection.transforms.Length >1;
	}

	static bool validate_distribute()
	{
	return Selection.transforms.Length >2;
	}

	//--------------------------------------------------------------
	// ALIGNMENT
	//--------------------------------------------------------------
	
	// ALIGN X
	//[MenuItem ("GameObject/Align/x/lowest",false,0)]
	[MenuItem("GameObject/align/x/lowest", false,10)]
	public static void align_lowestX()
	{
		AlignSelection(Axis.x, Align.lowest);
	}
	// validate 
	[MenuItem ("GameObject/align/x/lowest",true)]
	static bool check_align_x_low() {
	return validate_align();
	}
		
	[MenuItem ("GameObject/align/x/middle",false,10)]
	public static void align_middleX()
	{
		AlignSelection(Axis.x, Align.center);
	}
	// validate 
	[MenuItem ("GameObject/align/x/middle", true)]
	static bool check_align_x_middle() {
	return validate_align();
	}

	[MenuItem ("GameObject/align/x/highest",false,10)]
	public static void align_highestX()
	{
		AlignSelection(Axis.x, Align.highest);
	}

	// validate 
	[MenuItem ("GameObject/align/x/highest", true)]

	static bool check_align_x_high() {
	return validate_align();
	}
	
	// ALIGN Y
	[MenuItem ("GameObject/align/y/lowest",false,10)]
	public static void align_lowestY()
	{
		AlignSelection(Axis.y, Align.lowest);
	}
	// validate 
	[MenuItem ("GameObject/align/y/lowest", true)]
	static bool check_align_y_low() {
	return validate_align();
	}

	[MenuItem ("GameObject/align/y/middle",false,10)]
	public static void align_middleY()
	{
		AlignSelection(Axis.y, Align.center);
	}
	// validate 
	[MenuItem ("GameObject/Align/y/middle", true)]
	static bool check_align_y_middle() {
	return validate_align();
	}

	[MenuItem ("GameObject/align/y/highest",false,10)]
	public static void align_highestY()
	{
		AlignSelection(Axis.y, Align.highest);
	}
	// validate 
	[MenuItem ("GameObject/Align/y/highest", true)]
	static bool check_align_y_high() {
	return validate_align();
	}
	
	// ALIGN Z
	[MenuItem ("GameObject/align/z/lowest", false,10)]
	public static void align_lowestZ()
	{
		AlignSelection(Axis.z, Align.lowest);
	}
	// validate 
	[MenuItem ("GameObject/Align/z/lowest", true)]
	static bool check_align_z_low() {
	return validate_align();
	}

	[MenuItem ("GameObject/align/z/middle",false,10)]
	public static void align_middleZ()
	{
		AlignSelection(Axis.z, Align.center);
	}
	// validate 
	[MenuItem ("GameObject/Align/z/middle", true)]
	static bool check_align_z_middle() {
	return validate_align();
	}

	[MenuItem ("GameObject/align/z/highest",false,10)]
	public static void align_highestZ()
	{
		AlignSelection(Axis.z, Align.highest);
	}
	// validate 
	[MenuItem ("GameObject/Align/z/highest", true)]
	static bool check_align_z_high() {
	return validate_align();
	}
	
	/// <summary>
	/// Aligns the selected GameObjects
	/// </summary>
	/// <param name='a'>
	/// Axis to align
	/// </param>
	/// <param name='align'>
	/// Would you like to align to the lowest, middle or highest object?
	/// </param>
	static public void AlignSelection(Axis a, Align align)
	{
		try {
			selectedObjects = Selection.transforms; // make array of all selected objects
			if(selectedObjects.Length==0)return; // if nothing is selected, do nothing

			#if UNITY_4_2
			Undo.CreateSnapshot();
			Undo.RegisterUndo(selectedObjects, "Align by "+a.ToString()+" Axis"); // make an undo state for this action
			#else
			Undo.RecordObjects(Selection.transforms, "Align by "+a.ToString()+" Axis");
			#endif

			
			Vector3 desiredLocation = Vector3.zero;
			
			selectedObjects =  getSelectedByAxisOrder(a);
			
			switch (align) {
				
				case Align.lowest:
					desiredLocation = selectedObjects[0].position; // Lowest = first item in the Ordered Array
				break;
				
				case Align.highest:
					desiredLocation = selectedObjects[selectedObjects.Length-1].position;	// Highest = last item in the ordered array
				break;
				
				case Align.center:
					float sum = 0;
					foreach (Transform t in selectedObjects) {
						switch (a) {
							case Axis.x:
							sum += t.position.x;
							break;
							case Axis.y:
							sum += t.position.y;
							break;
							case Axis.z:
							sum += t.position.z;
							break;
						}
					}
					float mean = sum / selectedObjects.Length;
				
					switch (a) {
					case Axis.x:
						desiredLocation = new Vector3(mean, desiredLocation.y, desiredLocation.z);
					break;
					case Axis.y:
						desiredLocation = new Vector3(desiredLocation.x, mean, desiredLocation.z);
					break;
					case Axis.z:
						desiredLocation = new Vector3(desiredLocation.x, desiredLocation.y, mean);
					break;
				}
				break;
				
			}	
		
			// set the desired axis value to 'desiredLocation'
			foreach (Transform item in selectedObjects) {
				switch (a) {
					case Axis.x:
						item.setX(desiredLocation.x);
					break;
					case Axis.y:
						item.setY(desiredLocation.y);
					break;
					case Axis.z:
						item.setZ(desiredLocation.z);
					break;
				}
			}
			
		} catch (System.Exception ex) {
		Debug.Log("Error Encountered attempting to align objects: " + ex.Message);
		Debug.Log("Send questions and bug reports to mrdcturner@gmail.com, or find me on twitter @DcTurner");
		}
	}
	
	
	//--------------------------------------------------------------
	// DISTRIBUTE
	//--------------------------------------------------------------
	[MenuItem ("GameObject/distribute/X Axis",false,10)]
	public static void distributeHorizontal()
	{
		Distribute(Axis.x);
	}
	// validate X
	[MenuItem ("GameObject/Distribute/X Axis", true)]
	static bool check_distX() {
	return validate_distribute();
	}

	[MenuItem ("GameObject/distribute/Y Axis",false,10)]
	public static void distributeVertical()
	{
		Distribute(Axis.y);
	}
	// validate Y
	[MenuItem ("GameObject/Distribute/Y Axis", true)]
	static bool check_distY() {
	return validate_distribute();
	}

	[MenuItem ("GameObject/distribute/Z Axis",false,10)]
	public static void distributeDepth()
	{
		Distribute(Axis.z);
	}
	// validate Y
	[MenuItem ("GameObject/Distribute/Z Axis", true)]
	static bool check_distZ() {
	return validate_distribute();
	}
	
	/// <summary>
	/// Distribute the selected gameobjects along a chosen axis
	/// </summary>
	/// <param name='a'>
	/// Axis to align
	/// </param>
	public static void Distribute(Axis a)
	{
		try {


			selectedObjects = getSelectedByAxisOrder(a);
			if(selectedObjects.Length==0)return; // if nothing is selected, do nothing
			
			#if UNITY_4_2
			Undo.CreateSnapshot();
			Undo.RegisterUndo(selectedObjects, "Distribute by by "+a.ToString()+" Axis"); // make an undo state for this action
			#else
			Undo.RecordObjects(Selection.transforms, "Distribute by "+a.ToString()+" Axis");
			#endif



			int totalObjects = selectedObjects.Length;
			
			// find the lowest and highest values
			Transform lowest = selectedObjects[0];
			Transform highest = selectedObjects[totalObjects-1];
			
			float range = getDistanceByAxis(lowest.position, highest.position, a);
			float div = range / (totalObjects-1);
			
			for (int i = 0; i < totalObjects; i++) {
				Vector3 myPos = selectedObjects[i].position;
				selectedObjects[i].position = getIncrementLocation(div, lowest.position, myPos, a, i);
			}
		} catch (System.Exception ex) {
			Debug.Log("Error Encountered attempting to distribute objects: " + ex.Message);
			Debug.Log("Send questions and bug reports to mrdcturner@gmail.com, or find me on twitter @DcTurner");
		}
	}
	
	private static Vector3 getIncrementLocation(float div, Vector3 startPos, Vector3 obJPos,  Axis a, int inc)
	{
		Vector3 newPos = obJPos;
		
		switch (a) {
		case Axis.x:
			newPos.x = startPos.x + (div*inc);
			break;
			case Axis.y:
			newPos.y = startPos.y + (div*inc);
			break;
			case Axis.z:
			newPos.z = startPos.z + (div*inc);
			break;
		}
		return newPos;
	}
	
	private static Transform[] getSelectedByAxisOrder(Axis a)
	{
		Transform[] result = new Transform[0];
		
		switch(a)
		{
			case Axis.x:
			result = Selection.transforms.OrderBy(t => t.position.x).ToArray();
			break;
			
			case Axis.y:
			result = Selection.transforms.OrderBy(t => t.position.y).ToArray();
			break;
			
			case Axis.z:
			result = Selection.transforms.OrderBy(t => t.position.z).ToArray();
			break;
		}
		
		return result;
	}
	
	/// How far apart are the Vectors, according to each axis?
	private static float getDistanceByAxis(Vector3 lowest, Vector3 highest, Axis a)
	{
		float high = 0, low = 0;
			
		switch (a) {
			case Axis.x:
				high = highest.x;
				low = lowest.x;
			break;
			case Axis.y:
				high = highest.y;
				low = lowest.y;
			break;
			case Axis.z:
				high = highest.z;
				low = lowest.z;
			break;
		}
		
		return high - low;
	}
	
	//--------------------------------------------------------------
	// SPACING
	//--------------------------------------------------------------
	public static void ApplySpacingByScale(Axis a, float div, bool useLocal = false)
	{
		switch (a) {
			case Axis.x:
				selectedObjects = Selection.transforms.OrderBy(t => (t.position.x + t.localScale.x)).ToArray();
			break;
			case Axis.y:
				selectedObjects = Selection.transforms.OrderBy(t => (t.position.y + t.localScale.y)).ToArray();
			break;
			case Axis.z:
				selectedObjects = Selection.transforms.OrderBy(t => (t.position.z + t.localScale.z)).ToArray();
			break;
		}
		
		for (int i = 1; i < selectedObjects.Length; i++) {
			Transform current = selectedObjects[i];
			Transform prev = selectedObjects[i-1];
			float newLocation;
			switch (a) {
				case Axis.x:
					newLocation = (useLocal) ? (prev.localPosition.x + prev.localScale.x) : (prev.position.x + prev.localScale.x);
					current.setX(newLocation + div);
				break;
				case Axis.y:
					newLocation = (useLocal) ? (prev.localPosition.y + prev.localScale.y) : (prev.position.y + prev.localScale.y);
					current.setY(newLocation + div);
				break;
				case Axis.z:
					newLocation = (useLocal) ? (prev.localPosition.z + prev.localScale.z) : (prev.position.z + prev.localScale.z);
					current.setZ(newLocation + div);
				break;
			}
		}
	}
	
	#endif
}
