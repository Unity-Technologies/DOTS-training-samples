using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class Utils
{
    public static bool IsTopOfStack(ResourceGridParams resGridParams, DynamicBuffer<StackHeightParams> stackHeights,
                                int gridX, int gridY, int stackIndex, bool stacked)
    {
        if (stacked)
        {
            int index = resGridParams.gridCounts.y * gridX + gridY;
            if (stackIndex == stackHeights[index].Value - 1)
            {
                return true;
            }
        }
        return false;

    }


    public static void UpdateStackHeights(ResourceGridParams resGridParams, DynamicBuffer<StackHeightParams> stackHeights, 
                                            int gridX, int gridY, bool stacked, int updateValue)
    {
        if (stacked)
        {
            int index = resGridParams.gridCounts.y * gridX + gridY;

            var element = stackHeights[index];
            element.Value += updateValue;
            stackHeights[index] = element;
        }
    }

    /*
    public static float3 GetRandomPosition(ResourceGridParams resGridParams, FieldAuthoring field, float randomValue)
    {
        float3 pos;
        pos.x = resGridParams.minGridPos.x * .25f + randomValue * field.size.x * .25f;
        pos.y = randomValue * 10f;
        pos.z = resGridParams.minGridPos.y + randomValue * field.size.z;

        return pos;
    }
    */

    public static float3 NearestSnappedPos(ResourceGridParams resGridParams, float3 pos)
    {
        GetGridIndex(resGridParams, pos, out int gridX, out int gridY);

        float3 snapPos;
        snapPos.x = resGridParams.minGridPos.x + gridX * resGridParams.gridSize.x;
        snapPos.y = pos.y;
        snapPos.z = resGridParams.minGridPos.y + gridY * resGridParams.gridSize.y;
        return snapPos;
    }


    public static void GetGridIndex(ResourceGridParams resGridParams, float3 pos, out int gridX, out int gridY)
    {
        gridX = Mathf.FloorToInt((pos.x - resGridParams.minGridPos.x + resGridParams.gridSize.x * .5f) / resGridParams.gridSize.x);
        gridY = Mathf.FloorToInt((pos.z - resGridParams.minGridPos.y + resGridParams.gridSize.y * .5f) / resGridParams.gridSize.y);

        gridX = Mathf.Clamp(gridX, 0, resGridParams.gridCounts.x - 1);
        gridY = Mathf.Clamp(gridY, 0, resGridParams.gridCounts.y - 1);
    }


    public static float3 GetStackPos(ResourceParams resParams, ResourceGridParams resGridParams, FieldAuthoring field,
                                        DynamicBuffer<StackHeightParams> stackHeights, int gridX, int gridY)
    {
        int index = resGridParams.gridCounts.y * gridX + gridY;
        var height = stackHeights[index];

        float3 pos;
        pos.x = resGridParams.minGridPos.x + gridX * resGridParams.gridSize.x;
        pos.y = -field.size.y * .5f + (height.Value + .5f) * resParams.resourceSize;
        pos.z = resGridParams.minGridPos.y + gridY * resGridParams.gridSize.y;
        return pos;
    }

    public static int SearchDeadBee(NativeList<Entity> deadBeelist, Entity beeEntity)
    {
        for(int i = 0; i < deadBeelist.Length; i++)
        {
            if(deadBeelist.ElementAt(i) == beeEntity)
            {
                return i;
            }
        }
        return -1;
    }

    public static bool MouseRayTrace(Camera camera, FieldAuthoring field, out float3 pos)
    {
        bool isMouseTouchingField = false;
        Vector3 hitPoint = new Vector3();

        Ray mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        for(int i = 0; i < 3; i++)
        {
            for(int j = -1; j <= 1; j += 2)
            {
                Vector3 wallCenter = new Vector3();
                Vector3 size = new Vector3();
                if(i == 0)
                {
                    size[i] = field.size.x;
                }
                else if(i == 1)
                {
                    size[i] = field.size.y;
                }
                else
                {
                    size[i] = field.size.z;
                }

                wallCenter[i] = size[i] * .5f * j;
                Plane plane = new Plane(-wallCenter, wallCenter);
                float hitDistance;
                if (Vector3.Dot(plane.normal, mouseRay.direction) < 0f)
                {
                    if (plane.Raycast(mouseRay, out hitDistance))
                    {
                        hitPoint = mouseRay.GetPoint(hitDistance);
                        bool insideField = true;
                        for (int k = 0; k < 3; k++)
                        {
                            if (Mathf.Abs(hitPoint[k]) > size[k] * .5f + .01f)
                            {
                                insideField = false;
                                break;
                            }
                        }
                        if (insideField)
                        {
                            isMouseTouchingField = true;
                            break;
                        }
                    }
                }
            }
            if (isMouseTouchingField)
            {
                break;
            }
        }

        pos = new float3(hitPoint[0], hitPoint[1], hitPoint[2]);

        return isMouseTouchingField;
    }

    /*
    
		if (isMouseTouchingField) {
			marker.position = worldMousePosition;
			if (marker.gameObject.activeSelf == false) {
				marker.gameObject.SetActive(true);
			}
		} else {
			if (marker.gameObject.activeSelf) {
				marker.gameObject.SetActive(false);
			}
		}
	}
    
    */
}