using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GenerationSystem : ComponentSystem
{
	public struct State : ISystemStateComponentData
	{
		public float tornadoFader;
		public float tornadoX;
		public float tornadoZ;
	}

	protected override void OnUpdate()
    {
        Entities.WithNone<State>().ForEach((Entity entity, ref GenerationSetting settings) =>
        {
	        PostUpdateCommands.AddComponent<State>(entity);
	        
//            entity = PostUpdateCommands.CreateEntity();
            Debug.Log("Found the entity");
            
		var pointsList = new NativeList<ConstrainedPoint>(Allocator.Temp);
//		List<Bar> barsList = new List<Bar>();
//		List<List<Matrix4x4>> matricesList = new List<List<Matrix4x4>>();
//		matricesList.Add(new List<Matrix4x4>());

		// buildings
		for (int i = 0; i < 35; i++) {
			int height = Random.Range(4,12);
			Vector3 pos = new Vector3(Random.Range(-45f,45f),0f,Random.Range(-45f,45f));
			float spacing = 2f;
			for (int j = 0; j < height; j++) {
				var point = new ConstrainedPoint();
				point.position.x = pos.x+spacing;
				point.position.y = j * spacing;
				point.position.z = pos.z-spacing;
				point.oldPosition = point.position;
				if (j==0) {
					point.anchor=true;
				}
				pointsList.Add(point);
				point = new ConstrainedPoint();
				point.position.x = pos.x-spacing;
				point.position.y = j * spacing;
				point.position.z = pos.z-spacing;
				point.oldPosition = point.position;
				if (j==0) {
					point.anchor=true;
				}
				pointsList.Add(point);
				point = new ConstrainedPoint();
				point.position.x = pos.x+0f;
				point.position.y = j * spacing;
				point.position.z = pos.z+spacing;
				point.oldPosition = point.position;
				if (j==0) {
					point.anchor=true;
				}
				pointsList.Add(point);
			}
		}

//		// ground details
//		for (int i=0;i<600;i++) {
//			Vector3 pos = new Vector3(Random.Range(-55f,55f),0f,Random.Range(-55f,55f));
//			Point point = new Point();
//			point.x = pos.x + Random.Range(-.2f,-.1f);
//			point.y = pos.y+Random.Range(0f,3f);
//			point.z = pos.z + Random.Range(.1f,.2f);
//			point.oldX = point.x;
//			point.oldY = point.y;
//			point.oldZ = point.z;
//			pointsList.Add(point);
//
//			point = new Point();
//			point.x = pos.x + Random.Range(.2f,.1f);
//			point.y = pos.y + Random.Range(0f,.2f);
//			point.z = pos.z + Random.Range(-.1f,-.2f);
//			point.oldX = point.x;
//			point.oldY = point.y;
//			point.oldZ = point.z;
//			if (Random.value<.1f) {
//				point.anchor = true;
//			}
//			pointsList.Add(point);
//		}
//
//		int batch = 0;
//
//		for (int i = 0; i < pointsList.Count; i++) {
//			for (int j = i + 1; j < pointsList.Count; j++) {
//				Bar bar = new Bar();
//				bar.AssignPoints(pointsList[i],pointsList[j]);
//				if (bar.length < 5f && bar.length>.2f) {
//					bar.point1.neighborCount++;
//					bar.point2.neighborCount++;
//
//					barsList.Add(bar);
//					matricesList[batch].Add(bar.matrix);
//					if (matricesList[batch].Count == instancesPerBatch) {
//						batch++;
//						matricesList.Add(new List<Matrix4x4>());
//					}
//					if (barsList.Count % 500 == 0) {
//						yield return null;
//					}
//				}
//			}
//		}
//		points = new Point[barsList.Count * 2];
//		pointCount = 0;
//		for (int i=0;i<pointsList.Count;i++) {
//			if (pointsList[i].neighborCount > 0) {
//				points[pointCount] = pointsList[i];
//				pointCount++;
//			}
//		}
//		Debug.Log(pointCount + " points, room for " + points.Length + " (" + barsList.Count + " bars)");
//
//		bars = barsList.ToArray();
//
//		matrices = new Matrix4x4[matricesList.Count][];
//		for (int i=0;i<matrices.Length;i++) {
//			matrices[i] = matricesList[i].ToArray();
//		}
//
//		matProps = new MaterialPropertyBlock[barsList.Count];
//		Vector4[] colors = new Vector4[instancesPerBatch];
//		for (int i=0;i<barsList.Count;i++) {
//			colors[i%instancesPerBatch] = barsList[i].color;
//			if ((i + 1) % instancesPerBatch == 0 || i == barsList.Count - 1) {
//				MaterialPropertyBlock block = new MaterialPropertyBlock();
//				block.SetVectorArray("_Color",colors);
//				matProps[i / instancesPerBatch] = block;
//			}
//		}
//
//		pointsList = null;
//		barsList = null;
//		matricesList = null;
//		System.GC.Collect();
//		generating = false;
//		Time.timeScale = 1f;

	        var pointsEntity = PostUpdateCommands.CreateEntity();
            var buffer = PostUpdateCommands.AddBuffer<ConstrainedPointEntry>(pointsEntity);
            buffer.AddRange(pointsList.AsArray().Reinterpret<ConstrainedPointEntry>());
        });
    }
}