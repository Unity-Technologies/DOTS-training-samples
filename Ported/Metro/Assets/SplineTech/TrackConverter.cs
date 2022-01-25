using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Onboarding.BezierPath;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TrackConverter : MonoBehaviour
{
    public GameObject rootToConvert;
    public GameObject origin;
    public PathBaker pathBaker;
    public float railsDistance = 8;
    public float bendFactor = 4.0f;
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(TrackConverter))]
public class TrackConverterEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var converter = target as TrackConverter;
        var outputRoot = converter.gameObject;
        var outputRootOrigin = outputRoot.transform.position;
        
        base.OnInspectorGUI();
        if (GUILayout.Button("Convert Track"))
        {
            // Delete all children
            while (outputRoot.transform.childCount!=0)
                GameObject.DestroyImmediate(outputRoot.transform.GetChild(0).gameObject);
            
            // Fetch original points
            GameObject[] children = new GameObject[converter.rootToConvert.transform.childCount];
            for (int i = 0; i < children.Length; ++i)
                children[i] = converter.rootToConvert.transform.GetChild(i).gameObject;

            Debug.Log($"Found {children.Length} children");
            
            // First pass, we create the control points
            List<Vector3> controlPoints = new List<Vector3>();
            List<Vector3> loopControlPoints = new List<Vector3>();
            var originInWorldSpace = converter.origin.transform.position;
            Vector3 lastTranslation = Vector3.zero;
            for (int i = 0; i < children.Length-1; ++i)
            {
                var start = children[i];
                var end = children[i+1];
                var startPos = start.transform.position - originInWorldSpace;
                var endPos = end.transform.position - originInWorldSpace;
                var tan1 = Vector3.Lerp(startPos, endPos, 1 / 3.0f);
                var tan2 = Vector3.Lerp(startPos, endPos, 2 / 3.0f);
                if (i==0)
                    controlPoints.Add(startPos);
                controlPoints.Add(tan1);
                controlPoints.Add(tan2);
                controlPoints.Add(endPos);
                
                var direction = (endPos - startPos).normalized;
                var translation = UnityEngine.Quaternion.Euler(0,-90,0) * direction;
                
                var averageTranslation =  (i != 0) ? (translation + lastTranslation) * 0.5f : translation;
                lastTranslation = translation;

                var translatedStart = startPos + averageTranslation * converter.railsDistance;
                var translatedEnd = endPos + averageTranslation * converter.railsDistance;
               
                loopControlPoints.Add(translatedStart);
                loopControlPoints.Add(Vector3.zero);
                loopControlPoints.Add(Vector3.zero);
                if (i==children.Length-2)
                    loopControlPoints.Add(translatedEnd);
            }

            // Add end bending
            int endBendStartIndex = controlPoints.Count - 1;
            AddBend();
            int endBendEndIndex = controlPoints.Count - 1;
            
            loopControlPoints.Reverse();
            // fix tangents
            for (int i = 0; i < loopControlPoints.Count - 3; i+=3)
            {
                var start = loopControlPoints[i];
                var end = loopControlPoints[i+3];
                loopControlPoints[i+1] = Vector3.Lerp(start, end, 1 / 3.0f);
                loopControlPoints[i+2] = Vector3.Lerp(start, end, 2 / 3.0f);
            }
            // Kill first point, as it's already output from the bend code
            loopControlPoints.RemoveAt(0);
            controlPoints.AddRange(loopControlPoints);
            
            // Add start bending
            int startBendStartIndex = controlPoints.Count - 1;
            AddBend();
            int startBendEndIndex = controlPoints.Count - 1;
            
            // Smooth out all tangents
            SmoothNormals(3, endBendStartIndex);
            SmoothNormals(endBendEndIndex + 3, startBendStartIndex);

            // Second pass, create all GOs
            List<GameObject> gos = new List<GameObject>();
            for (int i=0;i<controlPoints.Count;++i)
            {
                var go = new GameObject($"Spline_{i}_{i % 4}");
                go.transform.position = outputRootOrigin + controlPoints[i];
                go.transform.SetParent(outputRoot.transform);
                gos.Add(go);
            }

            // Update baker
            converter.pathBaker.m_ControlPoints = gos.ToArray();

            void SmoothNormals(int start, int end)
            {
                for (int i = start; i < end; i+=3)
                {
                    var currPt = controlPoints[i];
                    var tan1 = controlPoints[i-1] - currPt;
                    var negtan2 = currPt - controlPoints[i+1];
                    var newTan = (tan1 + negtan2) * 0.5f;

                    controlPoints[i - 1] = currPt + newTan;
                    controlPoints[i + 1] = currPt - newTan;
                }
            }

            void AddBend()
            {
                var lastSegmentStart = controlPoints[controlPoints.Count - 2];
                var lastSegmentEnd = controlPoints[controlPoints.Count - 1];
                var lastSegmentDir = (lastSegmentEnd - lastSegmentStart).normalized;
                var translation = UnityEngine.Quaternion.Euler(0, -90, 0) * lastSegmentDir;

                float rd = converter.railsDistance;
                float hrd = converter.railsDistance * 0.5f;
                
                float bendFactor = converter.bendFactor;
                float tangentOffset = hrd * bendFactor;
                
                // Add tangent out towards hcircle top
                controlPoints.Add(lastSegmentEnd + tangentOffset * lastSegmentDir);
                // Add tangent in at hcircle top
                controlPoints.Add(lastSegmentEnd + hrd * lastSegmentDir + (hrd - tangentOffset) * translation);
                // Add top for hcircle
                controlPoints.Add(lastSegmentEnd + hrd * lastSegmentDir + hrd * translation );
                
                // Add tangent out at hcircle top
                controlPoints.Add(lastSegmentEnd + hrd * lastSegmentDir + (hrd + tangentOffset) * translation);
                // Add tangent out at hcircle end
                controlPoints.Add(lastSegmentEnd + tangentOffset * lastSegmentDir + rd * translation);
                // Add hcircle end
                controlPoints.Add(lastSegmentEnd + rd * translation);
            }
        }
    }
}
#endif

