
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


namespace JumpTheGun
{

	public class Options : MonoBehaviour
	{
		static public EntityQueryDesc QueryBoard => new EntityQueryDesc()
		{
			All = new ComponentType[]
			{
				ComponentType.ReadWrite<Board>()
			},
		};

		static public EntityQueryDesc QueryPlayer => new EntityQueryDesc()
		{
			All = new ComponentType[]
			{
				ComponentType.ReadWrite<Player>()
			},
		};

		float initTerrainWidth;
		float initTerrainLength;
		float initMinTerrainHeight;
		float initMaxTerrainHeight;
		float initBulletSpeed;
		float initBoxHeightDamage;
		float initNumTanks;
		float initTankLaunchPeriod;

		[Header("Scene")]
		public GameObject optionsMenu;

		[Header("Children")]
		public SliderProp terrainWidth;
		public SliderProp terrainLength;
		public SliderProp minTerrainHeight;
		public SliderProp maxTerrainHeight;


		public SliderProp bulletSpeed;
		public SliderProp boxHeightDamage;
		public SliderProp numTanks;

		public SliderProp tankLaunchPeriod;


		public void UpdateBoard()
		{
			UpdateSliders();

			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			var query = entityManager.CreateEntityQuery(QueryBoard);

			using var boards = query.ToEntityArray(Allocator.TempJob);
			var board = boards.SingleOrDefault();
			if (board == Entity.Null)
				return;

			entityManager.AddComponent<BoardSpawnerTag>(board);

			query = entityManager.CreateEntityQuery(QueryPlayer);

			using var players = query.ToEntityArray(Allocator.TempJob);
			var player = boards.SingleOrDefault();
			if (player == Entity.Null)
				return;

			entityManager.AddComponent<PlayerSpawnerTag>(player);
		}

		void Start()
		{

			terrainWidth.SetBounds(10, 1000);
			terrainWidth.value = initTerrainWidth = 100;

			terrainLength.SetBounds(10, 1000);
			terrainLength.value = initTerrainLength = 100;

			minTerrainHeight.SetBounds(0.5F, 20F);
			minTerrainHeight.value = initMinTerrainHeight = 2.5F;

			maxTerrainHeight.SetBounds(0.5F, 20F);
			maxTerrainHeight.value = initMaxTerrainHeight = 5.5F;

			boxHeightDamage.SetBounds(0.1F, 5F);
			boxHeightDamage.value = 0.4F;

			numTanks.SetBounds(0, 10000);
			numTanks.value = 100;

			tankLaunchPeriod.SetBounds(0.1F, 20F);
			tankLaunchPeriod.value = 4F;

			bulletSpeed.SetBounds(0.1F, 10F);
			bulletSpeed.value = 2F;

			optionsMenu.SetActive(false);

			UpdateSliders();
		}

		void UpdateSliders()
		{
			var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			var query = entityManager.CreateEntityQuery(QueryBoard);

            using var entities = query.ToEntityArray(Allocator.TempJob);
            var entity = entities.SingleOrDefault();
            if (entity == Entity.Null)
                return;

            var data = entityManager.GetComponentData<BoardSpawnerData>(entity);

			initTerrainWidth = terrainWidth.value;
			initTerrainLength = terrainLength.value;
			initMinTerrainHeight = minTerrainHeight.value;
			initMaxTerrainHeight = maxTerrainHeight.value;

			initBulletSpeed = bulletSpeed.value;
			initBoxHeightDamage = boxHeightDamage.value;
			initNumTanks = numTanks.value;
			initTankLaunchPeriod = tankLaunchPeriod.value;

			data.SizeX = Mathf.RoundToInt(terrainWidth.value);
            data.SizeY = Mathf.RoundToInt(terrainLength.value);
            data.MinHeight = minTerrainHeight.value;
            data.MaxHeight = maxTerrainHeight.value;

            data.HitStrength = boxHeightDamage.value;
            data.NumberOfTanks = Mathf.RoundToInt(numTanks.value);
            data.ReloadTime = tankLaunchPeriod.value;
            data.BulletSpeed = bulletSpeed.value;

			entityManager.SetComponentData(entity, data);

            terrainWidth.SetText($"Terrain Width: {data.SizeX}");
            terrainLength.SetText($"Terrain Length: {data.SizeY}");

            minTerrainHeight.SetText($"Min Terrain Height: {data.MinHeight}");
            maxTerrainHeight.SetText($"Max Terrain Height: {data.MaxHeight}");

            boxHeightDamage.SetText($"Terrain Damage: {data.HitStrength}");

            numTanks.SetText($"Tanks Amount: {data.NumberOfTanks}");

            tankLaunchPeriod.SetText($"Tank Reload Time: {data.ReloadTime}");

            bulletSpeed.SetText($"Bullet Speed: {data.BulletSpeed}");
        }


		// Update is called once per frame
		void Update()
		{
			if (initTerrainWidth == terrainWidth.value &&
				initTerrainLength == terrainLength.value &&
				initMinTerrainHeight == minTerrainHeight.value &&
				initMaxTerrainHeight == maxTerrainHeight.value &&
			    initBulletSpeed == bulletSpeed.value &&
			    initBoxHeightDamage == boxHeightDamage.value &&
			    initNumTanks == numTanks.value &&
			    initTankLaunchPeriod == tankLaunchPeriod.value)

				return;

			UpdateBoard();
        }
	}

}