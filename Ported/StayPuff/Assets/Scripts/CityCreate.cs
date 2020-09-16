using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
//using UnityEngine;

public class CityCreate : SystemBase
{
    Random SystemRandom;

    protected override void OnCreate()
    {
        SystemRandom = new Random(999);
    }

    protected override void OnUpdate()
    {
        Random jobRandom = SystemRandom;
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in CityData city) =>
            {
          
                for (int x = 0; x < city.CountX; ++x)
                    for (int z = 0; z < city.CountZ; ++z)
                    {
                        //Random jobRandom = new Random(x);
                        var posX = 10 * (x - (city.CountX - 1) / 2);
                        var posY = 0f;
                        var posZ = 10 * (z - (city.CountZ - 1) / 2);
                        Entity newentity = EntityManager.CreateEntity();
                        EntityManager.AddComponentData(newentity, 
                            new SpawnData { 
                                position = new float3(posX, posY, posZ), 
                                height = jobRandom.NextInt(city.height.x, city.height.y) });
                    }

                EntityManager.DestroyEntity(entity);
            }).Run();
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