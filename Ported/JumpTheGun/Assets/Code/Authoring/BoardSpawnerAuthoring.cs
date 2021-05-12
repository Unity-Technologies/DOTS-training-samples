
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class BoardSpawnerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    [Range(1, 1000)]
    public int SizeX;

    [Range(1, 1000)]
    public int SizeY;

    [Range(0.5F, 20F)]
    public float MinHeight;

    [Range(0.5F, 20F)]
    public float MaxHeight;

    [Range(0.1F, 20F)]
    public float ReloadTime;

    [Range(0.1F, 20F)]
    public float Radius;

    [Range(1, 10000)]
    public int NumberOfTanks;

    [Range(0.1F, 5F)]
    public float HitStrength;
    
    public GameObject PlaformPrefab;
    public GameObject TankPrefab;
    public GameObject TurretPrefab;
    public GameObject BulletPrefab;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PlaformPrefab);
        referencedPrefabs.Add(TankPrefab);
        referencedPrefabs.Add(TurretPrefab);
        referencedPrefabs.Add(BulletPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BoardSpawnerData
        {
            SizeX = SizeX,
            SizeY = SizeY,
            MinHeight = MinHeight,
            MaxHeight = MaxHeight,
            ReloadTime = ReloadTime,
            Radius = Radius,
            NumberOfTanks = NumberOfTanks,
            HitStrength = HitStrength,
            
            PlaformPrefab = conversionSystem.GetPrimaryEntity(PlaformPrefab),
            TankPrefab = conversionSystem.GetPrimaryEntity(TankPrefab),
            TurretPrefab = conversionSystem.GetPrimaryEntity(TurretPrefab),
            BulletPrefab = conversionSystem.GetPrimaryEntity(BulletPrefab)
        });

        dstManager.AddComponent<Board>(entity);
        dstManager.AddComponent<BoardSpawnerTag>(entity);
    }
}
