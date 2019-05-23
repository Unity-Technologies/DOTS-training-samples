/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SWS
{
    /// <summary>
    /// Custom inspector for navMove.
    /// <summary>
    [CustomEditor(typeof(navMove))]
    public class navMoveEditor : moveEditor
    {
        public override void OnInspectorGUI()
        {
            //this pulls the relative variables from unity runtime and stores them in the object
            m_Object.Update();
            //DrawDefaultInspector();
            
            //draw custom variable properties by using serialized properties
            EditorGUILayout.PropertyField(m_Object.FindProperty("pathContainer"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("onStart"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("moveToPath"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("reverse"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("updateRotation"));
            EditorGUILayout.PropertyField(m_Object.FindProperty("startPoint"));
            SerializedProperty loopType = m_Object.FindProperty("loopType");
            EditorGUILayout.PropertyField(loopType);
            if (loopType.enumValueIndex == 1)
                EditorGUILayout.PropertyField(m_Object.FindProperty("closeLoop"));

            //get Path Manager component
            var path = GetPathTransform();
            EditorGUILayout.Space();

            //draw bold delay settings label
            GUILayout.Label("Settings:", EditorStyles.boldLabel);

            //check whether a Path Manager component is set, if not display a label
            if (path == null)
            {
                GUILayout.Label("No path set.");
                //clear old events data if no path is set
                m_List.arraySize = 0;
            }
            else
            {
                //draw events
                EventSettings();
            }

            //we push our modified variables back to our serialized object
            m_Object.ApplyModifiedProperties();
        }

        //if this path is selected, display small info boxes above all waypoint positions
        void OnSceneGUI()
        {
            //get Path Manager component
            var path = GetPathTransform();

            //do not execute further code if we have no path defined
            //or delay settings are not visible
            if (path == null) return;

            //get waypoints array of Path Manager
            var waypoints = path.waypoints;
            Vector3 wpPos = Vector3.zero;
            float size = 1f;

            //loop through waypoint array
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (!waypoints[i]) continue;
                wpPos = waypoints[i].position;
                size = HandleUtility.GetHandleSize(wpPos) * 0.4f;

                //do not draw waypoint header if too far away
                if (size < 3f)
                {
                    //begin 2D GUI block
                    Handles.BeginGUI();
                    //translate waypoint vector3 position in world space into a position on the screen
                    var guiPoint = HandleUtility.WorldToGUIPoint(wpPos);
                    //create rectangle with that positions and do some offset
                    var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 100, 20);
                    //draw box at rect position with current waypoint name
                    GUI.Box(rect, waypoints[i].name);
                    Handles.EndGUI(); //end GUI block
                }
            }
        }
    }
}