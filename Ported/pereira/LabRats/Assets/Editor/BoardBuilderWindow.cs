//

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using Unity.Entities;

public class BoardBuilderWindow : EditorWindow
{
    private const string kSizeXName = "Editor.SizeX";
    private const string kSizeYName = "Editor.SizeY";
    private const string kYNoiseName = "Editor.YNoise";
    private const string kRandomSeedName = "Editor.RandomSeed";

    private const string kCellName = "Editor.CellPrefab";
    private const string kWallName = "Editor.WallPrefab";
    private const string kSpawnerName = "Editor.SpawnerPrefab";

    private const string kHomebaseName1 = "Editor.HomeBase1";
    private const string kHomebaseName2 = "Editor.HomeBase2";
    private const string kHomebaseName3 = "Editor.HomeBase3";
    private const string kHomebaseName4 = "Editor.HomeBase4";

    private const string kNoHoles = "Editor.NoHoles";
    private const string kNoDefaultSpawners = "Editor.NoDefaultSpawners";
    private const string kAdditionalSpawners = "Editor.AditionalSpawners";

    private int m_SizeX = 100;
    private int m_SizeY = 100;
    private float m_YNoise = 0.05f;
    private int m_RandomSeed = -1;

    private bool m_NoHoles = false;
    private bool m_NoDefaultSpawners = false;
    private int m_AdditionalSpawners = 0;

    private GameObject m_CellPrefab = null;
    private GameObject m_WallPrefab = null;

    private GameObject m_SpawnerPrefab = null;

    private GameObject m_Homebase1 = null;
    private GameObject m_Homebase2 = null;
    private GameObject m_Homebase3 = null;
    private GameObject m_Homebase4 = null;

    private bool m_IsGenerating = false;
    private int m_GeneratingProgress = 0;

    // Add menu named "My Window" to the Window menu
    [MenuItem("LabRats/Board Builder")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BoardBuilderWindow window = (BoardBuilderWindow)EditorWindow.GetWindow(typeof(BoardBuilderWindow));
        window.Show();
    }

    /// <summary>
    /// Called when activated
    /// </summary>
    protected void OnEnable()
    {
        LoadAll();
    }

    /// <summary>
    /// Draw the window
    /// </summary>
    protected void OnGUI()
    {
        GUILayout.Label("Board Builder");

        GUILayout.Space(5.0f);
        GUILayout.Label("BOARD");

        IntField(kSizeXName, "Size X:", ref m_SizeX);
        IntField(kSizeYName, "Size Y:", ref m_SizeY);
        FloatField(kYNoiseName, "Y Noise:", ref m_YNoise);

        GUILayout.Space(5.0f);
        GUILayout.Label("RANDOM");

        IntField(kRandomSeedName, "Random Seed:", ref m_RandomSeed);
        BoolField(kNoDefaultSpawners, "No Default Spawners:", ref m_NoDefaultSpawners);
        IntField(kAdditionalSpawners, "Additional Spawners:", ref m_AdditionalSpawners);
        BoolField(kNoHoles, "No Holes:", ref m_NoHoles);

        GUILayout.Space(5.0f);
        GUILayout.Label("PREFABS");

        GUILayout.Space(3.0f);
        GUILayout.Label("Board");
        ObjectField(kCellName, "Cell Prefab", ref m_CellPrefab);
        ObjectField(kWallName, "Wall Prefab", ref m_WallPrefab);
        ObjectField(kSpawnerName, "Spawner Prefab", ref m_SpawnerPrefab);

        GUILayout.Space(3.0f);
        GUILayout.Label("Home bases");
        ObjectField(kHomebaseName1, "Homebase1 Prefab", ref m_Homebase1);
        ObjectField(kHomebaseName2, "Homebase2 Prefab", ref m_Homebase2);
        ObjectField(kHomebaseName3, "Homebase3 Prefab", ref m_Homebase3);
        ObjectField(kHomebaseName4, "Homebase4 Prefab", ref m_Homebase4);

        GUILayout.Space(25.0f);

        if (m_IsGenerating)
        {
            var maxProgress = m_SizeX * m_SizeY;
            GUILayout.Label("Progress: " + m_GeneratingProgress + " / " + maxProgress);

            GUILayout.Space(5.0f);
            if (GUILayout.Button("Stop generation!"))
            {
                StopGenerateBoard();
            }
        }
        else
        {
            if (GUILayout.Button("Generate Board"))
            {
                GenerateBoard();
            }
        }
    }

    /// <summary>
    /// Load all fields
    /// </summary>
    public void LoadAll()
    {
        LoadInt(kSizeXName, ref m_SizeX);
        LoadInt(kSizeYName, ref m_SizeY);
        LoadFloat(kYNoiseName, ref m_YNoise);

        LoadInt(kRandomSeedName, ref m_RandomSeed);
        LoadBool(kNoDefaultSpawners, ref m_NoDefaultSpawners);
        LoadInt(kAdditionalSpawners, ref m_AdditionalSpawners);
        LoadBool(kNoHoles, ref m_NoHoles);

        LoadObject(kCellName, ref m_CellPrefab);
        LoadObject(kWallName, ref m_WallPrefab);
        LoadObject(kSpawnerName, ref m_SpawnerPrefab);

        LoadObject(kHomebaseName1, ref m_Homebase1);
        LoadObject(kHomebaseName2, ref m_Homebase2);
        LoadObject(kHomebaseName3, ref m_Homebase3);
        LoadObject(kHomebaseName4, ref m_Homebase4);
    }

    /// <summary>
    /// Generates the board
    /// </summary>
    private void GenerateBoard()
    {
        if (IsObjectInvalid("Cell", m_CellPrefab)) return;
        if (IsObjectInvalid("Wall", m_WallPrefab)) return;
        if (IsObjectInvalid("Spawner", m_SpawnerPrefab)) return;
        if (IsObjectInvalid("Homebase1", m_Homebase1)) return;
        if (IsObjectInvalid("Homebase2", m_Homebase2)) return;
        if (IsObjectInvalid("Homebase3", m_Homebase3)) return;
        if (IsObjectInvalid("Homebase4", m_Homebase4)) return;


        m_IsGenerating = true;
        m_GeneratingProgress = 0;

        StartCoroutine("GenerateBoardInternal");
    }

    /// <summary>
    /// Stops to generate the board
    /// </summary>
    private void StopGenerateBoard()
    {
        StopCoroutine("GenerateBoardInternal");
        m_IsGenerating = false;
    }

    /// <summary>
    /// Coroutine for generate the board
    /// </summary>
    private IEnumerator GenerateBoardInternal()
    {
        // Get the board object
        var board = FindOrCreateBoardObject();
        board.Size = new Vector2Int(m_SizeX, m_SizeY);

        // Clean up all childrens
        Transform boardTransform = null; //board.transform;
        if (boardTransform != null)
        {
            EditorUtil.DestroyChildren(boardTransform);
        }
        else
        {
            foreach (var cell in FindObjectsOfType<Cell>())
                DestroyImmediate(cell.gameObject);

            foreach (var cell in FindObjectsOfType<Wall>())
                DestroyImmediate(cell.gameObject);

            foreach (var cell in FindObjectsOfType<HomebaseAuthoring>())
                DestroyImmediate(cell.gameObject);

            foreach (var cell in FindObjectsOfType<Spawner_Authoring>())
                DestroyImmediate(cell.gameObject);
        }

        // Generate the board floor and external walls
        for (int z = 0; z < m_SizeY; ++z)
        {
            for (int x = 0; x < m_SizeX; ++x)
            {
                var coord = new Vector2Int(x, z);

                var index = (coord.x + coord.y) % 2 == 0 ? 1 : 0;

                var obj = Instantiate<GameObject>(m_CellPrefab);
                obj.name = "board_" + coord;
                obj.transform.SetParent(boardTransform);

                var cell = obj.GetComponent<Cell>();
                if (cell == null)
                    cell = obj.AddComponent<Cell>();
                cell.location = coord;
                cell.isHole = false;

                // Position the block
                obj.transform.localPosition = new Vector3(
                    coord.x,
                    UnityEngine.Random.value * m_YNoise,
                    coord.y);

                PlaceWall(Directions.North, coord + Vector2Int.up, boardTransform, coord.y == m_SizeY - 1);
                PlaceWall(Directions.East, coord + Vector2Int.right, boardTransform, coord.x == m_SizeX - 1);
                PlaceWall(Directions.South, coord, boardTransform, coord.y == 0);
                PlaceWall(Directions.West, coord, boardTransform, coord.x == 0);

                m_GeneratingProgress++;
            }

            yield return null;
        }


        var oldState = UnityEngine.Random.state;
        if (m_RandomSeed != -1)
            UnityEngine.Random.InitState(m_RandomSeed);

        var wallMap = board.GetWalls();

        // Create random Walls
        int numWalls = (int)(m_SizeX * m_SizeY * 0.2f);
        for (int c = 0; c < numWalls; ++c)
        {
            var location = new Vector2Int(UnityEngine.Random.Range(0, m_SizeX), UnityEngine.Random.Range(0, m_SizeY));
            var direction = GetRandomDirection();

            // Avoid wall outside the board
            if (location.x == 0)
            {
                if (direction == Directions.West || direction == Directions.East)
                    continue;
            }

            if (location.y == 0)
            {
                if (direction == Directions.North || direction == Directions.South)
                    continue;
            }

            if (BoardAuthoring.HasWall(wallMap, location, direction))
                continue;

            int count = 0;
            if (direction != Directions.North && BoardAuthoring.HasWall(wallMap, location, Directions.North))
                count++;
            if (direction != Directions.East && BoardAuthoring.HasWall(wallMap, location, Directions.East))
                count++;
            if (direction != Directions.South && BoardAuthoring.HasWall(wallMap, location, Directions.South))
                count++;
            if (direction != Directions.West && BoardAuthoring.HasWall(wallMap, location, Directions.West))
                count++;

            // Avoid closed cells
            if (count >= 3)
                continue;

            var wall = PlaceWall(direction, location, boardTransform);
            BoardAuthoring.AddWallToDictionary(ref wallMap, wall);
        }

        // Setup home bases
        var offset = 1f / 3f;
        var placeX = m_SizeX * offset;
        var placeY = m_SizeY * offset;

        // Setup holes
        var cellMap = board.GetCellMap();

        PlaceHomebase(Players.Player1, placeX, placeY, boardTransform, cellMap);
        PlaceHomebase(Players.Player2, placeX * 2f, placeY, boardTransform, cellMap);
        PlaceHomebase(Players.Player3, placeX * 2f, placeY * 2f, boardTransform, cellMap);
        PlaceHomebase(Players.Player4, placeX, placeY * 2f, boardTransform, cellMap);

        // Setup spawners
        if (!m_NoDefaultSpawners)
        {
            PlaceSpawner(0, 0, boardTransform, Quaternion.identity, cellMap);
            PlaceSpawner(m_SizeX - 1, 0, boardTransform, Quaternion.Euler(0, 0, 0), cellMap);
            PlaceSpawner(m_SizeX - 1, m_SizeY - 1, boardTransform, Quaternion.identity, cellMap);
            PlaceSpawner(0, m_SizeY - 1, boardTransform, Quaternion.Euler(0, 0, 0), cellMap);
        }

        int additionalSpawners = m_AdditionalSpawners;
        while (additionalSpawners > 0)
        {
            var coord = new Vector2Int(UnityEngine.Random.Range(0, m_SizeX), UnityEngine.Random.Range(0, m_SizeY));
            if (coord.x > 0 && coord.y > 0 && coord.x < m_SizeX - 1 && coord.y < m_SizeY - 1 && cellMap.ContainsKey(coord))
            {
                var cell = cellMap[coord];
                if (cell.hasSpawner || cell.homebase != null)
                    continue;

                PlaceSpawner(coord.x, coord.y, boardTransform, Quaternion.identity, cellMap);
                additionalSpawners--;
            }
        }

        if (!m_NoHoles)
        {
            int numHoles = (int)(m_SizeX * m_SizeY * 0.05f);
            for (int i = 0; i < numHoles; ++i)
            {
                var coord = new Vector2Int(UnityEngine.Random.Range(0, m_SizeX), UnityEngine.Random.Range(0, m_SizeY));
                if (coord.x > 0 && coord.y > 0 && coord.x < m_SizeX - 1 && coord.y < m_SizeY - 1 && cellMap.ContainsKey(coord))
                {
                    var cell = cellMap[coord];
                    if (cell.isHole || cell.hasSpawner || cell.homebase != null)
                    {
                        continue;
                    }

                    var holeObject = new GameObject();
                    holeObject.name = "hole_" + coord;
                    holeObject.transform.SetParent(boardTransform);
                    holeObject.transform.localPosition = cell.transform.localPosition;

                    var holeCell = holeObject.AddComponent<Cell>();
                    holeCell.location = coord;
                    holeCell.isHole = true;

                    cellMap[coord] = holeCell;
                    DestroyImmediate(cell.gameObject);
                }
            }
        }

        UnityEngine.Random.state = oldState;

        Debug.Log("Board Generated!");
        m_IsGenerating = false;
    }

    /// <summary>
    /// Place a spawner in the world
    /// </summary>
    private void PlaceSpawner(int X, int Y, Transform parent, Quaternion quaternion, Dictionary<Vector2Int, Cell> cellMap)
    {
        var location = new Vector2Int(X, Y);

        var obj = Instantiate(m_SpawnerPrefab, Vector3.zero, quaternion, parent);
        obj.name = "spawner_" + location;

        var center = new Vector3(
            location.x,
            0.75f,                   // Change when we have a height variable
            location.y);

        obj.transform.localPosition = center;
        obj.transform.SetParent(parent);

        if (cellMap.ContainsKey(location))
            cellMap[location].hasSpawner = true;
    }

    /// <summary>
    /// Place a homebase for the given player in the location
    /// </summary>
    private void PlaceHomebase(Players player, float X, float Y, Transform parent, Dictionary<Vector2Int, Cell> cellMap)
    {
        var location = new Vector2Int((int)X, (int)Y);
        var prefab = GetHomebasePrefab(player);

        var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        obj.name = "homebase_" + player;

        var center = new Vector3(
            location.x,             
            0.0f,                   // Change when we have a height variable
            location.y);

        obj.transform.localPosition = center;
        obj.transform.SetParent(parent);

        if (cellMap.ContainsKey(location))
            cellMap[location].homebase = obj;
    }

    /// <summary>
    /// Get the homebse prefab for the player
    /// </summary>
    private GameObject GetHomebasePrefab(Players player)
    {
        switch(player)
        {
            case Players.Player1:
                return m_Homebase1;

            case Players.Player2:
                return m_Homebase2;

            case Players.Player3:
                return m_Homebase3;

            case Players.Player4:
                return m_Homebase4;

            default:
                throw new Exception("Invalid player!");
        }
    }

    /// <summary>
    /// Get a random direction 
    /// </summary>
    /// <returns></returns>
    private Directions GetRandomDirection()
    {
        var dirNumber = UnityEngine.Random.Range(0, 4);
        switch (dirNumber)
        {
            case 0:
                return Directions.North;

            case 1:
                return Directions.East;

            case 2:
                return Directions.South;

            default:
                return Directions.West;
        }
    }

    /// <summary>
    /// Find or create the board object
    /// </summary>
    /// <returns></returns>
    private BoardAuthoring FindOrCreateBoardObject()
    {
        var obj = GameObject.Find("Board");
        if (obj == null)
        {
            obj = new GameObject();
            obj.name = "Board";
        }

        var board = obj.GetComponent<BoardAuthoring>();
        if (board == null)
        {
            board = obj.AddComponent<BoardAuthoring>();
        }

        if (obj.GetComponent<ConvertToEntity>() == null)
        {
            obj.AddComponent<ConvertToEntity>();
        }

        return board;
    }

    /// <summary>
    /// Place a wall in the given coord with the given direction
    /// </summary>
    /// <param name="place">True to spawn the wall false to skip spawning</param>
    private Wall PlaceWall(Directions direction, Vector2Int coord, Transform parent, bool place = true)
    {
        if (!place)
            return null;

        var obj = Instantiate(m_WallPrefab, Vector3.zero, Quaternion.identity, parent);
        obj.name = "wall_" + coord;

        var wall = obj.GetComponent<Wall>();
        if (wall == null)
            wall = obj.AddComponent<Wall>();
        wall.location = coord;

        var halfBoardWidth = 0.5f;
        var halfWallWidth = 0.025f;
        

        var center = new Vector3(
            coord.x,
            0.75f,                         // Change when we have a height variable
            coord.y);

        Vector3 offset = Vector3.zero;
        switch(direction)
        {
            case Directions.North:
                offset = new Vector3(0.0f, 0.0f, -1.0f * (halfBoardWidth + halfWallWidth));
                break;

            case Directions.East:
                offset = new Vector3(-1.0f * (halfWallWidth + halfBoardWidth), 0.0f, 0.0f);
                break;

            case Directions.West:
                offset = new Vector3(halfWallWidth- halfBoardWidth, 0.0f, 0.0f);
                break;

            case Directions.South:
                offset = new Vector3(0.0f, 0.0f, halfWallWidth - halfBoardWidth);
                break;
        }

        if (direction == Directions.North || direction == Directions.South)
        {
            obj.transform.Rotate(0, 90f, 0);
            wall.isHorizontal = true;
        }
        else
        {
            wall.isHorizontal = false;
        }

        obj.transform.localPosition = center + offset;
        obj.transform.SetParent(parent);

        return wall;
    }

    #region GUI_HELPERS
    /// <summary>
    /// Start a coroutine
    /// </summary>
    private void StartCoroutine(string coroutineName)
    {
        marijnz.EditorCoroutines.EditorCoroutines.StartCoroutine(coroutineName, this);
    }

    /// <summary>
    /// Stop a coroutine
    /// </summary>
    private void StopCoroutine(string coroutineName)
    {
        marijnz.EditorCoroutines.EditorCoroutines.StopCoroutine(coroutineName, this);
    }

    /// <summary>
    /// Check if the object is null and print an error message
    /// </summary>
    private bool IsObjectInvalid(string objName, GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("Cannot generate Board: " + objName + " prefab is null!");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Help to change a float field and serialize it to disk
    /// </summary>
    private void FloatField(string key, string label, ref float number)
    {
        var n = EditorGUILayout.DelayedFloatField(label, number);
        if (Math.Abs(n - number) > 0.01f)
        {
            PlayerPrefs.SetFloat(key, n);
            number = n;
        }
    }

    /// <summary>
    /// Helper to load a float field
    /// </summary>
    private void LoadFloat(string key, ref float number)
    {
        if (PlayerPrefs.HasKey(key))
        {
            number = PlayerPrefs.GetFloat(key);
        }
    }

    /// <summary>
    /// Helper to change a bool field and serialize it to disk
    /// </summary>
    private void BoolField(string key, string label, ref bool value)
    {
        var b = EditorGUILayout.Toggle(label, value);
        if (b != value)
        {
            PlayerPrefs.SetInt(key, b ? 1 : 0);
            value = b;
        }
    }

    /// <summary>
    /// Load a flag
    /// </summary>
    private void LoadBool(string key, ref bool value)
    {
        if (PlayerPrefs.HasKey(key))
            value = PlayerPrefs.GetInt(key) == 1;
    }

    /// <summary>
    /// Help to change a float field and serialize it to disk
    /// </summary>
    private void IntField(string key, string label, ref int number)
    {
        var n = EditorGUILayout.DelayedIntField(label, number);
        if (n != number)
        {
            PlayerPrefs.SetInt(key, n);
            number = n;
        }
    }

    /// <summary>
    /// Helper to load a float field
    /// </summary>
    private void LoadInt(string key, ref int number)
    {
        if (PlayerPrefs.HasKey(key))
        {
            number = PlayerPrefs.GetInt(key);
        }
    }
    
    /// <summary>
    /// Helper to change an object field and serialize it to disk
    /// </summary>
    private void ObjectField(string key, string label, ref GameObject go)
    {
        var prefab = EditorGUILayout.ObjectField(label, go, typeof(GameObject), false) as GameObject;
        if (prefab != go)
        {
            if (prefab == null)
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();

                go = null;
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(prefab);

                PlayerPrefs.SetString(key, path);
                PlayerPrefs.Save();

                go = prefab;
            }
        }
    }

    /// <summary>
    /// Helper to load an object field
    /// </summary>
    private void LoadObject(string key, ref GameObject go)
    {
        if (go == null && PlayerPrefs.HasKey(key))
        {
            var path = PlayerPrefs.GetString(key);
            go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        }
    }
#endregion
}
