using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;


[RequiresEntityConversion]
public class BoardAuthoring_FromEntity : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject TilePrefabWhite;
    public GameObject TilePrefabBlack;
    public GameObject WallPrefab;
    public GameObject HomebasePrefabR;
    public GameObject HomebasePrefabG;
    public GameObject HomebasePrefabB;
    public GameObject HomebasePrefabY;

    public float yNoise;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(TilePrefabWhite);
        referencedPrefabs.Add(TilePrefabBlack);
        referencedPrefabs.Add(WallPrefab);
        referencedPrefabs.Add(HomebasePrefabR);
        referencedPrefabs.Add(HomebasePrefabG);
        referencedPrefabs.Add(HomebasePrefabB);
        referencedPrefabs.Add(HomebasePrefabY);

    }

    //private void 

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // Board Generation
        Debug.Log("generating board");

        // Grab entity prefabs
        Entity WallEntity = conversionSystem.GetPrimaryEntity(WallPrefab);
        Entity TileEntityWhite = conversionSystem.GetPrimaryEntity(TilePrefabWhite);
        Entity TileEntityBlack = conversionSystem.GetPrimaryEntity(TilePrefabBlack);

        Entity HomebaseEntityR = conversionSystem.GetPrimaryEntity(HomebasePrefabR);
        Entity HomebaseEntityG = conversionSystem.GetPrimaryEntity(HomebasePrefabG);
        Entity HomebaseEntityB = conversionSystem.GetPrimaryEntity(HomebasePrefabB);
        Entity HomebaseEntityY = conversionSystem.GetPrimaryEntity(HomebasePrefabY);

        BoardSystem boardSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BoardSystem>();
        Vector2 CellSize = boardSystem.Board.CellSize;

        for (int i = 0; i < Board.k_Width; i++)
        {
            for(int j = 0; j < Board.k_Height; j++)
            {

                Tile currTile = boardSystem.Board[i, j];

                switch(currTile.TileType)
                {
                    case eTileType.Blank:
                        {
                            Entity tile;
                            if ((i + j * Board.k_Width) % 2 ==0)
                            {
                                tile = dstManager.Instantiate(TileEntityWhite);
                            }
                            else
                            {
                                tile = dstManager.Instantiate(TileEntityBlack);
                            }
                            dstManager.SetComponentData(tile, new Translation { Value = new Vector3(i * CellSize.x, Random.value * yNoise, j * CellSize.y) });
                            break;
                        }
                    case eTileType.Hole:
                        break;
                    case eTileType.HomeBase:
                        {
                            Vector3 spawnTrans = new Vector3(i * CellSize.x, Random.value * yNoise, j * CellSize.y);
                            Entity tile;
                            if ((i + j * Board.k_Width) % 2 ==0)
                            {
                                tile = dstManager.Instantiate(TileEntityWhite);
                            }
                            else
                            {
                                tile = dstManager.Instantiate(TileEntityBlack);
                            }
                            Entity homebase;
                            eColor color = currTile.Color;
                            switch(color)
                            {
                                case eColor.Red:
                                    homebase = dstManager.Instantiate(HomebaseEntityR);
                                    break;
                                case eColor.Green:
                                    homebase = dstManager.Instantiate(HomebaseEntityG);
                                    break;
                                case eColor.Blue:
                                    homebase = dstManager.Instantiate(HomebaseEntityB);
                                    break;
                                case eColor.Black:
                                    homebase = dstManager.Instantiate(HomebaseEntityY);
                                    break;
                                default:
                                    homebase = dstManager.Instantiate(HomebaseEntityR);
                                    break;
                                }
                            dstManager.SetComponentData(tile, new Translation { Value = spawnTrans });
                            dstManager.SetComponentData(homebase, new Translation { Value = spawnTrans + new Vector3(0, 0.5f, 0) });
                            break;
                        }
                    default:
                        break;
                }

                // Spawn wall logic
                if(currTile.HasWall(eDirection.East))
                {
                    Entity wall = dstManager.Instantiate(WallEntity);
                    Quaternion rot = Quaternion.Euler(0, 0, 0);
                    dstManager.SetComponentData(wall, new Translation { Value = new Vector3(i * CellSize.x + 0.5f , 0.7f , j*CellSize.y) });
                    dstManager.SetComponentData(wall, new Rotation { Value = rot });
                }
                if (currTile.HasWall(eDirection.West))
                {
                    Entity wall = dstManager.Instantiate(WallEntity);
                    Quaternion rot = Quaternion.Euler(0, 0, 0);
                    dstManager.SetComponentData(wall, new Translation { Value = new Vector3(i * CellSize.x - 0.5f , 0.7f, j * CellSize.y) });
                    dstManager.SetComponentData(wall, new Rotation { Value = rot });
                }
                if (currTile.HasWall(eDirection.North))
                {
                    Entity wall = dstManager.Instantiate(WallEntity);
                    Quaternion rot = Quaternion.Euler(0, 90, 0);
                    dstManager.SetComponentData(wall, new Translation { Value = new Vector3(i * CellSize.x, 0.7f, j * CellSize.y + 0.5f) });
                    dstManager.SetComponentData(wall, new Rotation { Value = rot });
                }

                if (currTile.HasWall(eDirection.South))
                {
                    Entity wall = dstManager.Instantiate(WallEntity);
                    Quaternion rot = Quaternion.Euler(0, 90, 0);
                    dstManager.SetComponentData(wall, new Translation { Value = new Vector3(i * CellSize.x, 0.7f, j * CellSize.y - 0.5f) });
                    dstManager.SetComponentData(wall, new Rotation { Value = rot });
                    //dstManager.GetComponentData<RenderMesh>(wall);
                }
            }
        }
    }
}
