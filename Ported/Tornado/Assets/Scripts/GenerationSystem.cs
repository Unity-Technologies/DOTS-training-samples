using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
		var listOfLists = new List<NativeList<Bar>>();

		// buildings
		for (int i = 0; i < 100; i++)
		{
			var plStart = pointsList.Length;
			var barsList = new NativeList<Bar>(Allocator.Temp);
			listOfLists.Add(barsList);
			int height = Random.Range(4,12);
			Vector3 pos = new Vector3(Random.Range(-45f,45f),0f,Random.Range(-45f,45f));
			float spacing = 2f;
			for (int j = 0; j < height; j++)
			{
				var point = new ConstrainedPoint();
				point.position.x = pos.x + spacing;
				point.position.y = j * spacing;
				point.position.z = pos.z - spacing;
				point.oldPosition = point.position;
				if (j == 0)
				{
					point.anchor = true;
				}

				pointsList.Add(point);
				point = new ConstrainedPoint();
				point.position.x = pos.x - spacing;
				point.position.y = j * spacing;
				point.position.z = pos.z - spacing;
				point.oldPosition = point.position;
				if (j == 0)
				{
					point.anchor = true;
				}

				pointsList.Add(point);
				point = new ConstrainedPoint();
				point.position.x = pos.x + 0f;
				point.position.y = j * spacing;
				point.position.z = pos.z + spacing;
				point.oldPosition = point.position;
				if (j == 0)
				{
					point.anchor = true;
				}

				pointsList.Add(point);
			}
			
			for (int pl = plStart; pl < pointsList.Length; pl++) {
				for (int j = pl + 1; j < pointsList.Length; j++)
				{
					var p1 = pointsList[pl];
					var p2 = pointsList[j];
					Vector3 delta = p2.position - p1.position;
					var length = delta.magnitude;

					if (length < 5f && length>.2f) {
						Bar bar = new Bar();
						bar.AssignPoints(pl, j, length);
						p1.neighborCount++;
						p2.neighborCount++;
						barsList.Add(bar);

						pointsList[pl] = p1;
						pointsList[j] = p2;
					}
				}
			}
		}

		// ground details
//		for (int i=0;i<600;i++) {
//			Vector3 pos = new Vector3(Random.Range(-55f,55f),0f,Random.Range(-55f,55f));
//			var point = new ConstrainedPoint();
//			point.position = new float3(pos.x + Random.Range(-.2f, -.1f), pos.y + Random.Range(0f, 3f), pos.z + Random.Range(.1f, .2f));
//			point.oldPosition = point.position;
//			pointsList.Add(point);
//
//			point = new ConstrainedPoint();
//			point.position = new float3(pos.x + Random.Range(.2f, .1f), pos.y + Random.Range(0f, .2f), pos.z + Random.Range(-.1f, -.2f));
//			point.oldPosition = point.position;
//			pointsList.Add(point);
//
//			if (Random.value<.1f) {
//				point.anchor = true;
//			}
//			pointsList.Add(point);
//		}

//		var connectedPoints = new NativeList<ConstrainedPoint>(Allocator.Temp);
//		var pointCount = 0;
//		for (int i = 0 ;i < pointsList.Length; i++) {
//			if (pointsList[i].neighborCount > 0) {
//				connectedPoints.Add(pointsList[i]);
//			}
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

		var pointsEntity = PostUpdateCommands.CreateEntity();
        var pointBuffer = PostUpdateCommands.AddBuffer<ConstrainedPointEntry>(pointsEntity);
        pointBuffer.AddRange(pointsList.AsArray().Reinterpret<ConstrainedPointEntry>());

        foreach (var bl in listOfLists)
        {
	        var barsEntity = PostUpdateCommands.CreateEntity();
	        var barsBuffer = PostUpdateCommands.AddBuffer<BarEntry>(barsEntity);
	        PostUpdateCommands.AddBuffer<RenderMatrixEntry>(barsEntity);
	        barsBuffer.AddRange(bl.AsArray().Reinterpret<BarEntry>());
	        bl.Dispose();
        }

        pointsList.Dispose();
	}
}