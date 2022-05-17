/*
Author: DC Turner  
www.dcturner/net
 	
Send question or comments to: mrdcturner@gmail.com
Twitter:  @DcTurner

Information:  This is a short collection of C# Extensions that allow you to position GameObject and Transform objects more quickly, by setting individual x,y,z values;

How to Use: 
	1) If the WorkFast.cs file is included in your Unity project, enter "using WorkFast;" at the top. 
	2) The Transform and Gameobject classes should now have 3 new methods available; setX(), setY() and setZ();
	3) For extra control, you can specify whether to move the object in global or local space.
	
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -	
example;  
this.gameObject.setX(1000); // Moves the gameObject to x:1000 in global space. 

To use local coordinates, use;   
this.gameObject.setX(1000,true); // Moves the gameObject to x:1000 in local space. 
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

I hope you find these extensions useful :)

All the best,
DcTurner
*/

using UnityEngine;
using System.Collections;

namespace WorkFast{

	public static class WorkFast {
	
		//--------------------------------------------------------------
		// Transform Extends
		//--------------------------------------------------------------
		
		/// <summary>
		/// Sets the x property 
		/// </summary>
		/// <param name='newValue'>
		/// New position.
		/// </param>
		/// <param name='useLocal'>
		/// Allows you to choose local or global positioning (false: transform.position, true: transform.localPosition)
		/// </param>
		public static void setX(this Transform t, float newValue, bool useLocal = false)
		{
			if(!useLocal){
				Vector3 pos = t.position;
				t.position = new Vector3(newValue, pos.y, pos.z);	
			}else{
				Vector3 posLocal = t.localPosition;
				t.localPosition = new Vector3(newValue, posLocal.y, posLocal.z);
			}
		}
		
		/// <summary>
		/// Sets the y property 
		/// </summary>
		/// <param name='newValue'>
		/// New position.
		/// </param>
		/// <param name='useLocal'>
		/// Allows you to choose local or global positioning (false: transform.position, true: transform.localPosition)
		/// </param>
		public static void setY(this Transform t, float newValue, bool useLocal = false)
		{
			if(!useLocal){
				Vector3 pos = t.position;
				t.position = new Vector3(pos.x, newValue, pos.z);	
			}else{
				Vector3 posLocal = t.localPosition;
				t.localPosition = new Vector3(posLocal.x, newValue, posLocal.z);
			}
		}
		
		/// <summary>
		/// Sets the z property 
		/// </summary>
		/// <param name='newValue'>
		/// New position.
		/// </param>
		/// <param name='useLocal'>
		/// Allows you to choose local or global positioning (false: transform.position, true: transform.localPosition)
		/// </param>
		public static void setZ(this Transform t, float newValue, bool useLocal = false)
		{
			if(!useLocal){
				Vector3 pos = t.position;
				t.position = new Vector3(pos.x, pos.y, newValue);	
			}else{
				Vector3 posLocal = t.localPosition;
				t.localPosition = new Vector3(posLocal.x, posLocal.y, newValue);
			}
			
		}
		
		//--------------------------------------------------------------
		// GameObject Extends
		//--------------------------------------------------------------
		/// <summary>
		/// Sets the x property 
		/// </summary>
		/// <param name='newValue'>
		/// New position.
		/// </param>
		/// <param name='useLocal'>
		/// Allows you to choose local or global positioning (false: transform.position, true: transform.localPosition)
		/// </param>
		public static void setX(this GameObject go, float newValue, bool useLocal = false)
		{
			if(!useLocal){
				Vector3 pos = go.transform.position;
				go.transform.position = new Vector3(newValue, pos.y, pos.z);	
			}else{
				Vector3 posLocal = go.transform.localPosition;
				go.transform.localPosition = new Vector3(newValue, posLocal.y, posLocal.z);
			}
		}
		
		/// <summary>
		/// Sets the y property 
		/// </summary>
		/// <param name='newValue'>
		/// New position.
		/// </param>
		/// <param name='useLocal'>
		/// Allows you to choose local or global positioning (false: transform.position, true: transform.localPosition)
		/// </param>
		public static void setY(this GameObject go, float newValue, bool useLocal = false)
		{
			if(!useLocal){
				Vector3 pos = go.transform.position;
				go.transform.position = new Vector3(pos.x, newValue, pos.z);	
			}else{
				Vector3 posLocal = go.transform.localPosition;
				go.transform.localPosition = new Vector3(posLocal.x, newValue, posLocal.z);
			}
		}
		
		/// <summary>
		/// Sets the z property 
		/// </summary>
		/// <param name='newValue'>
		/// New position.
		/// </param>
		/// <param name='useLocal'>
		/// Allows you to choose local or global positioning (false: transform.position, true: transform.localPosition)
		/// </param>
		public static void setZ(this GameObject go, float newValue, bool useLocal = false)
		{
			if(!useLocal){
				Vector3 pos = go.transform.position;
				go.transform.position = new Vector3(pos.x, pos.y, newValue);	
			}else{
				Vector3 posLocal = go.transform.localPosition;
				go.transform.localPosition = new Vector3(posLocal.x, posLocal.y, newValue);
			}
			
		}
	}
}
