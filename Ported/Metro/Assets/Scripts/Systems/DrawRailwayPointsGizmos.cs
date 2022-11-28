using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial class DrawRailwayPointsGizmos : SystemBase
    {
        private Color c = Color.green;

        protected override void OnUpdate()
        {
            foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<RailwayPoint>())
            {
                Matrix4x4 m = transform.WorldMatrix;
                var point1 = m.MultiplyPoint(new Vector3(-0.15f, -0.15f, 0.15f));
                var point2 = m.MultiplyPoint(new Vector3(0.15f, -0.15f, 0.15f));
                var point3 = m.MultiplyPoint(new Vector3(0.15f, -0.15f, -0.15f));
                var point4 = m.MultiplyPoint(new Vector3(-0.15f, -0.15f, -0.15f));
                var point5 = m.MultiplyPoint(new Vector3(-0.15f, 0.15f, 0.15f));
                var point6 = m.MultiplyPoint(new Vector3(0.15f, 0.15f, 0.15f));
                var point7 = m.MultiplyPoint(new Vector3(0.15f, 0.15f, -0.15f));
                var point8 = m.MultiplyPoint(new Vector3(-0.15f, 0.15f, -0.15f));

                Debug.DrawLine(point1, point2, c);
                Debug.DrawLine(point2, point3, c);
                Debug.DrawLine(point3, point4, c);
                Debug.DrawLine(point4, point1, c);

                Debug.DrawLine(point5, point6, c);
                Debug.DrawLine(point6, point7, c);
                Debug.DrawLine(point7, point8, c);
                Debug.DrawLine(point8, point5, c);

                Debug.DrawLine(point1, point5, c);
                Debug.DrawLine(point2, point6, c);
                Debug.DrawLine(point3, point7, c);
                Debug.DrawLine(point4, point8, c);
            }
        }
    }
}