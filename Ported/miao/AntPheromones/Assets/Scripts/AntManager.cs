using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AntPheromones_ECS
{
	public class AntManager : MonoBehaviour
	{
		[Header("Materials")]
		public Material obstacleMaterial;
		public Material resourceMaterial;
		public Material colonyMaterial;
		
		[Header("Meshes")]
		public Mesh obstacleMesh;
		public Mesh colonyMesh;
		public Mesh resourceMesh;
		
		public int AntCount = 1000;
		public int MapWidth = 128;
		public int BucketResolution;

		public int ObstacleRingCount = 3;
		public float ObstaclesPerRing = 0.8f;
		public float ObstacleRadius = 2;

		public float2[,][] ObstacleBuckets
		{
			get
			{
				if (!this._obstacleBuckets.IsGenerated)
				{
					GenerateObstacles();
				}
				return this._obstacleBuckets.Values;
			}
		}
		
		public float2[] ObstaclePositions
		{
			get
			{
				if (!this._obstaclePositions.IsGenerated)
				{
					GenerateObstacles();
				}
				return this._obstaclePositions.Values;
			}
		}

		public float2 ColonyPosition
		{
			get
			{
				if (!this._colonyPosition.IsCalculated)
				{
					this._colonyPosition = (IsCalculated: true, Value: new float2(1f, 1f) * this.MapWidth * 0.5f);
				}
				return this._colonyPosition.Value;
			}
		}

		public float2 ResourcePosition
		{
			get
			{
				if (!this._resourcePosition.IsCalculated)
				{
					float resourceAngle = Random.value * 2f * Mathf.PI;
					this._resourcePosition = 
						(IsCalculated: true,
						Value: 0.5f * this.MapWidth * new float2(1f, 1f) + new float2(
							       Mathf.Cos(resourceAngle) * this.MapWidth * 0.475f,
							       Mathf.Sin(resourceAngle) * this.MapWidth * 0.475f));
				}

				return this._resourcePosition.Value;
			}
		}
		
		private (bool IsCalculated, float2 Value) _colonyPosition;
		private (bool IsCalculated, float2 Value) _resourcePosition;
		private (bool IsGenerated, float2[] Values) _obstaclePositions;
		private (bool IsGenerated, float2[,][] Values) _obstacleBuckets;
		
		private Matrix4x4[][] _obstacleMatrices;
		private Matrix4x4 _resourceMatrix;
		private Matrix4x4 _colonyMatrix;

		private bool _isJustStarted;
		private (bool IsRetrieved, SimulationSystemGroup Value) _simulationSystemGroup;

		private const int InstancesPerBatch = 1023;

		private void Start()
		{
			this._isJustStarted = true;
			
			this._colonyMatrix = float4x4.TRS(
				new float3(this._colonyPosition.Value / this.MapWidth, 0), 
				Quaternion.identity, 
				new float3(4f, 4f, 0.1f) / this.MapWidth);

			this._resourceMatrix = float4x4.TRS(
				new float3(this._resourcePosition.Value / this.MapWidth, 0f),
				Quaternion.identity,
				new float3(4f, 4f, 0.1f) / this.MapWidth);
			
			CreateAnts();
		}

		private void Update()
		{
			for (int i = 0; i < this._obstacleMatrices.Length; i++)
			{
				Graphics.DrawMeshInstanced(this.obstacleMesh, submeshIndex: 0, obstacleMaterial, _obstacleMatrices[i]);
			}

			Graphics.DrawMesh(colonyMesh, _colonyMatrix, colonyMaterial, layer: 0);
			Graphics.DrawMesh(resourceMesh, _resourceMatrix, resourceMaterial, layer: 0);
		}

		private void FixedUpdate()
		{
			if (this._isJustStarted)
			{
				this._isJustStarted = false;
				return;
			}

			if (!this._simulationSystemGroup.IsRetrieved)
			{
				this._simulationSystemGroup = 
					(IsRetrieved: true, 
					Value: World.Active.GetExistingSystem<SimulationSystemGroup>());
			}
			
			this._simulationSystemGroup.Value.Update();
		}

		private void CreateAnts()
		{
			EntityManager entityManager = World.Active.EntityManager;
			
			EntityArchetype antArchetype =
				entityManager.CreateArchetype(
					typeof(Position),
					typeof(Velocity),
					typeof(Speed),
					typeof(FacingAngle),
					typeof(Colour),
					typeof(Brightness),
					typeof(ResourceCarrier),
					typeof(PheromoneSteering),
					typeof(WallSteering),
					typeof(LocalToWorld));

			NativeArray<Entity> antEntities = new NativeArray<Entity>(length: this.AntCount, Allocator.Temp);
			entityManager.CreateEntity(antArchetype, antEntities);

			foreach (Entity entity in antEntities)
			{
				entityManager.SetComponentData(
					entity,
					new Position
					{
						Value = this.MapWidth * 0.5f + new float2(Random.Range(-5f, 5f), Random.Range(-5f, 5f))
					});
				entityManager.SetComponentData(
					entity,
					new Brightness
					{
						Value = Random.Range(0.75f, 1.25f)
					});
				entityManager.SetComponentData(
					entity,
					new FacingAngle
					{
						Value = Random.value * 2 * math.PI
					});
			}
			antEntities.Dispose();
		}
		
		private void GenerateObstacles()
		{
			this._obstaclePositions = (IsGenerated: true, CalculateObstaclePositions().ToArray());
			this._obstacleMatrices = CalculateObstacleMatrices();
			
			List<float2>[,] temporaryObstacleBuckets = GenerateObstacleBuckets();
			
			this._obstacleBuckets.Values = new float2[this.BucketResolution, this.BucketResolution][];

			for (int x = 0; x < this.BucketResolution; x++)
			{
				for (int y = 0; y < this.BucketResolution; y++)
				{
					this._obstacleBuckets.Values[x, y] = temporaryObstacleBuckets[x, y].ToArray();
				}
			}

			this._obstacleBuckets.IsGenerated = true;
		}

		private List<float2>[,] GenerateObstacleBuckets()
		{
			List<float2>[,] buckets = new List<float2>[this.BucketResolution,this.BucketResolution];

			for (int x = 0; x < this.BucketResolution; x++)
			{
				for (int y = 0; y < this.BucketResolution; y++)
				{
					buckets[x, y] = new List<float2>();
				}
			}

			for (int i = 0; i < this._obstaclePositions.Values.Length; i++)
			{
				float2 position = this._obstaclePositions.Values[i];
				
				for (int x = Mathf.FloorToInt((position.x - this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
					x <= Mathf.FloorToInt((position.x + this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
					x++)
				{
					if (x < 0 || x >= BucketResolution)
					{
						continue;
					}

					for (int y = Mathf.FloorToInt((position.y - this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
						y <= Mathf.FloorToInt((position.y + this.ObstacleRadius) / this.MapWidth * this.BucketResolution);
						y++)
					{
						if (y < 0 || y >= this.BucketResolution)
						{
							continue;
						}

						buckets[x, y].Add(this._obstaclePositions.Values[i]);
					}
				}
			}

			return buckets;
		}

		private IEnumerable<float2> CalculateObstaclePositions()
		{
            for (int i = 1; i <= this.ObstacleRingCount; i++)
            {
            	float ringRadius = i / (this.ObstacleRingCount + 1f) * (this.MapWidth * 0.5f);
            	float circumference = ringRadius * 2f * Mathf.PI;
            	int maxCount = Mathf.CeilToInt(circumference / (2f * this.ObstacleRadius) * 2f);
                
            	int offset = Random.Range(0, maxCount);
            	int holeCount = Random.Range(1, 3);
            	
            	for (int j = 0; j < maxCount; j++)
            	{
            		float t = (float) j / maxCount;

            		if (!(t * holeCount % 1f < this.ObstaclesPerRing))
            		{
            			continue;
            		}
            		
            		float angle = (j + offset) / (float) maxCount * (2f * Mathf.PI);
            		yield return new float2(
	                    x: this.MapWidth * 0.5f + math.cos(angle) * ringRadius,
            			y: this.MapWidth * 0.5f + math.sin(angle) * ringRadius); 
            	}
            }
		}

		private Matrix4x4[][] CalculateObstacleMatrices()
		{
			Matrix4x4[][] obstacleMatrices =
				new Matrix4x4[Mathf.CeilToInt((float)this._obstaclePositions.Values.Length / InstancesPerBatch)][];

			for (int i = 0; i < obstacleMatrices.Length; i++)
			{
				obstacleMatrices[i] = new Matrix4x4[Mathf.Min(InstancesPerBatch, this._obstaclePositions.Values.Length - i * InstancesPerBatch)];
				
				for (int j = 0; j < obstacleMatrices[i].Length; j++)
				{
					obstacleMatrices[i][j] =
						Matrix4x4.TRS(
							new float3(xy: this._obstaclePositions.Values[i * InstancesPerBatch + j] / this.MapWidth, z: 0),
							Quaternion.identity,
							s: new float3(this.ObstacleRadius * 2f, this.ObstacleRadius * 2f, 1f) / MapWidth);
				}
			}

			return obstacleMatrices;
		}
	}
}