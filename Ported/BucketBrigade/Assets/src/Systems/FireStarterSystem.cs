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
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfig>();
            RequireSingletonForUpdate<Temperature>();
        }

        protected override void OnUpdate()
        {
            var configEntity = GetSingletonEntity<FireSimConfig>();
            var configValues = GetComponent<FireSimConfigValues>(configEntity);

            // TODO: grid plane should be at the quad where the grid is projected. For now using a 
            // hardcoded scale and assuming the plane is on the XZ quadrant with origin at (0, 0, 0)

            var cellSize = configValues.CellSize * 100;
            var rows = configValues.Rows;
            var columns = configValues.Columns;

            for (int i = 0; i <= rows; ++i)
                for (int j = 0; j <= columns; ++j)
                {
                    var cellWorldPos = GetCellWorldPosition(i, j, cellSize, default);
                    var offset = cellSize * 0.5f;
                    var hor_from = cellWorldPos + new float3(-offset, .5f, 0f);
                    var hor_to = cellWorldPos + new float3(offset, .5f, 0f);
                    var ver_from = cellWorldPos + new float3(0f, .5f, -offset);
                    var ver_to = cellWorldPos + new float3(0f, .5f, offset);
                    UnityEngine.Debug.DrawLine(hor_from, hor_to, UnityEngine.Color.cyan, 0);
                    UnityEngine.Debug.DrawLine(ver_from, ver_to, UnityEngine.Color.cyan, 0);
                }
                    

            if (UnityInput.GetMouseButtonDown(0))
            {
                var camera = this.GetSingleton<GameObjectRefs>().Camera;
                if (camera)
                {
                    var screenPointToRay = camera.ScreenPointToRay(UnityInput.mousePosition);                    
                    var plane = new UnityPlane(UnityEngine.Vector3.up, 0);
                    plane.Raycast(screenPointToRay, out float distance);
                    var position = screenPointToRay.direction * distance;

                    // TODO: should take into account origin, rotation and scale of the quad where the grid is projected...
                    
                    var (row, col) = GetCellRowCol(new float3(position.x, position.y, position.z), cellSize, default);
                    if (row > 0 && row < rows && col > 0 && col < columns)
                    {
                        var temperatureEntity = GetSingletonEntity<Temperature>();
                        var temperatureBuffer = EntityManager.GetBuffer<Temperature>(temperatureEntity);

                        var random = new Random((uint)Time.ElapsedTime);
                        var temperature = temperatureBuffer[row * columns + col];
                        temperature.Intensity = random.NextFloat(configValues.Flashpoint, 1f);
                        temperatureBuffer[row * columns + col] = temperature;
                    }
                }
            }
        }

        private float3 GetCellWorldPosition(int row, int col, float cellSize, float3 origin)
            => origin + new float3(col, 0f, row) * cellSize;

        private (int row, int col) GetCellRowCol(float3 worldPosition, float cellSize, float3 origin)
        {
            var localPosition = worldPosition - origin;
            
            // Convert to grid position
            var row = (int)(localPosition.z / cellSize);
            var col = (int)(localPosition.x / cellSize);
            return (row, col);
        }
    }
}