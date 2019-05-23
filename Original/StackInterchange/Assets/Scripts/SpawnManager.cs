using System;
using UnityEngine;
using System.ComponentModel;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    #region Public Members
    [Header("Vehicles")]
    public Car CarPrefab;
    public Car BikePrefab;
    public Car TruckPrefab;

    [Header("Vehicle Spawn Weights")]
    [Range(0, 1f)]
    public float CarSpawnWeight = 0.4f;
    [Range(0, 1f)]
    public float BikeSpawnWeight = 0.4f;
    [Range(0, 1f)]
    public float TruckSpawnWeight = 0.2f;

    private const int NUMBER_OF_LANES = 4;
    private const int NUMBER_OF_CARS_PER_LANE = 2;
    public PathManager PathManagerInstance;
    #endregion

    #region Private Members
    private List<Car> _carPool;
    private int _currentPoolIndex;
    private bool _isOnLeftLane = true;
    private Destination _currentSpawnLocation;
    private float _elapsedTimeSinceLastCarLaunches;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        _carPool = new List<Car>(GameManager.Instance.POOL_SIZE);
    }

    private void Start()
    {
        for (int i = 0; i < GameManager.Instance.POOL_SIZE; i++)
        {
            Car carToSpawn = WeightedRandomCarToSpawn();
            var newCar = Instantiate(carToSpawn, transform);
            newCar.name = string.Format("{0}_{1}", i, carToSpawn.name);
            newCar.IsOnLeftLane = _isOnLeftLane;
            newCar.StartPosition = _currentSpawnLocation;
            newCar.CarId = i;
            newCar.gameObject.SetActive(false);
            _carPool.Add(newCar);
            _isOnLeftLane = !_isOnLeftLane;
            if (!_isOnLeftLane)
            {
                GetNextSpawnLocation();
            }
        }
    }

    private void Update()
    {
        _elapsedTimeSinceLastCarLaunches += Time.deltaTime;


        if (_elapsedTimeSinceLastCarLaunches > GameManager.Instance.CurrentTimeToWaitBetweenCarLaunches)
        {
            for (int i = 0; i < (NUMBER_OF_LANES * NUMBER_OF_CARS_PER_LANE); i++)
            {
                if (!_carPool[_currentPoolIndex].IsInMotion)
                {
                    _carPool[_currentPoolIndex].Launch(this);
                }

                _currentPoolIndex++;
                _currentPoolIndex %= GameManager.Instance.POOL_SIZE;
            }
            _elapsedTimeSinceLastCarLaunches = 0;
        }
    }

    #endregion



    #region Helpers
    private void GetNextSpawnLocation()
    {
        _currentSpawnLocation = (Destination)(((int)_currentSpawnLocation + 1) % Enum.GetValues(typeof(Destination)).Length);
    }

    private Car WeightedRandomCarToSpawn()
    {
        float random = UnityEngine.Random.Range(0, CarSpawnWeight + BikeSpawnWeight + TruckSpawnWeight);
        if (random < CarSpawnWeight)
        {
            return CarPrefab;
        }
        else if (random < (CarSpawnWeight + BikeSpawnWeight))
        {
            return BikePrefab;
        }
        else
        {
            return TruckPrefab;
        }
    }

    public Car GetRandomActiveCar()
    {
        List<Car> ActiveCars = new List<Car>();
        foreach (var car in _carPool)
        {
            if (car.IsInMotion)
            {
                ActiveCars.Add(car);
            }
        }
        return ActiveCars[UnityEngine.Random.Range(0, ActiveCars.Count)];
    }
    #endregion
}