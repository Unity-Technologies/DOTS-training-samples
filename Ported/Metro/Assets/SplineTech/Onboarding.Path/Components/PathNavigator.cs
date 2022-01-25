using System;
using UnityEngine;

namespace Onboarding.BezierPath
{
    public class PathNavigator : MonoBehaviour
    {
        public enum LoopMode
        {
            CLAMP,
            WRAP,
            PINGPONG
        }

        public PathData                     PathData;
        public float                        Speed = 1;            // meters per second
        public float                        InitialDistance = 0;  // meters since the origin
        public LoopMode                     Looping;

        private float                       NavigatedDistance;

        private PathController.LookupCache  Cache;

        public void OnEnable()
        {
            NavigatedDistance = InitialDistance;
            Cache = new PathController.LookupCache();
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            PathController.InterpolatePositionAndDirection(PathData, Cache, NavigatedDistance, out var pos, out var dir);
            transform.position = pos;
            transform.LookAt(pos + dir);
        }

        private float totalTime = 0;
        public void Update()
        {
            NavigatedDistance += Speed * Time.deltaTime;
            if (Looping == LoopMode.CLAMP)
            {
                NavigatedDistance = Mathf.Clamp(NavigatedDistance, 0, PathData.PathLength);
            }
            else if (Looping == LoopMode.WRAP)
            {
                if (NavigatedDistance > PathData.PathLength)
                {
                    NavigatedDistance = NavigatedDistance % PathData.PathLength;
                    PathController.ResetCache(Cache);
                }
                else if (NavigatedDistance < 0)
                {
                    NavigatedDistance = PathData.PathLength + (NavigatedDistance % PathData.PathLength);
                    PathController.ResetCache(Cache);
                }
            }
            else if (Looping == LoopMode.PINGPONG)
            {
                if (NavigatedDistance > PathData.PathLength || NavigatedDistance < 0)
                {
                    Speed = -Speed;
                    NavigatedDistance = Mathf.Clamp(NavigatedDistance, 0, PathData.PathLength);
                }
            }

            UpdatePosition();

            totalTime += Time.deltaTime;
            if (totalTime > 1)
            {
                totalTime -= 1;
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = this.transform.position;
            }
        }
    }
}