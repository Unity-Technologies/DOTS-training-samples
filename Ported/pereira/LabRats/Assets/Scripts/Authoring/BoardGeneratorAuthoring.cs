using System.Collections.Generic;
using System.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// ReSharper disable once InconsistentNaming
[RequiresEntityConversion]
public class BoardGeneratorAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public Vector2Int Size;
    public float YNoise = 0.05f;

    public GameObject CellPrefab;
    public GameObject WallPrefab;

    public GameObject Player1HombasePrefab;
    public GameObject Player2HombasePrefab;
    public GameObject Player3HombasePrefab;
    public GameObject Player4HombasePrefab;

    public GameObject SpawnerPrefab;
    public int AdditinalSpawners;

    public GameObject Player1CursorPrefab;
    public GameObject Player2CursorPrefab;
    public GameObject Player3CursorPrefab;
    public GameObject Player4CursorPrefab;

    // The MonoBehaviour data is converted to ComponentData on the entity.
    // We are specifically transforming from a good editor representation of the data (Represented in degrees)
    // To a good runtime representation (Represented in radians)
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var generator = new LbBoardGenerator()
        {
            SizeX = (byte)Size.x,
            SizeY = (byte)Size.y,
            YNoise = YNoise,

            CellPrefab = conversionSystem.GetPrimaryEntity(CellPrefab),
            WallPrefab = conversionSystem.GetPrimaryEntity(WallPrefab),

            Player1Homebase = conversionSystem.GetPrimaryEntity(Player1HombasePrefab),
            Player2Homebase = conversionSystem.GetPrimaryEntity(Player2HombasePrefab),
            Player3Homebase = conversionSystem.GetPrimaryEntity(Player3HombasePrefab),
            Player4Homebase = conversionSystem.GetPrimaryEntity(Player4HombasePrefab),

            SpawnerPrefab = conversionSystem.GetPrimaryEntity(SpawnerPrefab),
            AdditionalSpawners = AdditinalSpawners,

            Player1Cursor = conversionSystem.GetPrimaryEntity(Player1CursorPrefab),
            Player2Cursor = conversionSystem.GetPrimaryEntity(Player2CursorPrefab),
            Player3Cursor = conversionSystem.GetPrimaryEntity(Player3CursorPrefab),
            Player4Cursor = conversionSystem.GetPrimaryEntity(Player4CursorPrefab),
        };
        dstManager.AddComponentData(entity, generator);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(CellPrefab);
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(Player1HombasePrefab);
        referencedPrefabs.Add(Player2HombasePrefab);
        referencedPrefabs.Add(Player3HombasePrefab);
        referencedPrefabs.Add(Player4HombasePrefab);
        referencedPrefabs.Add(SpawnerPrefab);
        referencedPrefabs.Add(Player1CursorPrefab);
        referencedPrefabs.Add(Player2CursorPrefab);
        referencedPrefabs.Add(Player3CursorPrefab);
        referencedPrefabs.Add(Player4CursorPrefab);
    }
}
