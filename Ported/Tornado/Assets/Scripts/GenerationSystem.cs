using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GenerationSystem : ComponentSystem
{
	private EntityQuery settingsQuery;
	
	public struct State : IComponentData
	{
		public float tornadoFader;
		public float tornadoX;
		public float tornadoZ;
	}

	protected override void OnCreate()
	{
		base.OnCreate();
		
		settingsQuery = GetEntityQuery( new EntityQueryDesc {All=new ComponentType[]{typeof(GenerationSetting)}, 
			None=new ComponentType[]{typeof(State)}});
		
		RequireForUpdate(settingsQuery);
	}

	protected override void OnUpdate()
	{
		var settingsEntity = settingsQuery.GetSingletonEntity();
		var settings = EntityManager.GetComponentObject<GenerationSetting>(settingsEntity);
		
		PostUpdateCommands.AddComponent<State>(settingsEntity);
		Debug.Log("Found the entity");
            
		var pointsList = new NativeList<ConstrainedPoint>(Allocator.Temp);
		var barsList = new NativeList<Bar>(Allocator.Temp);
//		List<List<Matrix4x4>> matricesList = new List<List<Matrix4x4>>();
//		matricesList.Add(new List<Matrix4x4>());

		// buildings
		for (int i = 0; i < 1000; i++) {
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
		for (uint i = 0; i < pointsList.Length; i++) {
			for (uint j = i + 1; j < pointsList.Length; j++)
			{

				var p1 = pointsList[(int)i];
				var p2 = pointsList[(int)j];
				Vector3 delta = p2.position - p1.position;
				var length = delta.magnitude;

				if (length < 5f && length>.2f) {
					Bar bar = new Bar();
					bar.AssignPoints(i, j, length);
					p1.neighborCount++;
					p2.neighborCount++;
					barsList.Add(bar);

					pointsList[(int)i] = p1;
					pointsList[(int)j] = p2;

//					matricesList[batch].Add(bar.matrix);
//					if (matricesList[batch].Count == instancesPerBatch) {
//						batch++;
//						matricesList.Add(new List<Matrix4x4>());
//					}
				}
			}
		}

		var connectedPoints = new NativeList<ConstrainedPoint>(Allocator.Temp);
		var pointCount = 0;
		for (int i = 0 ;i < pointsList.Length; i++) {
			if (pointsList[i].neighborCount > 0) {
				connectedPoints.Add(pointsList[i]);
			}
		}
		Debug.Log(pointCount + " points, room for " + connectedPoints.Length + " (" + barsList.Length + " bars)");
		
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
        var pointBuffer = PostUpdateCommands.AddBuffer<ConstrainedPointEntry>(pointsEntity);
        pointBuffer.AddRange(connectedPoints.AsArray().Reinterpret<ConstrainedPointEntry>());
        var matrixBuffer = PostUpdateCommands.AddBuffer<RenderMatrixEntry>(pointsEntity);

        var barsBuffer = PostUpdateCommands.AddBuffer<BarEntry>(pointsEntity);
        barsBuffer.AddRange(barsList.AsArray().Reinterpret<BarEntry>());

        barsList.Dispose();
        connectedPoints.Dispose();
        pointsList.Dispose();
	}
}