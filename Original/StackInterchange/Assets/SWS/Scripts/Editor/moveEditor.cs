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
    /// Serves as template for movement script editor inspectors.
    /// <summary>
    //[CustomEditor(typeof(...))]
    public class moveEditor : Editor
    {
        //define Serialized Objects we want to use/control
        //this will be our serialized reference to the inspected script
        public SerializedObject m_Object;

        //serialized events list property
        public SerializedProperty m_List;
        //inspector scrollbar x/y position, modified by mouse input
        public Vector2 scrollPosEvents;
        //whether events settings menu should be visible
        public bool showEventSetup = false;


        //called whenever this inspector window is loaded 
        public virtual void OnEnable()
        {
            //we create a reference to our script object by passing in the target
            m_Object = new SerializedObject(target);
            //get reference to events list
            m_List = m_Object.FindProperty("events");
        }


        //returns PathManager component for later use
        public virtual PathManager GetPathTransform()
        {
            //get pathContainer from serialized property and return its PathManager component
            return m_Object.FindProperty("pathContainer").objectReferenceValue as PathManager;
        }


        //called whenever the inspector gui gets rendered
        public override void OnInspectorGUI()
        {
            //this pulls the relative variables from unity runtime and stores them in the object
            m_Object.Update();

            //show default variables in inspector
            DrawDefaultInspector();

            //get Path Manager component
            var path = GetPathTransform();
            EditorGUILayout.Space();

            //draw bold settings label
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
                //draw message options
                EventSettings();
            }

            //we push our modified variables back to our serialized object
            m_Object.ApplyModifiedProperties();
        }


        public virtual void EventSettings()
        {
            //path is set and boolean for displaying events settings is true
            if (showEventSetup)
            {
                //draw button for hiding events
                if (GUILayout.Button("Hide Events"))
                    showEventSetup = false;

                EditorGUILayout.BeginHorizontal();

                //clear events values
                if (GUILayout.Button("Clear All"))
                {
                    //display custom dialog and wait for user input to clear all events
                    if (EditorUtility.DisplayDialog("Are you sure?",
                        "Do you really want to reset all events and their values?",
                        "Continue",
                        "Cancel"))
                    {
                        m_List.arraySize = 0;
                        showEventSetup = false;
                        return;
                    }
                }

                EditorGUILayout.EndHorizontal();

                //begin a scrolling view inside GUI, pass in Vector2 scroll position 
                scrollPosEvents = EditorGUILayout.BeginScrollView(scrollPosEvents, GUILayout.Height(190));

                //loop through list
                for (int i = 0; i < m_List.arraySize; i++)
                {
                    //draw label with waypoint index, display total count of events for this waypoint
                    EditorGUILayout.HelpBox(i + ". Waypoint", MessageType.None);		
					EditorGUILayout.PropertyField (m_List.GetArrayElementAtIndex(i));
                }
                //ends the scrollview defined above
                EditorGUILayout.EndScrollView();

                //here we check for the last GUI pass, where the Inspector gets drawn
                //the first pass is responsible for the GUI layout of all fields,
                //and if we resize the list in the first pass it throws an error in the second pass
                //this is because the first and the second pass must have the same values on redraw
                if (Event.current.type == EventType.Repaint)
                {
                    //get total list size and set it to the waypoint size,
                    //so each waypoint has one event
                    m_List.arraySize = GetPathTransform().GetWaypointCount();
                }
            }
            else
            {
                //draw button to toggle events
                if (GUILayout.Button("Show Events"))
                    showEventSetup = true;
            }
        }


        //draws a red dot at the walker's path starting point
        [DrawGizmo(GizmoType.Active)]
        static void DrawGizmoStartPoint(splineMove script, GizmoType gizmoType)
        {
            if (script.pathContainer == null) return;

            int maxLength = script.pathContainer.GetPathPoints().Length - 1;
            int index = Mathf.Clamp(script.startPoint, 0, maxLength);
            if (script.reverse) index = maxLength - index;
            Vector3 position = script.pathContainer.GetPathPoints()[index];
            float size = Mathf.Clamp(HandleUtility.GetHandleSize(position) * 0.1f, 0, 0.3f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(position, size);
        }
    }
}