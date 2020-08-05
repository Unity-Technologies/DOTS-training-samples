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
    /// Waypoint and path creation editor.
    /// <summary>
    [CustomEditor(typeof(WaypointManager))]
    public class WaypointEditor : Editor
    {
        //manager reference
        private WaypointManager script;
		//new path name
		private string pathName = "";
		//enables 2D mode placement (auto-detection)
		private bool mode2D = false;

		//if we are placing new waypoints in editor
        private static bool placing = false;
        //new path gameobject
        private static GameObject path;
        //Path Manager reference for editing waypoints
        private static PathManager pathMan;
        //temporary list for editor created waypoints in a path
        private static List<GameObject> wpList = new List<GameObject>();   

        //path type selection enum
        private enum PathType
        {
            standard,
            bezier
        }
        private PathType pathType = PathType.standard;


        public void OnSceneGUI()
        {
            //with creation mode enabled, place new waypoints on keypress
            if (Event.current.type != EventType.KeyDown || !placing) return;

            //scene view camera placement
            if (Event.current.keyCode == KeyCode.C)
            {
                Event.current.Use();
                Vector3 camPos = GetSceneView().camera.transform.position;

                //place a waypoint at the camera
                if (pathMan is BezierPathManager)
                    PlaceBezierPoint(camPos);
                else
                    PlaceWaypoint(camPos);

            }
            else if (Event.current.keyCode == script.placementKey)
            {
                //cast a ray against mouse position
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;

                //2d placement
                if (mode2D)
                {
                    Event.current.Use();
                    //convert screen to 2d position
                    Vector3 pos2D = worldRay.origin;
                    pos2D.z = 0;

                    //place a waypoint at clicked point
                    if (pathMan is BezierPathManager)
                        PlaceBezierPoint(pos2D);
                    else
                        PlaceWaypoint(pos2D);
                }
                //3d placement
                else
                {
                    if (Physics.Raycast(worldRay, out hitInfo))
                    {
                        Event.current.Use();

                        //place a waypoint at clicked point
                        if (pathMan is BezierPathManager)
                            PlaceBezierPoint(hitInfo.point);
                        else
                            PlaceWaypoint(hitInfo.point);
                    }
                    else
                    {
                        Debug.LogWarning("Waypoint Manager: 3D Mode. Trying to place a waypoint but couldn't "
                                         + "find valid target. Have you clicked on a collider?");
                    }
                }
            }
        }


        public override void OnInspectorGUI()
        {
            //show default variables of manager
            DrawDefaultInspector();
            //get manager reference
            script = (WaypointManager)target;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //get sceneview to auto-detect 2D mode
            SceneView view = GetSceneView();
            mode2D = view.in2DMode;

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            //draw path text label
            GUILayout.Label("Enter Path Name: ", GUILayout.Height(15));
            //display text field for creating a path with that name
            pathName = EditorGUILayout.TextField(pathName, GUILayout.Height(15));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            //draw path type selection enum
            GUILayout.Label("Select Path Type: ", GUILayout.Height(15));
            pathType = (PathType)EditorGUILayout.EnumPopup(pathType);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //display label of current mode
            if (mode2D)
                GUILayout.Label("2D Mode Detected.", GUILayout.Height(15));
            else
                GUILayout.Label("3D Mode Detected.", GUILayout.Height(15));
            EditorGUILayout.Space();

            //draw path creation button
            if (!placing && GUILayout.Button("Start Path", GUILayout.Height(40)))
            {
                if (pathName == "")
                {
                    EditorUtility.DisplayDialog("No Path Name", "Please enter a unique name for your path.", "Ok");
                    return;
                }

                if (script.transform.Find(pathName) != null)
                {
                    if(EditorUtility.DisplayDialog("Path Exists Already",
                        "A path with this name exists already.\n\nWould you like to edit it?", "Ok", "Cancel"))
                    {
                        Selection.activeTransform = script.transform.Find(pathName);
                    }
                    return;
                }

                //create a new container transform which will hold all new waypoints
                path = new GameObject(pathName);
                //reset position and parent container gameobject to this manager gameobject
                path.transform.position = script.gameObject.transform.position;
                path.transform.parent = script.gameObject.transform;
                StartPath();

                //we passed all prior checks, toggle waypoint placement
                placing = true;
                //focus sceneview for placement
                view.Focus();
            }

            GUI.backgroundColor = Color.yellow;

            //finish path button
            if (placing && GUILayout.Button("Finish Editing", GUILayout.Height(40)))
            {
				FinishPath();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.Space();
            //draw instructions
            GUILayout.TextArea("Hint:\nEnter a unique name for your path, "
                            + "then press 'Start Path' to begin placement mode. Press '" + script.placementKey
                            + "' on your keyboard to place new waypoints in the Scene view. In 3D Mode "
                            + "you have to place waypoints onto game objects with colliders. You can "
                            + "also place waypoints at the current scene view camera position by pressing '"
                            + script.viewPlacementKey + "'.\n\nPress 'Finish Editing' to end your path.");
        }


        //when losing editor focus
        void OnDisable()
        {
			FinishPath();
        }


        //differ between path selection
        void StartPath()
        {
            switch (pathType)
            {
                case PathType.standard:
                    pathMan = path.AddComponent<PathManager>();
                    pathMan.waypoints = new Transform[0];
                    break;
                case PathType.bezier:
                    pathMan = path.AddComponent<BezierPathManager>();
                    BezierPathManager thisPath = pathMan as BezierPathManager;
                    thisPath.showHandles = true;
                    thisPath.bPoints = new List<BezierPoint>();
                    break;
            }
        }


		public static void ContinuePath(PathManager p)
		{
			path = p.gameObject;
			pathMan = p;
			placing = true;

            wpList.Clear();
            if(p is BezierPathManager)
            {
                for (int i = 0; i < (p as BezierPathManager).bPoints.Count; i++)
                    wpList.Add((p as BezierPathManager).bPoints[i].wp.gameObject);
            }
            else
            {
                for (int i = 0; i < p.waypoints.Length; i++)
                    wpList.Add(p.waypoints[i].gameObject);
            }

            GetSceneView().Focus();
        }


        //path manager placement
        void PlaceWaypoint(Vector3 placePos)
        {
            //instantiate waypoint gameobject
            GameObject wayp = new GameObject("Waypoint");

            //with every new waypoint, our waypoints array should increase by 1
            //but arrays gets erased on resize, so we use a classical rule of three
            Transform[] wpCache = new Transform[pathMan.waypoints.Length];
            System.Array.Copy(pathMan.waypoints, wpCache, pathMan.waypoints.Length);

            pathMan.waypoints = new Transform[pathMan.waypoints.Length + 1];
            System.Array.Copy(wpCache, pathMan.waypoints, wpCache.Length);
            pathMan.waypoints[pathMan.waypoints.Length - 1] = wayp.transform;

            //this is executed on placement of the first waypoint:
            //we position our path container transform to the first waypoint position,
            //so the transform (and grab/rotate/scale handles) aren't out of sight
            if (wpList.Count == 0)
                pathMan.transform.position = placePos;

            //position current waypoint at clicked position in scene view
            if (mode2D) placePos.z = 0f;
            wayp.transform.position = placePos;
            wayp.transform.rotation = Quaternion.Euler(-90, 0, 0);
            //parent it to the defined path 
            wayp.transform.parent = pathMan.transform;
            //add waypoint to temporary list
            wpList.Add(wayp);
            //rename waypoint to match the list count
            wayp.name = "Waypoint " + (wpList.Count - 1);
        }


        //bezier path placement
        void PlaceBezierPoint(Vector3 placePos)
        {
            //create new bezier point property class
            BezierPoint newPoint = new BezierPoint();

            //instantiate waypoint gameobject
            Transform wayp = new GameObject("Waypoint").transform;
            //assign waypoint to the class
            newPoint.wp = wayp;

            //same as above
            if (wpList.Count == 0)
                pathMan.transform.position = placePos;

            //position current waypoint at clicked position in scene view
            if (mode2D) placePos.z = 0f;
            wayp.position = placePos;
            wayp.transform.rotation = Quaternion.Euler(-90, 0, 0);
            //parent it to the defined path
            wayp.parent = pathMan.transform;

            BezierPathManager thisPath = pathMan as BezierPathManager;
            //create new array with bezier point handle positions
            Transform left = new GameObject("Left").transform;
            Transform right = new GameObject("Right").transform;
            left.parent = right.parent = wayp;

            //initialize positions and last waypoint
            Vector3 handleOffset = new Vector3(2, 0, 0);
            Vector3 targetDir = Vector3.zero;
            int lastIndex = wpList.Count - 1;

            //position handles to the left/right of the waypoint respectively
            left.position = wayp.position + wayp.rotation * handleOffset;
            right.position = wayp.position + wayp.rotation * -handleOffset;
            newPoint.cp = new[] { left, right };

            //position first handle in direction of the second waypoint
            if (wpList.Count == 1)
            {
                targetDir = (wayp.position - wpList[0].transform.position).normalized;
                thisPath.bPoints[0].cp[1].localPosition = targetDir * 2;
            }
            //always position last handle to look at the previous waypoint 
            else if (wpList.Count >= 1)
            {
                targetDir = (wpList[lastIndex].transform.position - wayp.position);
                wayp.transform.rotation = Quaternion.LookRotation(targetDir) * Quaternion.Euler(0, -90, 0);
            }
            
            //position handle direction to the center of both last and next waypoints
            //takes into account 2D mode
            if (wpList.Count >= 2)
            {
                //get last point and center direction
                BezierPoint lastPoint = thisPath.bPoints[lastIndex];
                targetDir = (wayp.position - wpList[lastIndex].transform.position) +
                                    (wpList[lastIndex - 1].transform.position - wpList[lastIndex].transform.position);

                //rotate to the center 2D/3D
                Quaternion lookRot = Quaternion.LookRotation(targetDir);
                if (mode2D)
                {
                    float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg + 90;
                    lookRot = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                lastPoint.wp.rotation = lookRot;

                //cache handle and get previous of last waypoint
                Vector3 leftPos = lastPoint.cp[0].position;
                Vector3 preLastPos = wpList[lastIndex - 1].transform.position;

                //calculate whether right or left handle distance is greater to last waypoint
                //left handle should point to the last waypoint, so reposition if necessary
                if (Vector3.Distance(leftPos, preLastPos) > Vector3.Distance(lastPoint.cp[1].position, preLastPos))
                {
                    lastPoint.cp[0].position = lastPoint.cp[1].position;
                    lastPoint.cp[1].position = leftPos;
                }
            }

            //add waypoint to the list of waypoints
            thisPath.bPoints.Add(newPoint);
            thisPath.segmentDetail.Add(thisPath.pathDetail);
            //add waypoint to temporary list
            wpList.Add(wayp.gameObject);
            //rename waypoint to match the list count
            wayp.name = "Waypoint " + (wpList.Count - 1);
            //recalculate bezier path
            thisPath.CalculatePath();
        }


		void FinishPath()
		{
			if (!placing) return;

			if (wpList.Count < 2)
			{
				Debug.LogWarning("Not enough waypoints placed. Cancelling.");
				//if we have created a path already, destroy it again
				if (path) DestroyImmediate(path);
			}
			
			//toggle placement off
			placing = false;
			//clear list with temporary waypoint references,
			//we only needed this for getting the waypoint count
			wpList.Clear();
			//reset path name input field
			pathName = "";
			//make the new path the active selection
			Selection.activeGameObject = path;
		}


        /// <summary>
        /// Gets the active SceneView or creates one.
        /// </summary>
        public static SceneView GetSceneView()
        {
            SceneView view = SceneView.lastActiveSceneView;
            if (view == null)
                view = EditorWindow.GetWindow<SceneView>();

            return view;
        }
    }
}
