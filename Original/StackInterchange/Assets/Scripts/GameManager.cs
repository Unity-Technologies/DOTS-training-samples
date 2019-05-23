using System;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject BgGameObject;
    public SpawnManager SpawnManagerPrefab;
    public PathManager PathManagerPrefab;

    public int QuadCount;
    public int ActiveSpawnManagerIndex { get; private set; }
    public SpawnManager ActiveSpawnManager { get; private set; }
    private List<SpawnManager> _spawnManagers = new List<SpawnManager>();

    [Header("Road Spawn Weights")]
    [Range(0, 1f)]
    public float RoadNoTurnsWeight = 0.6f;
    [Range(0, 1f)]
    public float RoadOneTurnWeight = 0.2f;
    [Range(0, 1f)]
    public float RoadTwoTurnsWeight = 0.2f;

    [Header("Vehicle Materials")]
    public Material PinkMaterial;
    public Material RedMaterial;
    public Material PurpleMaterial;
    public Material BlueMaterial;

    public enum TrafficLevel
    {
        High,
        Medium,
        Light,
    }

    #region Properties
    private int _countOfActiveCars;
    private int _peakCountActiveCars;
    public int CountOfActiveCars
    {
        get
        {
            return _countOfActiveCars;
        }
        set
        {
            _countOfActiveCars = value;
            _peakCountActiveCars = Mathf.Max(_peakCountActiveCars, _countOfActiveCars);
            SetCarCountText();
        }
    }

    public static GameManager Instance { get; private set; }
    #endregion

    #region Variable traffic spawn
    private TrafficLevel _currentTrafficLevel;
    private float _elapsedTimeSinceTrafficLevelSwitch;
    private const float SEONDS_BETWEEN_TRAFFIC_LEVEL_SWITCH = 15;
    private const float TIME_TO_WAIT_BETWEEN_CAR_LAUNCHES_MULTIPLIER = 0.5f;
    public float CurrentTimeToWaitBetweenCarLaunches { get; private set; }
    #endregion

    #region Constants
    public readonly int POOL_SIZE = 250;
    private const int DISTANCE_BETWEEN_QUADS = 500;
    public readonly float TIME_TO_WAIT_BETWEEN_CAR_LAUNCHES = 5.0f;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        Instance = this;
        if (QuadCount < 1)
        {
            QuadCount = 1;
        }
        
        float xPos = ((QuadCount - 1) * DISTANCE_BETWEEN_QUADS) / 2f;
        BgGameObject.transform.position = new Vector3(xPos, BgGameObject.transform.position.y, 0);
        float newScale = BgGameObject.transform.localScale.x * (QuadCount * 1.5f);
        BgGameObject.transform.localScale = new Vector3(newScale, 1, newScale);
    }

    private void Start()
    {
        for (int i = 0; i < QuadCount; i++)
        {
            SpawnNextQuad(i);
        }

        SetCarCountText();
        SetTrafficStatusText();
        UIManager.Instance.SetQuadButtons(QuadCount);
        CurrentTimeToWaitBetweenCarLaunches = TIME_TO_WAIT_BETWEEN_CAR_LAUNCHES;
    }

    private void Update()
    {
        _elapsedTimeSinceTrafficLevelSwitch += Time.deltaTime;

        if (_elapsedTimeSinceTrafficLevelSwitch > SEONDS_BETWEEN_TRAFFIC_LEVEL_SWITCH)
        {
            _elapsedTimeSinceTrafficLevelSwitch = 0;
            _currentTrafficLevel = (TrafficLevel)(((int)_currentTrafficLevel + 1) % Enum.GetValues(typeof(TrafficLevel)).Length);
            CurrentTimeToWaitBetweenCarLaunches = GameManager.Instance.TIME_TO_WAIT_BETWEEN_CAR_LAUNCHES + (GameManager.Instance.TIME_TO_WAIT_BETWEEN_CAR_LAUNCHES * ((int)_currentTrafficLevel * TIME_TO_WAIT_BETWEEN_CAR_LAUNCHES_MULTIPLIER));
            SetTrafficStatusText();
        }
    }
    #endregion

    #region Helpers
    private void SpawnNextQuad(int quadIndex)
    {
        GameObject go = new GameObject
        {
            name = string.Format("Quad_{0}", quadIndex)
        };
        go.transform.position = new Vector3(quadIndex * DISTANCE_BETWEEN_QUADS, 0, 0);
        var pathManager = Instantiate(PathManagerPrefab, go.transform);
        var spawnManager = Instantiate(SpawnManagerPrefab, go.transform);
        pathManager.name = string.Format("PathManager_{0}", quadIndex);
        spawnManager.name = string.Format("SpawnManager_{0}", quadIndex);
        spawnManager.PathManagerInstance = pathManager;
        _spawnManagers.Add(spawnManager);
    }

    public void SetCarCountText()
    {
        UIManager.Instance.SetCarCountText(
                        string.Format("<color=#ffffff>{0} / {1} </color>active cars", CountOfActiveCars, (POOL_SIZE * QuadCount)),
                        string.Format("Peak: <color=#ffffff>{0}</color>", _peakCountActiveCars));
    }

    private void SetTrafficStatusText()
    {
        UIManager.Instance.TrafficStatusText.text = string.Format("Traffic status: <color=#ffffff><i>{0}</i></color>", _currentTrafficLevel.ToString());
    }

    private int GetRandomWeighted()
    {
        float random = UnityEngine.Random.Range(0, RoadNoTurnsWeight + RoadOneTurnWeight + RoadTwoTurnsWeight);
        if (random < RoadNoTurnsWeight)
        {
            return 0;
        }
        else if (random < (RoadNoTurnsWeight + RoadOneTurnWeight))
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
    #endregion

    #region Public API
    public Destination GetRandomDestination(Destination CurrentLocation)
    {
        int randomIndex = GetRandomWeighted();
        Destination[] destinations = new Destination[3];

        switch (CurrentLocation)
        {
            case Destination.Pink:
                destinations[0] = Destination.Pink;
                destinations[1] = Destination.Purple;
                destinations[2] = Destination.Blue;
                break;
            case Destination.Red:
                destinations[0] = Destination.Red;
                destinations[1] = Destination.Blue;
                destinations[2] = Destination.Purple;
                break;
            case Destination.Purple:
                destinations[0] = Destination.Purple;
                destinations[1] = Destination.Red;
                destinations[2] = Destination.Pink;
                break;
            case Destination.Blue:
                destinations[0] = Destination.Blue;
                destinations[1] = Destination.Pink;
                destinations[2] = Destination.Red;
                break;
            default:
                Debug.LogErrorFormat("Destination {0} unhandled.", CurrentLocation);
                return Destination.Red;
        }
        return destinations[randomIndex];
    }

    public Material GetMaterialForEndPosition(Destination endPosition)
    {
        switch (endPosition)
        {
            case Destination.Pink:
                return PinkMaterial;
            case Destination.Red:
                return RedMaterial;
            case Destination.Purple:
                return PurpleMaterial;
            case Destination.Blue:
                return BlueMaterial;
            default:
                Debug.LogErrorFormat("Destination {0} unhandled.", endPosition);
                return PinkMaterial;
        }
    }

    public void GoToNextQuad()
    {
        ActiveSpawnManagerIndex++;
        ActiveSpawnManagerIndex %= _spawnManagers.Count;
        ActiveSpawnManager = _spawnManagers[ActiveSpawnManagerIndex];
        StartCoroutine(CameraController.Instance.MoveCamera(new Vector3(ActiveSpawnManagerIndex * DISTANCE_BETWEEN_QUADS, 0, 0)));
    }

    public void GoToPreviousQuad()
    {
        ActiveSpawnManagerIndex--;
        if (ActiveSpawnManagerIndex < 0)
        {
            ActiveSpawnManagerIndex = _spawnManagers.Count - 1;
        }
        else
        {
            ActiveSpawnManagerIndex %= _spawnManagers.Count;
        }
        ActiveSpawnManager = _spawnManagers[ActiveSpawnManagerIndex];
        StartCoroutine(CameraController.Instance.MoveCamera(new Vector3(ActiveSpawnManagerIndex * DISTANCE_BETWEEN_QUADS, 0, 0)));
    }
    #endregion
}