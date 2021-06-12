using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Systems;
using Unity.Burst;
using Unity.Rendering;
using Unity.Collections;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class AntSystem : SystemBase
{
	BuildPhysicsWorld buildPhysicsWorld;
	StepPhysicsWorld stepPhysicsWorld;
	const int AntLayerMask = 1 << 0,
		ObstacleLayerMask = 1 << 1;
	EntityQuery antsQuery;
    protected override void OnCreate()
    {
		buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
		antsQuery = GetEntityQuery(ComponentType.ReadWrite<AntData>(), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<MaterialColor>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<NearbyObstacle>(),ComponentType.ReadWrite<Excitement>());
    }
    protected override void OnUpdate()
    {
        for (int i = 1; i < 10; i++)
        {
            if (Input.GetKey(KeyCode.Alpha0 + i))
            {
				UnityEngine.Time.timeScale = i;
				break;
            }
        }

		Dependency = new TriggerJob()
		{
			AntFromEntity = GetComponentDataFromEntity<AntData>(),
			ObstacleFromEntity = GetComponentDataFromEntity<ObstacleData>(),
			NearbyObstaclesFromEntity = GetBufferFromEntity<NearbyObstacle>(),
			TranslationFromEntity = GetComponentDataFromEntity<Translation>()
		}.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);
		Dependency.Complete();
		var collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
		var antParams = GetSingleton<AntParams>();
		var pheromoneTexture = EntityManager.GetSharedComponentData<PheromoneTexture>(GetSingletonEntity<PheromoneTexture>()).Value;
		var pheromones = pheromoneTexture.GetRawTextureData<float>();
		var resourcePosition = GetComponent<Translation>(GetSingletonEntity<Resource>()).Value.xy;
		var colonyPosition = GetComponent<Translation>(GetSingletonEntity<Colony>()).Value.xy;

		Dependency = new UpdateAntsJob()
		{
			AntParams = antParams,
			AntTypeHandle = GetComponentTypeHandle<AntData>(),
			ExcitementTypeHandle = GetComponentTypeHandle<Excitement>(),
			TranslationTypeHandle = GetComponentTypeHandle<Translation>(),
			MaterialColorTypeHandle = GetComponentTypeHandle<MaterialColor>(),
			NearbyObstaclesTypeHandle = GetBufferTypeHandle<NearbyObstacle>(),
			RotationTypeHandle = GetComponentTypeHandle<Rotation>(),
			ColonyPosition = colonyPosition,
			ResourcePosition = resourcePosition,
			CollisionWorld = collisionWorld,
			Pheromones = pheromones,
			RandomSeedFactor = (uint)Time.ElapsedTime.GetHashCode(),
			DeltaTime = Time.DeltaTime,
		}.ScheduleParallel(antsQuery,dependsOn:Dependency);
		var mapSize = antParams.MapSize;
		var trailAddSpeed = antParams.TrailAddSpeed;
		var fixedDeltaTime = Time.DeltaTime;

		Entities.ForEach((in Excitement excitement, in Translation translation) =>
		{
			DropPheromones(translation.Value.xy, excitement.Value);
			int PheromoneIndex(int x, int y)
			{
				return x + y * mapSize;
			}
			void DropPheromones(Vector2 position, float strength)
			{
				int x = Mathf.FloorToInt(position.x);
				int y = Mathf.FloorToInt(position.y);
				if (x < 0 || y < 0 || x >= mapSize || y >= mapSize)
				{
					return;
				}

				int index = PheromoneIndex(x, y);
				var pheromone = pheromones[index];
				pheromone += (trailAddSpeed * strength * fixedDeltaTime) * (1f - pheromones[index]);
				if (pheromone > 1f)
				{
					pheromone = 1f;
				}
				pheromones[index] = pheromone;
			}
		}).Schedule();
		Dependency.Complete();
		pheromoneTexture.Apply();
    }

	[BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
		public ComponentDataFromEntity<AntData> AntFromEntity;
		public ComponentDataFromEntity<ObstacleData> ObstacleFromEntity;
		public ComponentDataFromEntity<Translation> TranslationFromEntity;
		public BufferFromEntity<NearbyObstacle> NearbyObstaclesFromEntity;
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            if (AntFromEntity.HasComponent(entityA) && ObstacleFromEntity.HasComponent(entityB))
			{
				NearbyObstaclesFromEntity[entityA].Add(new NearbyObstacle() { Position = TranslationFromEntity[entityB].Value.xy });
            }
			else if(AntFromEntity.HasComponent(entityB) && ObstacleFromEntity.HasComponent(entityA))
			{
				NearbyObstaclesFromEntity[entityB].Add(new NearbyObstacle() { Position = TranslationFromEntity[entityA].Value.xy });
			}
        }
    }
	[BurstCompile]
    struct UpdateAntsJob : IJobEntityBatch
    {
		public ComponentTypeHandle<AntData> AntTypeHandle;
		public ComponentTypeHandle<Excitement> ExcitementTypeHandle;
		public ComponentTypeHandle<Translation> TranslationTypeHandle;
		public ComponentTypeHandle<MaterialColor> MaterialColorTypeHandle;
		public BufferTypeHandle<NearbyObstacle> NearbyObstaclesTypeHandle;
		public ComponentTypeHandle<Rotation> RotationTypeHandle;
		public AntParams AntParams;
		public float2 ResourcePosition;
		public float2 ColonyPosition;
		[ReadOnly] public CollisionWorld CollisionWorld;
		[ReadOnly,NativeDisableParallelForRestriction] public NativeArray<float> Pheromones;
		public uint RandomSeedFactor;
		public float DeltaTime;
		public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
		{
			var ants = batchInChunk.GetNativeArray(AntTypeHandle);
			var translations = batchInChunk.GetNativeArray(TranslationTypeHandle);
			var antColors = batchInChunk.GetNativeArray(MaterialColorTypeHandle);
			var nearbyObstaclesAccessor = batchInChunk.GetBufferAccessor(NearbyObstaclesTypeHandle);
			var rotations = batchInChunk.GetNativeArray(RotationTypeHandle);
			var excitements = batchInChunk.GetNativeArray(ExcitementTypeHandle);
			var antSpeed = AntParams.AntSpeed;
			var randomSteering = AntParams.RandomSteering;
			var mapSize = AntParams.MapSize;
			var pheromoneSteerStrength = AntParams.PheromoneSteerStrength;
			var wallSteerStrength = AntParams.WallSteerStrength;
			var antAccel = AntParams.AntAccel;
			var resourcePosition = ResourcePosition;
			var colonyPosition = ColonyPosition;
			var searchColor = AntParams.SearchColor;
			var carryColor = AntParams.CarryColor;
			var goalSteerStrength = AntParams.GoalSteerStrength;
			var obstacleRadius = AntParams.ObstacleRadius;
			var outwardStrength = AntParams.OutwardStrength;
			var inwardStrength = AntParams.InwardStrength;
			var trailAddSpeed = AntParams.TrailAddSpeed;
			var pheromones = Pheromones;
			var collisionWorld = CollisionWorld;
			
			var fixedDeltaTime = DeltaTime;
			var random = Random.CreateFromIndex((uint)batchIndex ^ RandomSeedFactor);
			for (int entityInBatchIndex = 0; entityInBatchIndex < batchInChunk.Count; entityInBatchIndex++)
			{
				var ant = ants[entityInBatchIndex];
				var position = translations[entityInBatchIndex].Value;
				var antColor = antColors[entityInBatchIndex];
				var nearbyObstacles = nearbyObstaclesAccessor[entityInBatchIndex];
				var rotation = rotations[entityInBatchIndex];
				var targetSpeed = antSpeed;
				ant.facingAngle += random.NextFloat(-randomSteering, randomSteering);

				float pheroSteering;

				{
					var distance = 3f;
					float output = 0;

					for (int i = -1; i <= 1; i += 2)
					{
						float angle = ant.facingAngle + i * math.PI * .25f;
						float testX = position.x + math.cos(angle) * distance;
						float testY = position.y + math.sin(angle) * distance;

						if (testX < 0 || testY < 0 || testX >= mapSize || testY >= mapSize)
						{

						}
						else
						{
							int index = PheromoneIndex((int)testX, (int)testY);
							float value = pheromones[index];
							output += value * i;
						}
					}
					pheroSteering = math.sign(output);
				}
				int wallSteering;
				{
					var distance = 1.5f;
					int output = 0;

					for (int i = -1; i <= 1; i += 2)
					{
						float angle = ant.facingAngle + i * math.PI * .25f;
						float testX = position.x + math.cos(angle) * distance;
						float testY = position.y + math.sin(angle) * distance;

						if (testX < 0 || testY < 0 || testX >= mapSize || testY >= mapSize)
						{

						}
						else if (collisionWorld.CastRay(new RaycastInput()
						{
							Start = position,
							End = new float3(testX, testY, 0),
							Filter = new CollisionFilter()
							{
								BelongsTo = AntLayerMask,
								CollidesWith = ObstacleLayerMask,
								GroupIndex = 0,
							}
						}))
						{
							output -= i;
						}
					}
					wallSteering = output;
				}
				ant.facingAngle += pheroSteering * pheromoneSteerStrength;
				ant.facingAngle += wallSteering * wallSteerStrength;

				targetSpeed *= 1f - (math.abs(pheroSteering) + math.abs(wallSteering)) / 3f;

				ant.speed += (targetSpeed - ant.speed) * antAccel;
				float2 targetPos;
				if (ant.holdingResource == false)
				{
					targetPos = resourcePosition;
					antColor.Value += (searchColor * ant.brightness - antColor.Value) * .05f;
				}
				else
				{
					targetPos = colonyPosition;
					antColor.Value += (carryColor * ant.brightness - antColor.Value) * .05f;
				}

				if (Linecast(position.xy, targetPos) == false)
				{
					float targetAngle = math.atan2(targetPos.y - position.y, targetPos.x - position.x);
					if (targetAngle - ant.facingAngle > math.PI)
					{
						ant.facingAngle += math.PI * 2f;
					}
					else if (targetAngle - ant.facingAngle < -math.PI)
					{
						ant.facingAngle -= math.PI * 2f;
					}
					else
					{
						if (math.distance(targetAngle, ant.facingAngle) < math.PI * .5f)
							ant.facingAngle += (targetAngle - ant.facingAngle) * goalSteerStrength;
					}

				}
				if (math.distancesq(position.xy, targetPos) < 4f * 4f)
				{
					ant.holdingResource = !ant.holdingResource;
					ant.facingAngle += math.PI;
				}

				float vx = math.cos(ant.facingAngle) * ant.speed;
				float vy = math.sin(ant.facingAngle) * ant.speed;
				float ovx = vx;
				float ovy = vy;

				if (position.x + vx < 0f || position.x + vx > mapSize)
				{
					vx = -vx;
				}
				else
				{
					position.x += vx;
				}
				if (position.y + vy < 0f || position.y + vy > mapSize)
				{
					vy = -vy;
				}
				else
				{
					position.y += vy;
				}
				float2 delta;
				float dist;

				for (int j = 0; j < nearbyObstacles.Length; j++)
				{
					var obstacle = nearbyObstacles[j];
					delta = position.xy - obstacle.Position;
					float sqrDist = math.lengthsq(delta);
					if (sqrDist < obstacleRadius * obstacleRadius)
					{
						dist = math.sqrt(sqrDist);
						delta /= dist;
						position.xy = obstacle.Position + delta * obstacleRadius;
						vx -= delta.x * (delta.x * vx + delta.y * vy) * 1.5f;
						vy -= delta.y * (delta.x * vx + delta.y * vy) * 1.5f;
					}
				}
				nearbyObstacles.Clear();
				float inwardOrOutward = -outwardStrength;
				float pushRadius = mapSize * .4f;
				if (ant.holdingResource)
				{
					inwardOrOutward = inwardStrength;
					pushRadius = mapSize;
				}
				delta = colonyPosition - position.xy;
				dist = math.length(delta);
				inwardOrOutward *= 1f - math.saturate(dist / pushRadius);
				vx += delta.x / dist * inwardOrOutward;
				vy += delta.y / dist * inwardOrOutward;

				if (ovx != vx || ovy != vy)
				{
					ant.facingAngle = math.atan2(vy, vx);
				}
				float excitement = .3f;
				if (ant.holdingResource)
				{
					excitement = 1f;
				}
				excitement *= ant.speed / antSpeed;

				excitements[entityInBatchIndex] = new Excitement() { Value = excitement };

				rotation.Value = quaternion.AxisAngle(math.forward(), ant.facingAngle);

				ants[entityInBatchIndex] = ant;
				translations[entityInBatchIndex] = new Translation() { Value = position };
				rotations[entityInBatchIndex] = rotation;
				antColors[entityInBatchIndex] = antColor;

				int PheromoneIndex(int x, int y)
				{
					return x + y * mapSize;
				}
				bool Linecast(float2 point1, float2 point2)
				{
					return collisionWorld.CastRay(new RaycastInput()
					{
						Start = new float3(point1, 0),
						End = new float3(point2, 0),
						Filter = new CollisionFilter()
						{
							BelongsTo = AntLayerMask,
							CollidesWith = ObstacleLayerMask,
							GroupIndex = 0,
						}
					});
				}
				
			}
		}
    }
}

