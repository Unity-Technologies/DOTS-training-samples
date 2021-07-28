using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using UnityPlane = UnityEngine.Plane;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;

using src.Components;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FireStarterSystem: SystemBase
    {
        private Random random;

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
                var hor_from = configValues.GetCellWorldPosition3D(i, 0) + new float3(-offset, planeHeight, 0f);
                var hor_to = configValues.GetCellWorldPosition3D(i, columns) + new float3(offset, planeHeight, 0f);
                UnityEngine.Debug.DrawLine(hor_from, hor_to, UnityEngine.Color.grey, 0);
            }

            for (int j = 0; j <= columns; ++j)
            { 
                var ver_from = configValues.GetCellWorldPosition3D(0, j) + new float3(0f, planeHeight, -offset);
                var ver_to = configValues.GetCellWorldPosition3D(rows, j) + new float3(0f, planeHeight, offset);
                UnityEngine.Debug.DrawLine(ver_from, ver_to, UnityEngine.Color.grey, 0);
            }
#endif                    

            if (UnityInput.GetMouseButtonDown(0))
            {              
                var camera = this.GetSingleton<GameObjectRefs>().Camera;
                if (camera)
                {
                    var screenPointToRay = camera.ScreenPointToRay(UnityInput.mousePosition);                    
                    var plane = new UnityPlane(UnityEngine.Vector3.up, -planeHeight);
                    plane.Raycast(screenPointToRay, out float enter);
                    
                    var clickPosition = screenPointToRay.GetPoint(enter);
                    var rowCol = configValues.GetCellRowCol(new float3(clickPosition.x, clickPosition.y, clickPosition.z));
                    var row = rowCol.x;
                    var col = rowCol.y;
                    if (row >= 0 && row < rows && col >= 0 && col < columns)
                    {
                        var temperatureEntity = GetSingletonEntity<Temperature>();
                        var temperatureBuffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);

                        var temperature = temperatureBuffer[row * columns + col];
                        temperature.Intensity = random.NextFloat(configValues.Flashpoint, 1f);
                        temperatureBuffer[row * columns + col] = temperature;
                    }
                }
            }
        }
    }
}