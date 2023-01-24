using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{

    public static Transform[] carTransforms;
    public GameObject carPrefab;
    public int numCars;
    public List<Car> carControl;

    public HighwaySpawner highwaySpawner;

    // Start is called before the first frame update
    void Start()
    {
        carTransforms = new Transform[numCars];
        for (int i = 0; i < numCars; i++)
        {
            GameObject go = GameObject.Instantiate(carPrefab);
            Car car = go.GetComponent<Car>();
            carControl.Add(car);
            Vector2 dir = Random.insideUnitCircle;
            car.SegmentID = 0;
            car.lane = i;
            carTransforms[i] = go.transform;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveCars();
    }

    public void moveCars()
    {
        foreach (Car car in carControl)
        {

            float moveSpeed = 100 * Time.deltaTime;
            float carLane = 4 + (2.45f * car.lane);

            Transform currentSegment = highwaySpawner.HighwaySegments[car.SegmentID];
            Vector3 currentPosition = currentSegment.position + (currentSegment.right * -carLane);

            Transform targetSegment = (car.SegmentID + 1 < highwaySpawner.HighwaySegments.Count) ? highwaySpawner.HighwaySegments[car.SegmentID + 1] : highwaySpawner.HighwaySegments[0];
            Vector3 targetPosition = targetSegment.position + (targetSegment.right * -carLane);

            Transform futureSegment = (car.SegmentID + 2 < highwaySpawner.HighwaySegments.Count) ? highwaySpawner.HighwaySegments[car.SegmentID + 2] : highwaySpawner.HighwaySegments[2 - (highwaySpawner.HighwaySegments.Count - car.SegmentID)];
            Vector3 futurePosition = futureSegment.position + (futureSegment.right * -carLane);

            //Calculate rotation
            float currentDistance = Vector3.Magnitude(car.transform.position - targetPosition);
            float calculatePrecentage = 1 - Mathf.Clamp01((currentDistance - 0.25f) / (car.SegmentDistance -0.25f));
            Vector3 currentDirection = targetPosition - currentPosition;
            Vector3 targetDirection = futurePosition - targetPosition;
            Vector3 directionLerp = Vector3.Lerp(currentDirection, targetDirection, calculatePrecentage).normalized * moveSpeed;
            Quaternion carRotation = Quaternion.LookRotation(directionLerp);

            car.transform.position = Vector3.MoveTowards(car.transform.position, targetPosition, moveSpeed);
            car.transform.localEulerAngles = new Vector3(0, carRotation.eulerAngles.y, 0);

            if(currentDistance <= 0.25f)
            {
                car.SegmentID = (car.SegmentID < highwaySpawner.HighwaySegments.Count - 1) ? car.SegmentID += 1 : 0;
                car.SegmentDistance = Vector3.Magnitude(car.transform.position - futurePosition);
            }
        }
    }


}
