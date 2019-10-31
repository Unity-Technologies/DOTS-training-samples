using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class BarSystemAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	//const int instancesPerBatch = 1023;
    //private Point[] points;
    //private int pointCount;
    //private Bar[] bars;
    //private Matrix4x4[][] matrices;
    //private MaterialPropertyBlock[] matProps;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        BarPoint point1 = new BarPoint { index=1 };
        BarPoint point2 = new BarPoint { index=2 };
        BarPoint point3 = new BarPoint { index=3 };
        Entity pointEntity1 = conversionSystem.DstEntityManager.CreateEntity();
        Entity pointEntity2 = conversionSystem.DstEntityManager.CreateEntity();
        Entity pointEntity3 = conversionSystem.DstEntityManager.CreateEntity();
        conversionSystem.DstEntityManager.AddComponentData(pointEntity1, point1);
        conversionSystem.DstEntityManager.AddComponentData(pointEntity2, point2);
        conversionSystem.DstEntityManager.AddComponentData(pointEntity3, point3);

        Bar bar1 = new Bar { point1 = pointEntity1, point2 = pointEntity2 };
        Bar bar2 = new Bar { point1 = pointEntity2, point2 = pointEntity3 };
        Bar bar3 = new Bar { point1 = pointEntity3, point2 = pointEntity1 };
        Entity barEntity1 = conversionSystem.DstEntityManager.CreateEntity();
        Entity barEntity2 = conversionSystem.DstEntityManager.CreateEntity();
        Entity barEntity3 = conversionSystem.DstEntityManager.CreateEntity();
        conversionSystem.DstEntityManager.AddComponentData(barEntity1, bar1);
        conversionSystem.DstEntityManager.AddComponentData(barEntity2, bar2);
        conversionSystem.DstEntityManager.AddComponentData(barEntity3, bar3);

        //pointEntity1.Index
        //dstManager

        //Bar bar1 = new Bar { point1Index = 1, point2Index = 2 };
        //Bar bar2 = new Bar { point1Index = 2, point2Index = 3 };
        //Bar bar3 = new Bar { point1Index = 3, point2Index = 1 };

        //conversionSystem.DstEntityManager.AddSharedComponentData(pointEntity1, bar1);
        //conversionSystem.DstEntityManager.AddSharedComponentData(pointEntity1, bar3);
        //conversionSystem.DstEntityManager.AddSharedComponentData(pointEntity2, bar1);
        //conversionSystem.DstEntityManager.AddSharedComponentData(pointEntity2, bar2);
        //conversionSystem.DstEntityManager.AddSharedComponentData(pointEntity3, bar2);
        //conversionSystem.DstEntityManager.AddSharedComponentData(pointEntity3, bar3);

        //NativeMultiHashMap<int, int> barMap = new NativeMultiHashMap<int, int>();
        //barMap.Add(point1.index, bar1.point1Index);
        //barMap.Add(point1.index, bar1.point2Index);
        //barMap.Add(point2.index, bar1.point1Index);
        //barMap.Add(point2.index, bar1.point1Index);
        //barMap.Add(point3.index, bar1.point2Index);
        //barMap.Add(point3.index, bar1.point2Index);
        //dstManager.AddBuffer<PointBufferElement>(entity);
        //dstManager.Bu
    }

    /*
    public void AssignPoints(Bar bar, Point a, Point b) {
		bar.point1 = a;
		bar.point2 = b;
		Vector3 delta = new Vector3(
            bar.point2.pos.x - bar.point1.pos.x,
            bar.point2.pos.y - bar.point1.pos.y,
            bar.point2.pos.z - bar.point1.pos.z
        );
		bar.length = delta.magnitude;

		bar.thickness = UnityEngine.Random.Range(.25f,.35f);

		Vector3 pos = new Vector3(bar.point1.pos.x + bar.point2.pos.x,
            bar.point1.pos.y + bar.point2.pos.y,
            bar.point1.pos.z + bar.point2.pos.z) * .5f;
		Quaternion rot = Quaternion.LookRotation(delta);
		Vector3 scale = new Vector3(bar.thickness, bar.thickness, bar.length);
		bar.matrix = Matrix4x4.TRS(pos,rot,scale);

		bar.minBounds.x = Mathf.Min(bar.point1.pos.x, bar.point2.pos.x);
		bar.maxBounds.x = Mathf.Max(bar.point1.pos.x,bar.point2.pos.x);
		bar.minBounds.y = Mathf.Min(bar.point1.pos.y,bar.point2.pos.y);
		bar.maxBounds.y = Mathf.Max(bar.point1.pos.y,bar.point2.pos.y);
		bar.minBounds.z = Mathf.Min(bar.point1.pos.z,bar.point2.pos.z);
		bar.maxBounds.z = Mathf.Max(bar.point1.pos.z,bar.point2.pos.z);

		float upDot = Mathf.Acos(Mathf.Abs(Vector3.Dot(Vector3.up,delta.normalized)))/Mathf.PI;
		bar.color = Color.white * upDot * UnityEngine.Random.Range(.7f,1f);
	}
    */

    /*
    public void Generate() {
        List<Point> pointsList = new List<Point>();
        List<Bar> barsList = new List<Bar>();
        List<List<Matrix4x4>> matricesList = new List<List<Matrix4x4>>();
        matricesList.Add(new List<Matrix4x4>());

        // buildings
        for (int i = 0; i < 35; i++) {
            int height = UnityEngine.Random.Range(4, 12);
            float3 pos = new Vector3(UnityEngine.Random.Range(-45f, 45f), 0f, UnityEngine.Random.Range(-45f, 45f));
            float spacing = 2f;
            for (int j = 0; j < height; j++) {
                Point point = new Point();
                point.oldPos.x = point.pos.x = pos.x + spacing;
                point.oldPos.y = point.pos.y = j * spacing;
                point.oldPos.z = point.pos.z = pos.z - spacing;
                if (j == 0) {
                    point.anchor = true;
                }
                pointsList.Add(point);
                point = new Point();
                point.oldPos.x = point.pos.x = pos.x - spacing;
                point.oldPos.y = point.pos.y = j * spacing;
                point.oldPos.z = point.pos.z = pos.z - spacing;
                if (j == 0) {
                    point.anchor = true;
                }
                pointsList.Add(point);
                point = new Point();
                point.oldPos.x = point.pos.x = pos.x + 0f;
                point.oldPos.y = point.pos.y = j * spacing;
                point.oldPos.z = point.pos.z = pos.z + spacing;
                if (j == 0) {
                    point.anchor = true;
                }
                pointsList.Add(point);
            }
        }

        // ground details
        for (int i = 0; i < 600; i++) {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-55f, 55f), 0f, UnityEngine.Random.Range(-55f, 55f));
            Point point = new Point();
            point.oldPos.x = point.pos.x = pos.x + UnityEngine.Random.Range(-.2f, -.1f);
            point.oldPos.y = point.pos.y = pos.y + UnityEngine.Random.Range(0f, 3f);
            point.oldPos.z = point.pos.z = pos.z + UnityEngine.Random.Range(.1f, .2f);
            pointsList.Add(point);

            point = new Point();
            point.oldPos.x = point.pos.x = pos.x + UnityEngine.Random.Range(.2f, .1f);
            point.oldPos.y = point.pos.y = pos.y + UnityEngine.Random.Range(0f, .2f);
            point.oldPos.z = point.pos.z = pos.z + UnityEngine.Random.Range(-.1f, -.2f);
            if (UnityEngine.Random.value < .1f) {
                point.anchor = true;
            }
            pointsList.Add(point);
        }

        int batch = 0;

        for (int i = 0; i < pointsList.Count; i++) {
            for (int j = i + 1; j < pointsList.Count; j++) {
                Bar bar = new Bar();
                AssignPoints(bar, pointsList[i], pointsList[j]);
                if (bar.length < 5f && bar.length > .2f) {
                    bar.point1.neighborCount++;
                    bar.point2.neighborCount++;

                    barsList.Add(bar);
                    matricesList[batch].Add(bar.matrix);
                    if (matricesList[batch].Count == instancesPerBatch) {
                        batch++;
                        matricesList.Add(new List<Matrix4x4>());
                    }
                }
            }
        }

        points = new Point[barsList.Count * 2];
        pointCount = 0;
        for (int i = 0; i < pointsList.Count; i++) {
            if (pointsList[i].neighborCount > 0) {
                points[pointCount] = pointsList[i];
                pointCount++;
            }
        }
        Debug.Log(pointCount + " points, room for " + points.Length + " (" + barsList.Count + " bars)");

        bars = barsList.ToArray();

        matrices = new Matrix4x4[matricesList.Count][];
        for (int i = 0; i < matrices.Length; i++) {
            matrices[i] = matricesList[i].ToArray();
        }

        matProps = new MaterialPropertyBlock[barsList.Count];
        Vector4[] colors = new Vector4[instancesPerBatch];
        for (int i = 0; i < barsList.Count; i++) {
            colors[i % instancesPerBatch] = barsList[i].color;
            if ((i + 1) % instancesPerBatch == 0 || i == barsList.Count - 1) {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetVectorArray("_Color", colors);
                matProps[i / instancesPerBatch] = block;
            }
        }

        pointsList = null;
        barsList = null;
        matricesList = null;
        System.GC.Collect();
    }
    */
	
}
