/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */
 
using UnityEngine;
using UnityEditor;

namespace SWS
{
    /// <summary>
    /// Adds a new Waypoint Manager gameobject to the scene.
    /// <summary>
    public class CreateWPManager : EditorWindow
    {
        //add menu named "Waypoint Manager" to the Window menu
        [MenuItem("Window/Simple Waypoint System/Waypoint Manager")]

        //initialize method
        static void Init()
        {
            //search for a waypoint manager object within current scene
            GameObject wpManager = GameObject.Find("Waypoint Manager");

            //if no waypoint manager object was found
            if (wpManager == null)
            {
                //create a new gameobject with that name
                wpManager = new GameObject("Waypoint Manager");
                //and attach the WaypointManager component to it
                wpManager.AddComponent<WaypointManager>();
            }

            //in both cases, select the gameobject
            Selection.activeGameObject = wpManager;
        }
    }
}