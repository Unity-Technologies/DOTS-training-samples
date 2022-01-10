using Unity.Entities;

public struct FarmerConfig : IComponentData
{
	public Entity FarmerPrefab;
	public int MoneyForFarmers;
	public int InitialFarmerCount;
	public int MaxFarmerCount;
	public int FarmerRange;

	public float WalkSpeed;
	public float MoveSmooth;
}
