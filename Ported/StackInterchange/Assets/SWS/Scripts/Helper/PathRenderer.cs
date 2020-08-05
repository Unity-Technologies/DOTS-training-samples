/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SWS
{
    /// <summary>
    /// Uses Unity's LineRenderer component to render paths.
    /// <summary>
    [RequireComponent(typeof(LineRenderer))]
    public class PathRenderer : MonoBehaviour
    {
        /// <summary>
        /// Whether LineRenderer positions should be updated per frame.
        /// <summary>
        public bool onUpdate = false;
        /// <summary>
        /// Spacing between LineRenderer positions on the path.
        /// <summary>
        public float spacing = 0.05f;

        private PathManager path;
        private LineRenderer line;
        private Vector3[] points;


        //get references and start rendering
        void Start()
        {
            line = GetComponent<LineRenderer>();
            path = GetComponent<PathManager>();
            if (path) StartCoroutine("StartRenderer");
        }


        //invokes the position update loop
        IEnumerator StartRenderer()
        {
            Render();

            if (onUpdate)
            {
                while (true)
                {
                    yield return null;
                    Render();
                }
            }
        }


        //updates LineRenderer positions
        void Render()
        {
            //avoid freeze in while loop with spacing set to zero
            spacing = Mathf.Clamp01(spacing);
            if (spacing == 0) spacing = 0.05f;

            //get path points of the path component
            List<Vector3> list = new List<Vector3>();
            list.AddRange(path.GetPathPoints());

            //differ between linear and curved paths
            if (path.drawCurved)
            {
                //on curved paths, the first and last waypoint
                //has to exist twice due to tween library calculations
                list.Insert(0, list[0]);
                list.Add(list[list.Count - 1]);
                points = list.ToArray();
                DrawCurved();
            }
            else
            {
                points = list.ToArray();
                DrawLinear();
            }
        }


        //draw curved path positions
        void DrawCurved()
        {
            //set initial LineRenderer size based on spacing
            int size = Mathf.RoundToInt(1f / spacing) + 1;
            #if UNITY_5_5_OR_NEWER
            line.positionCount = size;
            #else
            line.SetVertexCount(size);
            #endif
            float t = 0f;
            int i = 0;

            //loop over positions and set calculated path point
            while (i < size)
            {
                line.SetPosition(i, WaypointManager.GetPoint(points, t));
                t += spacing;
                i++;
            }
        }


        //draw linear path positions
        void DrawLinear()
        {
            //set initial size based on waypoint count
             #if UNITY_5_5_OR_NEWER
            line.positionCount = points.Length;
            #else
            line.SetVertexCount(points.Length);
            #endif
            float t = 0f;
            int i = 0;

            //loop over positions and set waypoint position
            while (i < points.Length)
            {
                line.SetPosition(i, points[i]);
                t += spacing;
                i++;
            }
        }
    }
}
