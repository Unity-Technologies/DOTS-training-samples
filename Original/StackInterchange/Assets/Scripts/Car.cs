using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    #region Properties
    public Car LastCarHit;
    private Vector3 _laneOffset;
    public int CarId { get; set; }
    public bool IsOnLeftLane { get; set; }
    public bool IsInMotion { get; private set; }
    public Destination StartPosition { get; set; }
    public Destination EndPosition { get; set; }
    #endregion

    [Header("Car Properties")]
    public float MinSpeed = 5;
    public float MaxSpeed = 8;
    public float AccelerationMultiplier = 0.1f;
    public float DeccelerationMultiplier = 2.6f;

    public float MaxSpeedWhenChangingLanes = 5;
    public MeshRenderer CarMeshRenderer;

    private bool[] _leftTurnsData;
    private float _speedMultiplier;
    private float _currentCarSpeed;
    private Vector3[] _currentPath;
    private bool _requireLaneChange;
    private int _currentWaypointIndex;
    private Vector3 _nextWaypointData;
    private float _pathCompletedProgress;
	private const float CAR_WIDTH = 1.2f;
    private float[] _leftTurnsMarks = new float[] { .15f, .32f, .7f };

    #region RayCastProperties
    private int _layerMask = 1 << 8;
    private int _currentRayCastIndex;
    private float _rayCastAngleDeltaResolution;
    private const int RAYCAST_ANGLE_RESOLUTION = 40;
    private const float CAR_SIZE_PLUS_SAFE_CAR_DISTANCE = 4.5f;

    //Distance
    private float _rayCastDistance = 20f;
    private const float MIN_RAYCAST_DISTANCE = 5;
    private const float MAX_RAYCAST_DISTANCE = 20;
    private const float MIN_DISTANCE_TO_NEXT_WAYPOINT = 4.0f;

    //Angle
    private float _rayCastAngleVariance = 0.1f;
    private const float MIN_RAYCAST_ANGLE_VARIANCE = 0.1f; //10 Degrees on each side
    private const float MAX_RAYCAST_ANGLE_VARIANCE = 0.6f; //60 Degrees on each side
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        _rayCastDistance = MIN_RAYCAST_DISTANCE;
        _rayCastAngleVariance = MIN_RAYCAST_ANGLE_VARIANCE;
        _rayCastAngleDeltaResolution = _rayCastAngleVariance / RAYCAST_ANGLE_RESOLUTION;
    }

    private void Update()
    {
        if (!IsInMotion)
        {
            return;
        }


        transform.LookAt(_nextWaypointData);
        var distanceToWayPoint = Vector3.Distance(transform.position, _nextWaypointData);
        bool forceNextWayPoint = distanceToWayPoint < 0;

        if (forceNextWayPoint)
        {
            Debug.LogErrorFormat("Distance {0} is behind car on: {1}", distanceToWayPoint, name);
        }

        if (forceNextWayPoint || Mathf.Abs(Vector3.Distance(transform.position, _nextWaypointData)) < MIN_DISTANCE_TO_NEXT_WAYPOINT)
        {
            _currentWaypointIndex++;
            _currentWaypointIndex = Mathf.Clamp(_currentWaypointIndex, 0, _currentPath.Length - 1);
            _pathCompletedProgress = _currentWaypointIndex / ((_currentPath.Length - 1) * 1f);
            _nextWaypointData = _currentPath[_currentWaypointIndex] + _laneOffset;
            _requireLaneChange = RequireLaneChange();
            if (_requireLaneChange)
            {
                _rayCastDistance = MAX_RAYCAST_DISTANCE;
                _rayCastAngleVariance = MAX_RAYCAST_ANGLE_VARIANCE;
                _rayCastAngleDeltaResolution = _rayCastAngleVariance / RAYCAST_ANGLE_RESOLUTION;
            }
            else
            {
                _rayCastDistance = MIN_RAYCAST_DISTANCE;
                _rayCastAngleVariance = MIN_RAYCAST_ANGLE_VARIANCE;
                _rayCastAngleDeltaResolution = _rayCastAngleVariance / RAYCAST_ANGLE_RESOLUTION;
            }
        }

        RetuneSpeedBasedOnCollisionDistance();
        MoveCar();
    }
    #endregion

    #region Public API
    public void Launch(SpawnManager spawnManager)
    {
        EndPosition = GameManager.Instance.GetRandomDestination(StartPosition);

        SetCarColor();
        _currentWaypointIndex = 0;
        _pathCompletedProgress = 0;
        _currentCarSpeed = UnityEngine.Random.Range(MinSpeed, MaxSpeed);

        _currentPath = spawnManager.PathManagerInstance.GetPath(StartPosition, EndPosition, ref _leftTurnsData);
        if (_currentPath != null && _currentPath.Length > 0)
        {
            GameManager.Instance.CountOfActiveCars++;
            transform.position = _currentPath[_currentWaypointIndex];
            transform.LookAt(_currentPath[_currentWaypointIndex + 1]);
			transform.Translate(IsOnLeftLane ? (Vector3.left * CAR_WIDTH): (Vector3.right * CAR_WIDTH), Space.Self);

            _laneOffset = transform.position - _currentPath[_currentWaypointIndex];
            _nextWaypointData = _currentPath[_currentWaypointIndex + 1] + _laneOffset;
            transform.LookAt(_nextWaypointData, Vector3.up);

            //Last things to do are to enable and set in motion
            gameObject.SetActive(true);
            IsInMotion = true;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Check if a lane change is required.
    /// This should be called after we update the _pathCompletedProgress
    /// </summary>
    /// <returns></returns>
    private bool RequireLaneChange()
    {
        if (_leftTurnsData == null)
        {
            return false;
        }

        for (int i = 0; i < _leftTurnsData.Length; i++)
        {
            float max = Mathf.Max
            (
                _leftTurnsMarks[Mathf.Clamp(i, i - 1, 0)],
                _leftTurnsMarks[i]
            );

            if (_pathCompletedProgress < max)
            {
                if (IsOnLeftLane != _leftTurnsData[i])
                {
                    //Debug.LogWarningFormat("RequireLaneChange {0} - {1} > {2}. IsOnLeftLane: {3} at {4:0.0}%. Max is: {5}.", name, StartPosition, EndPosition, IsOnLeftLane, _pathCompletedProgress * 100, max);
                    return true;
                }
            }
        }
        return false;
    }

    private void RetuneSpeedBasedOnCollisionDistance()
    {
        RaycastHit hit;
        //TODO - Optimisation - Angle variance could be improved to look at direction of turn only.
        float rayCastAngle = (_rayCastAngleDeltaResolution * _currentRayCastIndex * 2) - (_rayCastAngleVariance);
        var rayCastDirection = new Vector3(rayCastAngle, 0, 1);
        var transformDirection = transform.TransformDirection(rayCastDirection);
        if (Physics.Raycast(transform.position, transformDirection, out hit, _rayCastDistance, _layerMask))
        {
            if (_requireLaneChange)
            {
                var hitCar = hit.collider.GetComponent<Car>();
                if ((hitCar != null) && (hitCar.CarId > CarId))
                {
                    LastCarHit = hitCar;
                    //Debug.LogWarningFormat("BEFORE {0} - {1} > {2}. Pos: {3} - _laneOffset: {4}. IsOnLeftLane: {5}", name, StartPosition, EndPosition, transform.position, _laneOffset, IsOnLeftLane);
                    _laneOffset *= -1;
                    _requireLaneChange = false;
                    IsOnLeftLane = !IsOnLeftLane;
                    //Debug.LogWarningFormat("AFTER {0} - {1} > {2}. Pos: {3} - _laneOffset: {4}. IsOnLeftLane: {5}", name, StartPosition, EndPosition, transform.position, _laneOffset, IsOnLeftLane);
                }
            }
            //Smooth acceleration/deceleration
            _speedMultiplier = Mathf.Lerp(_speedMultiplier, ((hit.distance - CAR_SIZE_PLUS_SAFE_CAR_DISTANCE) / MAX_RAYCAST_DISTANCE), Time.deltaTime * DeccelerationMultiplier);
            Debug.DrawRay(transform.position, transformDirection * hit.distance, Color.yellow);
        }
        else
        {
            _speedMultiplier = Mathf.Lerp(_speedMultiplier, 1, Time.deltaTime * AccelerationMultiplier);
            Debug.DrawRay(transform.position, transformDirection * MAX_RAYCAST_DISTANCE, Color.green);
        }
        _speedMultiplier = _requireLaneChange ? Mathf.Min(_speedMultiplier, MaxSpeedWhenChangingLanes) : _speedMultiplier;
        _currentRayCastIndex++;
        _currentRayCastIndex %= RAYCAST_ANGLE_RESOLUTION;
    }

    private void MoveCar()
    {
        if (_pathCompletedProgress < 1.0f)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * _currentCarSpeed * _speedMultiplier);
        }
        else
        {
            IsInMotion = false;
            gameObject.SetActive(false);
            GameManager.Instance.CountOfActiveCars--;
            CameraController.Instance.SwitchCameraIfNeeded(this);
            //Debug.LogFormat("{0} left {1} & arrived at {1}", name, StartPosition, EndPosition);
        }
    }

    private void SetCarColor()
    {
        switch (EndPosition)
        {
            case Destination.Pink:
                CarMeshRenderer.material = GameManager.Instance.GetMaterialForEndPosition(Destination.Pink);
                break;
            case Destination.Red:
                CarMeshRenderer.material = GameManager.Instance.GetMaterialForEndPosition(Destination.Red);
                break;
            case Destination.Purple:
                CarMeshRenderer.material = GameManager.Instance.GetMaterialForEndPosition(Destination.Purple);
                break;
            case Destination.Blue:
                CarMeshRenderer.material = GameManager.Instance.GetMaterialForEndPosition(Destination.Blue);
                break;
            default:
                Debug.LogErrorFormat("Color for position {0} is not handled.", EndPosition);
                break;

        }
    }
    #endregion
}