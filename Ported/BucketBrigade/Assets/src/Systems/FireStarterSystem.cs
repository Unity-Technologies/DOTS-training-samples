using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using UnityPlane = UnityEngine.Plane;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

using src.Components;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FireStarterSystem: SystemBase
    {
        private Random random;

        int m_LastIgnitedCell, m_LastExtinguishedCell;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfigValues>();
            RequireSingletonForUpdate<Temperature>();

            random = new Random((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        }

        protected override void OnUpdate()
        {
            var configValues = GetSingleton<FireSimConfigValues>();

            // TODO: grid plane should be at the quad where the grid is projected. For now using a 
            // hardcoded scale and assuming the plane is on the XZ quadrant with bottom left at (0, 0, 0)

            var cellSize = configValues.CellSize;
            var rows = configValues.Rows;
            var columns = configValues.Columns;
            var planeHeight = 0f; // this must move up ro down depending on the height of the green ground above 0

#if UNITY_EDITOR           
            var offset = 0f;
            for (int i = 0; i <= rows; ++i)
            {
                var hor_from = configValues.GetPosition3DOfCellColRow(0, i) + new float3(-offset, planeHeight, 0f);
                var hor_to = configValues.GetPosition3DOfCellColRow(columns, i) + new float3(offset, planeHeight, 0f);
                UnityEngine.Debug.DrawLine(hor_from, hor_to, UnityEngine.Color.grey, 0);
            }

            for (int j = 0; j <= columns; ++j)
            { 
                var ver_from = configValues.GetPosition3DOfCellColRow(j, 0) + new float3(0f, planeHeight, -offset);
                var ver_to = configValues.GetPosition3DOfCellColRow(j, rows) + new float3(0f, planeHeight, offset);
                UnityEngine.Debug.DrawLine(ver_from, ver_to, UnityEngine.Color.grey, 0);
            }
#endif

            var createFire = UnityInput.GetMouseButton(0);
            var extinguishFire = UnityInput.GetMouseButton(1);
     
            if (createFire || extinguishFire)
            {              
                var camera = this.GetSingleton<GameObjectRefs>().Camera;
                if (camera)
                {
                    var screenPointToRay = camera.ScreenPointToRay(UnityInput.mousePosition);                    
                    var plane = new UnityPlane(UnityEngine.Vector3.up, -planeHeight);
                    plane.Raycast(screenPointToRay, out float enter);
                    
                    var clickPosition = screenPointToRay.GetPoint(enter);
                    var cellId = configValues.GetCellIdOfPosition3D(new float3(clickPosition.x, clickPosition.y, clickPosition.z));
                    if (configValues.IsValidCell(cellId))
                    {
                        var temperatureEntity = GetSingletonEntity<Temperature>();
                        var temperatureBuffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);

                        ref var temperature = ref temperatureBuffer.ElementAt(cellId);

                        if (createFire)
                        {
                            if (m_LastIgnitedCell != cellId)
                            {
                                temperature.Intensity = random.NextFloat(configValues.Flashpoint, 1f);
                                m_LastIgnitedCell = cellId;
                            }
                        }
                        else m_LastIgnitedCell = -1;

                        if (extinguishFire)
                        {
                            if (m_LastExtinguishedCell != cellId)
                            {
                                temperature.Intensity = 0;
                                m_LastExtinguishedCell = cellId;
                            }
                        }
                        else m_LastExtinguishedCell = -1;
                    }
                }
            }
        }
    }
}