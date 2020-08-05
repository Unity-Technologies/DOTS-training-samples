/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SWS
{
    /// <summary>
    /// Custom bezier path inspector.
    /// <summary>
    [CustomEditor(typeof(BezierPathManager))]
    public class BezierPathEditor : Editor
    {
        //define reference we want to use/control
        private BezierPathManager script;
        //optional segment detail button toggle
        private bool showDetailSettings = false;
        //inspector scrollbar x/y position, modified by mouse input
        private Vector2 scrollPosDetail;
        //currently selected waypoint node for position/rotation editing
        private int activeNode = -1;
        //modifier action that executes specific path manipulations
        private PathModifierOption editOption = PathModifierOption.SelectModifier;


        //called whenever this inspector window is loaded 
        public void OnEnable()
        {
            //we create a reference to our script object by passing in the target
            script = (BezierPathManager)target;
            if (script.bPoints.Count == 0) return;

            //reposition handles of the first and last point to the waypoint
            //they only have one control point so we set the other one to zero
            BezierPoint first = script.bPoints[0];
            first.cp[0].position = first.wp.position;
            BezierPoint last = script.bPoints[script.bPoints.Count - 1];
            last.cp[1].position = last.wp.position;

            //recalculate path points
            script.CalculatePath();
        }


        //adds a waypoint when clicking on the "+" button in the inspector
        private void AddWaypointAtIndex(int index)
        {
            //create a new bezier point property class
            BezierPoint point = new BezierPoint();
            //create new waypoint gameobject
            Transform wp = new GameObject("Waypoint " + (index + 1)).transform;

            //disabled because of a Unity bug that crashes the editor
            //Undo.RecordObject(script, "Add");
            //Undo.RegisterCreatedObjectUndo(wp, "Add");

            //set its position to the last one
            wp.position = script.bPoints[index].wp.position;
            //assign it to the class
            point.wp = wp;
            
            //assign new control points
            Transform left = new GameObject("Left").transform;
            Transform right = new GameObject("Right").transform;
            left.parent = right.parent = wp;

            //adjust control point position offsets
            left.position = wp.position;
            if(index != 0)
                left.position += new Vector3(2, 0, 0);
            right.position = wp.position;
            if(index + 1 != script.bPoints.Count)
                right.position += new Vector3(-2, 0, 0);

            point.cp = new[] { left, right };
            //parent bezier point to the path gameobject
            wp.parent = script.transform;
            wp.SetSiblingIndex(index + 1);
            //add new detail value for the new segment
            script.segmentDetail.Insert(index + 1, script.pathDetail);
            //finally, insert this new waypoint after the one clicked
            script.bPoints.Insert(index + 1, point);
            RenameWaypoints(true);
            activeNode = index + 1;
        }


        //removes a waypoint when clicking on the "-" button in the inspector
        private void RemoveWaypointAtIndex(int index)
        {
            //reset waypoint selection
            activeNode = -1;
            Undo.RecordObject(script, "Remove Waypoint");
            //remove corresponding detail value
            script.segmentDetail.RemoveAt(index - 1);
            //remove the point from the list
            Undo.DestroyObjectImmediate(script.bPoints[index].wp.gameObject);
            script.bPoints.RemoveAt(index);
            RenameWaypoints(true);
        }


        public override void OnInspectorGUI()
        {
            //don't draw inspector fields if the path contains less than 2 points
            //(a path with less than 2 points really isn't a path)
            if (script.bPoints.Count < 2)
            {
                //button to create path manually
                if (GUILayout.Button("Create Path from Children"))
                {
                    Undo.RecordObject(script, "Create Path");
                    script.Create();
                    SceneView.RepaintAll();
                }

                return;
            }

            //create new checkboxes for path gizmo property 
            script.showHandles = EditorGUILayout.Toggle("Show Handles", script.showHandles);
            script.connectHandles = EditorGUILayout.Toggle("Connect Handles", script.connectHandles);
            script.drawCurved = EditorGUILayout.Toggle("Draw Smooth Lines", script.drawCurved);
            script.drawDirection = EditorGUILayout.Toggle("Draw Direction", script.drawDirection);

            //create new color fields for editing path gizmo colors 
            script.color1 = EditorGUILayout.ColorField("Color1", script.color1);
            script.color2 = EditorGUILayout.ColorField("Color2", script.color2);
            script.color3 = EditorGUILayout.ColorField("Color3", script.color3);

            //calculate path length of all waypoints
            float pathLength = WaypointManager.GetPathLength(script.pathPoints);
            GUILayout.Label("Path Length: " + pathLength);

            float thisDetail = script.pathDetail;
            //slider to modify the smoothing factor of the final path,
            //round because of path point imprecision placement (micro loops)
            script.pathDetail = EditorGUILayout.Slider("Path Detail", script.pathDetail, 0.5f, 10);
            script.pathDetail = Mathf.Round(script.pathDetail * 10f) / 10f;
            //toggle custom detail when modifying the whole path
            if (thisDetail != script.pathDetail)
                script.customDetail = false;
            //draw custom detail settings
            DetailSettings();

            //button for switching over to the WaypointManager for further path editing
            if (GUILayout.Button("Continue Editing"))
            {
                Selection.activeGameObject = (GameObject.FindObjectOfType(typeof(WaypointManager)) as WaypointManager).gameObject;
                WaypointEditor.ContinuePath(script);
            }

            //more path modifiers
            DrawPathOptions();
            EditorGUILayout.Space();

            //waypoint index header
            GUILayout.Label("Waypoints: ", EditorStyles.boldLabel);

            //loop through the waypoint array
            for (int i = 0; i < script.bPoints.Count; i++)
            {
                GUILayout.BeginHorizontal();
                //indicate each array slot with index number in front of it
                GUILayout.Label(i + ".", GUILayout.Width(20));

                //create an object field for every waypoint
                EditorGUILayout.ObjectField(script.bPoints[i].wp, typeof(Transform), true);

                //display an "Add Waypoint" button for every array row except the last one
                //on click we call AddWaypointAtIndex() to insert a new waypoint slot AFTER the selected slot
                if (i < script.bPoints.Count && GUILayout.Button("+", GUILayout.Width(30f)))
                {
                    AddWaypointAtIndex(i);
                    break;
                }

                //display an "Remove Waypoint" button for every array row except the first and last one
                //on click we call RemoveWaypointAtIndex() to delete the selected waypoint slot
                if (i > 0 && i < script.bPoints.Count - 1 && GUILayout.Button("-", GUILayout.Width(30f)))
                {
                    RemoveWaypointAtIndex(i);
                    break;
                }

                GUILayout.EndHorizontal();
            }

            //recalculate on inspector changes
            if (GUI.changed)
            {
                script.CalculatePath();
                EditorUtility.SetDirty(target);
            }
        }


        private void DetailSettings()
        {
            if (showDetailSettings)
            {
                if (GUILayout.Button("Hide Detail Settings"))
                    showDetailSettings = false;

                //draw bold settings checkbox
                GUILayout.Label("Segment Detail:", EditorStyles.boldLabel);
                script.customDetail = EditorGUILayout.Toggle("Enable Custom", script.customDetail);

                EditorGUILayout.BeginHorizontal();
                //begin a scrolling view inside GUI, pass in Vector2 scroll position 
                scrollPosDetail = EditorGUILayout.BeginScrollView(scrollPosDetail, GUILayout.Height(105));

                //loop through waypoint array
                for (int i = 0; i < script.bPoints.Count - 1; i++)
                {
                    float thisDetail = script.segmentDetail[i];
                    //create a float slider for every segment detail setting
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(i + "-" + (i + 1) + ".");
                    script.segmentDetail[i] = EditorGUILayout.Slider(script.segmentDetail[i], 0.5f, 10);
                    script.segmentDetail[i] = Mathf.Round(script.segmentDetail[i] * 10f) / 10f;
                    EditorGUILayout.EndHorizontal();
                    //toggle custom detail when modifying individual segments
                    if (thisDetail != script.segmentDetail[i])
                        script.customDetail = true;
                }

                //ends the scrollview defined above
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                //if path is set but detail settings are not shown,
                //draw button to toggle showDelaySetup
                if (GUILayout.Button("Show Detail Settings"))
                    showDetailSettings = true;
            }
        }


        private void ReplaceWaypoints()
        {
            if (script.replaceObject == null)
            {
                Debug.LogWarning("You haven't specified a replace object. Cancelling.");
                return;
            }

            Undo.RecordObject(script, "Replace");

            //old waypoints to remove after replace
            List<GameObject> toRemove = new List<GameObject>();
            //loop through waypoint list
            for (int i = 0; i < script.bPoints.Count; i++)
            {
                //get current bezier point at index position
                BezierPoint point = script.bPoints[i];
                Transform curWP = point.wp;
                //instantiate new waypoint at old position
                Transform newCur = ((GameObject)Instantiate(script.replaceObject, curWP.position, Quaternion.identity)).transform;
                Undo.RegisterCreatedObjectUndo(newCur.gameObject, "Replace");
                
                //parent control points to the new bezier point
                Undo.SetTransformParent(point.cp[0], newCur, "Replace");
                Undo.SetTransformParent(point.cp[1], newCur, "Replace");
                //parent new waypoint to this path
                newCur.parent = point.wp.parent;
                
                //replace old waypoint at index
                script.bPoints[i].wp = newCur;
                //indicate to remove old waypoint
                toRemove.Add(curWP.gameObject);
            }

            //destroy old waypoints
            for (int i = 0; i < toRemove.Count; i++)
                Undo.DestroyObjectImmediate(toRemove[i]);
        }


        //if this path is selected, display small info boxes above all waypoint positions
        //also display handles for the waypoints and their bezier points
        void OnSceneGUI()
        {
            //do not execute further code if we have no waypoints defined
            //(just to make sure, practically this can not occur)
            if (script.bPoints.Count == 0) return;
            Vector3 wpPos = Vector3.zero;
            float size = 1f;

            //handles
            for (int i = 0; i < script.bPoints.Count; i++)
            {
                //get related bezier point class
                BezierPoint point = script.bPoints[i];
                if (point == null || !point.wp) continue;
                wpPos = point.wp.position;
                size = HandleUtility.GetHandleSize(wpPos) * 0.4f;

                if (size < 3f)
                {
                    //begin GUI block
                    Handles.BeginGUI();
                    //translate waypoint vector3 position in world space into a position on the screen
                    var guiPoint = HandleUtility.WorldToGUIPoint(wpPos);
                    //create rectangle with that positions and do some offset
                    var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 100, 20);
                    //draw box at position with current waypoint name
                    GUI.Box(rect, point.wp.name);
                    Handles.EndGUI(); //end GUI block
                }

                //draw bezier point handles, clamp size
                Handles.color = script.color2;
                size = Mathf.Clamp(size, 0, 1.2f);
                
                #if UNITY_5_6_OR_NEWER
                Handles.FreeMoveHandle(wpPos, Quaternion.identity, size, Vector3.zero, (controlID, position, rotation, hSize, eventType) => 
                {
                    Handles.SphereHandleCap(controlID, position, rotation, hSize, eventType);
                    if(controlID == GUIUtility.hotControl && GUIUtility.hotControl != 0)
                        activeNode = i;
                });
                #else
                Handles.FreeMoveHandle(wpPos, Quaternion.identity, size, Vector3.zero, (controlID, position, rotation, hSize) => 
                {
                    Handles.SphereCap(controlID, position, rotation, hSize);
                    if(controlID == GUIUtility.hotControl && GUIUtility.hotControl != 0)
                        activeNode = i;
                });
                #endif

                Handles.RadiusHandle(point.wp.rotation, wpPos, size / 2);
            }
            
            if(activeNode > -1)
            {
                BezierPoint point = script.bPoints[activeNode];
                Handles.color = script.color3;


                Quaternion wpRot = script.bPoints[activeNode].wp.rotation;
                switch(Tools.current)
                {
                    case Tool.Move:
                        //draw control point handles
                        //left handle (0): all control points except first one
                        //right handle (1): all waypoints except last one
                        for (int i = 0; i <= 1; i++)
                        {
                            if (i == 0 && activeNode == 0) continue;
                            if (i == 1 && activeNode == script.bPoints.Count - 1) continue;

                            size = HandleUtility.GetHandleSize(point.cp[i].position) * 0.25f;
                            size = Mathf.Clamp(size, 0, 0.5f);
                            wpPos = point.cp[i].position;
                            
                            #if UNITY_5_6_OR_NEWER
                            Handles.SphereHandleCap(activeNode, wpPos, Quaternion.identity, size, EventType.Repaint);
                            #else
                            Handles.SphereCap(activeNode, wpPos, Quaternion.identity, size);
                            #endif

                            wpPos = Handles.PositionHandle(wpPos, Quaternion.identity);
                            if (Vector3.Distance(point.cp[i].position, wpPos) > 0.01f)
                            {
                                Undo.RecordObject(point.cp[i].transform, "Move Control Point");
                                PositionOpposite(point, i == 0 ? true : false, wpPos);
                            }
                        }

                        //draw line between control points
                        Handles.DrawLine(point.cp[0].position, point.cp[1].position);
                        wpPos = script.bPoints[activeNode].wp.position;

                        if (Tools.pivotRotation == PivotRotation.Global)
                            wpRot = Quaternion.identity;
                            
                        Vector3 newPos = Handles.PositionHandle(wpPos, wpRot);
                        if(wpPos != newPos)
                        {
                            Undo.RecordObject(script.bPoints[activeNode].wp, "Move Handle");
                            script.bPoints[activeNode].wp.position = newPos;
                        }
                        break;

                    case Tool.Rotate:
                        wpPos = script.bPoints[activeNode].wp.position;
                        Quaternion newRot = Handles.RotationHandle(wpRot, wpPos);

                        if (wpRot != newRot) 
                        {
                            //save child rotations before applying waypoint rotation
                            Vector3[] globalPos = new Vector3[script.bPoints[activeNode].wp.childCount];
                            for (int i = 0; i < globalPos.Length; i++)
                                globalPos[i] = script.bPoints[activeNode].wp.GetChild(i).position;

                            Undo.RecordObject(script.bPoints[activeNode].wp, "Rotate Handle");
                            script.bPoints[activeNode].wp.rotation = newRot;

                            //restore previous location after rotation
                            for (int i = 0; i < globalPos.Length; i++)
                                script.bPoints[activeNode].wp.GetChild(i).position = globalPos[i];
                        }
                        break;
                }
            }

            if (GUI.changed)
                EditorUtility.SetDirty(target);

            //recalculate path points after handles
            script.CalculatePath();

            if (!script.showHandles) return;
            //draw small dots for each path point (not waypoint)
            Handles.color = script.color2;
            Vector3[] pathPoints = script.pathPoints;
            
            for (int i = 0; i < pathPoints.Length; i++)
            {
                #if UNITY_5_6_OR_NEWER
                Handles.SphereHandleCap(0, pathPoints[i], Quaternion.identity, 
                Mathf.Clamp((HandleUtility.GetHandleSize(pathPoints[i]) * 0.12f), 0, 0.25f), EventType.Repaint);
                #else
                Handles.SphereCap(0, pathPoints[i], Quaternion.identity, 
                Mathf.Clamp((HandleUtility.GetHandleSize(pathPoints[i]) * 0.12f), 0, 0.25f));
                #endif
            }

                
            //waypoint direction handles drawing
            if(!script.drawDirection) return;
            float lerpVal = 0f;
            
            //create list of path segments (list of Vector3 list)
            List<List<Vector3>> segments = new List<List<Vector3>>();
            int curIndex = 0;
            
            for(int i = 0; i < script.bPoints.Count - 1; i++)
            {
                //loop over path points to find single segments
                segments.Add(new List<Vector3>());
                for(int j = curIndex; j < pathPoints.Length; j++)
                {
                    //the segment ends here, continue with new segment
                    //we are checking for the exact path point, because for bezier paths
                    //path points are exactly located on waypoint positions in the editor
                    if(pathPoints[j] == script.bPoints[i+1].wp.position)
                    {
                        curIndex = j;
                        break;
                    }
                    
                    //add path point to current segment
                    segments[i].Add(pathPoints[j]);
                }
            }
            
            //loop over segments
            for(int i = 0; i < segments.Count; i++)
            {   
                //loop over single positions on the segment
                for(int j = 0; j < segments[i].Count; j++)
                {
                    //get current lerp value for interpolating rotation
                    //draw arrow handle on current position with interpolated rotation
                    size = Mathf.Clamp(HandleUtility.GetHandleSize(segments[i][j]) * 0.4f, 0, 1.2f);
                    lerpVal = j / (float)segments[i].Count;

                    #if UNITY_5_6_OR_NEWER
                    Handles.ArrowHandleCap(0, segments[i][j], Quaternion.Lerp(script.bPoints[i].wp.rotation, script.bPoints[i + 1].wp.rotation, lerpVal), size, EventType.Repaint);
                    #else
                    Handles.ArrowCap( 0, segments[i][j], Quaternion.Lerp(script.bPoints[i].wp.rotation, script.bPoints[i+1].wp.rotation, lerpVal), size);
                    #endif
                }
            }
        }


        //repositions the opposite control point if one changes
        private void PositionOpposite(BezierPoint point, bool isLeft, Vector3 newPos)
        {
            Vector3 pos = point.wp.position;
            Vector3 toParent = pos - newPos;
            int inIndex = isLeft ? 0 : 1;
            int outIndex = inIndex == 0 ? 1 : 0;

            //because the last waypoint has a control point at the waypoint origin,
            //below we check against a Vector3.zero value and ignore that for the opposite
            toParent.Normalize();

            point.cp[inIndex].position = newPos;

            if (toParent != Vector3.zero && script.connectHandles)
            {
                //received the right handle, manipulating the left
                float magnitude = (pos - point.cp[outIndex].position).magnitude;
                point.cp[outIndex].position = pos + toParent * magnitude;
            }
        }


        private void DrawPathOptions()
        {
            editOption = (PathModifierOption)EditorGUILayout.EnumPopup(editOption);

            switch (editOption)
            {
                case PathModifierOption.PlaceToGround:
                    foreach (BezierPoint bp in script.bPoints)
                    {
                        //define ray to cast downwards waypoint position
                        Ray ray = new Ray(bp.wp.position + new Vector3(0, 2f, 0), -Vector3.up);
                        Undo.RecordObject(bp.wp, "Place To Ground");

                        RaycastHit hit;
                        //cast ray against ground, if it hit:
                        if (Physics.Raycast(ray, out hit, 100))
                        {
                            //position waypoint to hit point
                            bp.wp.position = hit.point;
                        }

                        //also try to raycast against 2D colliders
                        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, -Vector2.up, 100);
                        if (hit2D)
                        {
                            bp.wp.position = new Vector3(hit2D.point.x, hit2D.point.y, bp.wp.position.z);
                        }
                    }
                    break;

                case PathModifierOption.InvertDirection:
                    //does not do anything actually
                    Undo.RecordObject(script, "Invert Direction");

                    //to reverse the whole path we need to know where the waypoints were before
                    //for this purpose a new copy must be created
                    List<List<Vector3>> waypointCopy = new List<List<Vector3>>();
                    for (int i = 0; i < script.bPoints.Count; i++)
                    {
                        BezierPoint curPoint = script.bPoints[i];
                        waypointCopy.Add(new List<Vector3>() { curPoint.wp.position, curPoint.cp[0].position, curPoint.cp[1].position });
                    }

                    //reverse order based on the old list
                    for (int i = 0; i < script.bPoints.Count; i++)
                    {
                        BezierPoint curPoint = script.bPoints[i];
                        curPoint.wp.position = waypointCopy[waypointCopy.Count - 1 - i][0];
                        curPoint.cp[0].position = waypointCopy[waypointCopy.Count - 1 - i][2];
                        curPoint.cp[1].position = waypointCopy[waypointCopy.Count - 1 - i][1];
                    }

                    break;

                case PathModifierOption.RotateWaypointsToPath:
                    Undo.RecordObject(script, "Rotate Waypoints");

                    //orient waypoints to the path in forward direction
                    for (int i = 0; i < script.bPoints.Count; i++)
                    {
                        //save child rotations before applying waypoint rotation
                        Vector3[] globalPos = new Vector3[script.bPoints[i].wp.childCount];
                        for (int j = 0; j < globalPos.Length; j++)
                            globalPos[j] = script.bPoints[i].wp.GetChild(j).position;

                        if (i == script.bPoints.Count - 1)
                            script.bPoints[i].wp.rotation = script.bPoints[i - 1].wp.rotation;
                        else
                            script.bPoints[i].wp.LookAt(script.bPoints[i + 1].wp);

                        //restore previous location after rotation
                        for (int j = 0; j < globalPos.Length; j++)
                            script.bPoints[i].wp.GetChild(j).position = globalPos[j];
                    }
                    break;

                case PathModifierOption.RenameWaypoints:
                    //disabled because of a Unity bug that crashes the editor
                    //this is taken directly from the docs, thank you Unity.
                    //http://docs.unity3d.com/ScriptReference/Undo.RegisterCompleteObjectUndo.html
                    //Undo.RegisterCompleteObjectUndo(waypoints[0].gameObject, "Rename Waypoints");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Skip Custom Names?");
                    script.skipCustomNames = EditorGUILayout.Toggle(script.skipCustomNames, GUILayout.Width(20));
                    EditorGUILayout.EndHorizontal();

                    if (!GUILayout.Button("Rename Now"))
                    {
                        return;
                    }

                    RenameWaypoints(script.skipCustomNames);
                    break;

                case PathModifierOption.UpdateFromChildren:
                    Undo.RecordObject(script, "Update Path From Children");
                    script.Create();
                    SceneView.RepaintAll();
                    break;

                case PathModifierOption.ReplaceWaypointObject:
                    //draw object field for new waypoint object
                    script.replaceObject = (GameObject)EditorGUILayout.ObjectField("Replace Object", script.replaceObject, typeof(GameObject), true);

                    //replace all waypoints with the prefab
                    if (!GUILayout.Button("Replace Now")) return;
                    else if (script.replaceObject == null)
                    {
                        Debug.LogWarning("No replace object set. Cancelling.");
                        return;
                    }

                    //Undo.RecordObject(script, "Replace Object");
                    Undo.RegisterFullObjectHierarchyUndo(script.transform, "Replace Object");

                    //old waypoints to remove after replace
                    List<GameObject> toRemove = new List<GameObject>();
                    //loop through waypoint list
                    for (int i = 0; i < script.bPoints.Count; i++)
                    {
                        //get current bezier point at index position
                        BezierPoint point = script.bPoints[i];
                        Transform curWP = point.wp;
                        //instantiate new waypoint at old position
                        Transform newCur = ((GameObject)Instantiate(script.replaceObject, curWP.position, Quaternion.identity)).transform;
                        //Undo.RegisterCreatedObjectUndo(newCur.gameObject, "Replace Object");

                        //parent control points to the new bezier point
                        Undo.SetTransformParent(point.cp[0], newCur, "Replace Object");
                        Undo.SetTransformParent(point.cp[1], newCur, "Replace Object");
                        //parent new waypoint to this path
                        newCur.parent = point.wp.parent;

                        //replace old waypoint at index
                        script.bPoints[i].wp = newCur;
                        //indicate to remove old waypoint
                        toRemove.Add(curWP.gameObject);
                    }

                    //destroy old waypoint object
                    for (int i = 0; i < toRemove.Count; i++)
                        Undo.DestroyObjectImmediate(toRemove[i]);

                    break;
            }

            editOption = PathModifierOption.SelectModifier;
        }


        private void RenameWaypoints(bool skipCustom)
        {
            string wpName = string.Empty;
            string[] nameSplit;

            for (int i = 0; i < script.bPoints.Count; i++)
            {
                //cache name and split into strings
                wpName = script.bPoints[i].wp.name;
                nameSplit = wpName.Split(' ');

                //ignore custom names and just rename
                if (!script.skipCustomNames)
                    wpName = "Waypoint " + i;
                else if (nameSplit.Length == 2 && nameSplit[0] == "Waypoint")
                {
                    //try parsing the current index and rename,
                    //not ignoring custom names here
                    int index;
                    if (int.TryParse(nameSplit[1], out index))
                    {
                        wpName = nameSplit[0] + " " + i;
                    }
                }

                //set the desired index or leave it
                script.bPoints[i].wp.name = wpName;
            }
        }
    }
}