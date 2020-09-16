using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BuildingCreate : SystemBase
{
    protected override void OnUpdate()
    {
      
        
        
        Entities.ForEach((ref Translation translation, in Rotation rotation) => {

        }).Schedule();
    }
}
//for (int i = 0; i < 35; i++)
//{
//	int height = Random.Range(4, 12);
//	Vector3 pos = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));
//	float spacing = 2f;
//	for (int j = 0; j < height; j++)
//	{
//		Point point = new Point();
//		point.x = pos.x + spacing;
//		point.y = j * spacing;
//		point.z = pos.z - spacing;
//		point.oldX = point.x;
//		point.oldY = point.y;
//		point.oldZ = point.z;
//		if (j == 0)
//		{
//			point.anchor = true;
//		}
//		pointsList.Add(point);
//		point = new Point();
//		point.x = pos.x - spacing;
//		point.y = j * spacing;
//		point.z = pos.z - spacing;
//		point.oldX = point.x;
//		point.oldY = point.y;
//		point.oldZ = point.z;
//		if (j == 0)
//		{
//			point.anchor = true;
//		}
//		pointsList.Add(point);
//		point = new Point();
//		point.x = pos.x + 0f;
//		point.y = j * spacing;
//		point.z = pos.z + spacing;
//		point.oldX = point.x;
//		point.oldY = point.y;
//		point.oldZ = point.z;
//		if (j == 0)
//		{
//			point.anchor = true;
//		}
//		pointsList.Add(point);
//	}
//}