using Unity.Entities;
using Unity.Mathematics;

public class FarmMoneyAuthoring : UnityEngine.MonoBehaviour
{
    public int InitialDroneMoney;
    public int InitialFarmerMoney;
}

public class FarmMoneyBaker : Baker<FarmMoneyAuthoring>
{
    public override void Bake(FarmMoneyAuthoring authoring)
    {
        AddComponent(new FarmMoney
        {
            DroneMoney = authoring.InitialDroneMoney,
            FarmerMoney = authoring.InitialFarmerMoney,
            SpawnedDrones = 0,
            SpawnedFarmers=0,
        });
    }
}