
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
			var query = entityManager.CreateEntityQuery(QueryPlayer);

			using var players = query.ToEntityArray(Allocator.TempJob);
			var player = players.SingleOrDefault();
			if (player == Entity.Null)
				return;

			if (!entityManager.HasComponent<Reset>(player))
				entityManager.AddComponent<Reset>(player);
		}

		void Start()
		{

			terrainWidth.SetBounds(10, 1000);
			terrainWidth.value = initTerrainWidth = 100;
			terrainWidth.SetText($"Terrain Width: {initTerrainWidth}");

			terrainLength.SetBounds(10, 1000);
			terrainLength.value = initTerrainLength = 100;
			terrainLength.SetText($"Terrain Length: {initTerrainLength}");

			minTerrainHeight.SetBounds(0.5F, 20F);
			minTerrainHeight.value = initMinTerrainHeight = 2.5F;
			minTerrainHeight.SetText($"Min Terrain Height: {initMinTerrainHeight}");

			maxTerrainHeight.SetBounds(0.5F, 20F);
			maxTerrainHeight.value = initMaxTerrainHeight = 5.5F;
			maxTerrainHeight.SetText($"Max Terrain Height: {initMaxTerrainHeight}");

			boxHeightDamage.SetBounds(0.1F, 5F);
			boxHeightDamage.value = initBoxHeightDamage = 0.4F;
			boxHeightDamage.SetText($"Terrain Damage: {initBoxHeightDamage}");

			numTanks.SetBounds(0, 10000);
			numTanks.value = initNumTanks = 100;
			numTanks.SetText($"Tanks Amount: {initNumTanks}");

			tankLaunchPeriod.SetBounds(0.1F, 20F);
			tankLaunchPeriod.value = initTankLaunchPeriod = 4F;
			tankLaunchPeriod.SetText($"Tank Reload Time: {initTankLaunchPeriod}");

			bulletSpeed.SetBounds(0.1F, 10F);
			bulletSpeed.value = initBulletSpeed = 2F;
			bulletSpeed.SetText($"Bullet Speed: {initBulletSpeed}");

			UpdateSliders();

			optionsMenu.SetActive(false);
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