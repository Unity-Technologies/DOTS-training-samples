using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CityCreate : SystemBase
{
    protected override void OnUpdate()
    {
        
        
        Entities.ForEach((ref Translation translation, in Rotation rotation) => {

        }).Schedule();
    }
}

//// ground details
//for (int i = 0; i < 600; i++)
//{
//	Vector3 pos = new Vector3(Random.Range(-55f, 55f), 0f, Random.Range(-55f, 55f));
//	Point point = new Point();
//	point.x = pos.x + Random.Range(-.2f, -.1f);
//	point.y = pos.y + Random.Range(0f, 3f);
//	point.z = pos.z + Random.Range(.1f, .2f);
//	point.oldX = point.x;
//	point.oldY = point.y;
//	point.oldZ = point.z;
//	pointsList.Add(point);

//	point = new Point();
//	point.x = pos.x + Random.Range(.2f, .1f);
//	point.y = pos.y + Random.Range(0f, .2f);
//	point.z = pos.z + Random.Range(-.1f, -.2f);
//	point.oldX = point.x;
//	point.oldY = point.y;
//	point.oldZ = point.z;
//	if (Random.value < .1f)
//	{
//		point.anchor = true;
//	}
//	pointsList.Add(point);
//}

//int batch = 0;

//for (int i = 0; i < pointsList.Count; i++)
//{
//	for (int j = i + 1; j < pointsList.Count; j++)
//	{
//		Bar bar = new Bar();
//		bar.AssignPoints(pointsList[i], pointsList[j]);
//		if (bar.length < 5f && bar.length > .2f)
//		{
//			bar.point1.neighborCount++;
//			bar.point2.neighborCount++;

//			barsList.Add(bar);
//			matricesList[batch].Add(bar.matrix);
//			if (matricesList[batch].Count == instancesPerBatch)
//			{
//				batch++;
//				matricesList.Add(new List<Matrix4x4>());
//			}
//			if (barsList.Count % 500 == 0)
//			{
//				yield return null;
//			}
//		}
//	}
//}