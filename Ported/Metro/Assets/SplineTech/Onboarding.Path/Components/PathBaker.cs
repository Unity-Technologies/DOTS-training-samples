using UnityEngine;

namespace Onboarding.BezierPath
{
    public class PathBaker : MonoBehaviour
    {
        public PathData         m_PathData;
        public float            m_SamplingDistance = 0.10f;
        public float            m_ErrorThreshold = 0.10f;
        public GameObject[]     m_ControlPoints;
        public Color            m_SplineColor = Color.red;
        public Color            m_TangentColor = Color.green;

        public bool             m_DisplayTangents = true;

        #if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            int splineCount = (m_ControlPoints.Length - 1) / 3;
            for (int i = 0; i < splineCount; ++i)
            {
                int offset = i * 3;
                Vector3 start = m_ControlPoints[offset].transform.position;
                Vector3 end = m_ControlPoints[offset + 3].transform.position;
                Vector3 startTangent = m_ControlPoints[offset + 1].transform.position;
                Vector3 endTangent = m_ControlPoints[offset + 2].transform.position;


                UnityEditor.Handles.DrawBezier(start, end, startTangent, endTangent, m_SplineColor, Texture2D.whiteTexture, 4.0f);

                if (m_DisplayTangents)
                {
                    UnityEditor.Handles.color = m_TangentColor;
                    UnityEditor.Handles.DrawLine(start, startTangent, 1);
                    UnityEditor.Handles.DrawLine(end, endTangent, 1);
                }
            }
        }
        #endif
    }
}