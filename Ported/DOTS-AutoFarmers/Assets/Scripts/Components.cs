using Unity.Animation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using AnimationCurve = Unity.Animation.AnimationCurve;
namespace AutoFarmers
{
	
	public struct Farm : IComponentData
	{
		public const int DefaultMapSize = 60;

		public int2 MapSize;
		public int SeedOffset;
		public Entity PlantPrefab;
		public Entity GroundTilePrefab;
		public Entity RockPrefab;
		public Entity StorePrefab;
		public AnimationCurve SoldPlantYCurve;
		public AnimationCurve SoldPlantXZScaleCurve;
		public AnimationCurve SoldPlantYScaleCurve;
		[InternalBufferCapacity(0)]
        public struct GroundTiles :IBufferElementData
        {
			public Entity Value;
			public static implicit operator GroundTiles(Entity tileEntity) => new GroundTiles() { Value = tileEntity};
        }

        public struct RockSpawnAttempts : IComponentData
        {
			public int Value;
        }

        public struct StoreCount : IComponentData
        {
			public int Value;
        }

		public struct IsHarvestable : ITilePredicate
		{
			[ReadOnly] ComponentDataFromEntity<Plant.Growth> plantGrowthFromEntity;
            public IsHarvestable(ComponentDataFromEntity<Plant.Growth> plantGrowthFromEntity = default)
            {
                this.plantGrowthFromEntity = plantGrowthFromEntity;
            }

            public bool Predicate(GroundTile tile)
			{
				if (tile.TilePlant == Entity.Null)
				{
					return false;
				}
				var growth = plantGrowthFromEntity[tile.TilePlant].Value;
				return growth >= 1f;
			}
		}

		public struct IsHarvestableAndUnreserved : ITilePredicate
		{
			[ReadOnly] ComponentDataFromEntity<Plant> plantFromEntity;
			[ReadOnly] ComponentDataFromEntity<Plant.Growth> plantGrowthFromEntity;
            public IsHarvestableAndUnreserved(ComponentDataFromEntity<Plant> plantFromEntity, ComponentDataFromEntity<Plant.Growth> plantGrowthFromEntity)
            {
                this.plantFromEntity = plantFromEntity;
                this.plantGrowthFromEntity = plantGrowthFromEntity;
            }

            public bool Predicate(GroundTile tile)
			{
				if (tile.TilePlant == Entity.Null)
				{
					return false;
				}
				var plant = plantFromEntity[tile.TilePlant];
				var growth = plantGrowthFromEntity[tile.TilePlant].Value;
				return growth >= 1f & !plant.reserved;
			}
		}
	}
	[InternalBufferCapacity(Plant.Variants)]
    public struct PlantPrefabs : IBufferElementData
    {
		public Entity Value;
    }
    public struct Money : IComponentData
    {
		public int MoneyForFarmers;
		public int MoneyForDrones;
	}
    public struct Rock : IComponentData
    {
		public RectInt Rect;
		public int StartHealth;
		public int Health;
		public float Depth;
    }

    public struct GroundTile : IComponentData
    {
		public int2 Position;
		public Entity TileRock;
		public Entity TilePlant;
		public GroundState State;
		public bool StoreTile;
		[MaterialProperty("_Tilled",MaterialPropertyFormat.Float)]
        public struct Tilled : IComponentData
        {
			public float Value;
        }
		
	}

	public interface ITilePredicate
	{
		public bool Predicate(GroundTile tile);
	}
	
	public struct Plant : IComponentData
    {
		public const int Variants = 10;
		public int2 Position;
		public bool reserved;
		public bool harvested;
		[MaterialProperty("_Growth", MaterialPropertyFormat.Float)]
		public struct Growth : IComponentData
        {
			public float Value;
        }
        public struct Sold : IComponentData
        {
			public float ElapsedTime;
        }
	}

    public struct Store : IComponentData
    {

    }

	public struct Farmer : IComponentData
	{
		public float2 Position;
	}

    public struct Drone : IComponentData
    {
		internal const float ySpeed = 2f;
		internal const float xzSpeed = 6f;
		public float3 Position;
		public float HoverHeight;
		public float SearchTimer;
        public struct TargetPlant :IComponentData
        {
			public Entity Value;
        }

		public struct SellPlant : IComponentData
		{
            public struct StorePosition : IComponentData
            {
				public int2 Value;
            }
			public Entity HeldPlant;
		}
	}

    public struct DroneGlobalParams : IComponentData
    {
		public Entity DronePrefab;
		public int MaxDroneCount;
		public float MoveSmooth;
		public float CarrySmooth;
		public int DroneCount;
    }

	public static class Intension
	{
		[WriteGroup(typeof(Path))]
		public struct SmashRocks : IComponentData
		{
			public Entity TargetRock;
			public bool AttackingARock;
		}
		[WriteGroup(typeof(Path))]
		public struct TillGround : IComponentData
		{
			[WriteGroup(typeof(Path))]
			public struct FoundTillingZone : IComponentData
            {
				public RectInt TillingZone;
			}
		}
		[WriteGroup(typeof(Path))]
		public struct PlantSeeds : IComponentData
		{
			[WriteGroup(typeof(Path))]
			public struct HasBoughtSeeds : IComponentData
            {
                public struct SpawnPlant : IComponentData
                {
					public int Seed;
                }
            }
		}
		[WriteGroup(typeof(Path))]
		public struct SellPlants : IComponentData
		{
			[WriteGroup(typeof(Path))]
			public struct HeldPlant : IComponentData
            {
				public Entity Plant;
            }
		}
	}
	public struct FarmerGlobalParams : IComponentData
	{
		public Entity FarmerPrefab;
		public float MovementSmooth;
		public int MaxFarmerCount;
		public int FarmerCount;
	}

    public struct SpawnFarmer : IComponentData
    {
		public int Count;
    }
	[InternalBufferCapacity(32)]
	public struct Path : IBufferElementData
	{
		public int2 Position;
		public static implicit operator Path(int2 position)=>new Path() { Position = position };
	}
	
}