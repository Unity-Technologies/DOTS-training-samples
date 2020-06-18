using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacer
{
    public enum State
    {
        TOP_DOWN,
        TO_CAR,
        CAR,
        TO_TOP_DOWN,
    }

    public class CameraControlSys : SystemBase
    {
        public Rect bounds = new Rect(-500, -500, 1000, 1000);
        public float moveSpeed = 20;
        public float transitionDuration = 3;

        Vector3 topDownPosition = new Vector3();
        Quaternion topDownRotation = Quaternion.Euler(90, 0, 0);

        public const float minClickDist = 2f;

        public Entity car;
        
        public State state = State.TOP_DOWN;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            float dt = Time.DeltaTime;

            switch (state)
            {
                case State.TOP_DOWN:

                    const float moveScale = 2.0f;

                    Vector2 v = new Vector2();

                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        v.x = -moveSpeed;
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        v.x = moveSpeed;
                    }
                    else
                    {
                        v.x = 0;
                    }

                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        v.y = -moveSpeed;
                        Debug.Log("down");
                    }
                    else if (Input.GetKey(KeyCode.UpArrow))
                    {
                        v.y = moveSpeed;
                    }
                    else
                    {
                        v.y = 0;
                    }

                    // look for car nearest to click; if closest car is close enough, switch to that car's cam
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out hit))
                        {
                            Debug.Log(hit.point);

                            var point = new float3(hit.point);
                            var closestEnt = new Entity() {Index = -1};
                            var closestTrans = float3.zero;
                            var closestDist = float.MaxValue;
                            Entities.ForEach((Entity ent, in Translation translation) =>
                            {
                                var dist = math.distance(point, translation.Value);
                                if (dist < closestDist)
                                {
                                    closestEnt = ent;
                                    closestTrans = translation.Value;
                                    closestDist = dist;
                                }
                            }).Run();

                            if (closestDist != float.MaxValue && closestDist < minClickDist)
                            {
                                car = closestEnt;
                                state = State.CAR;
                            }
                        }
                    }

                    var transform = Camera.main.transform;
                    var newPos = new Vector3(
                        Mathf.Clamp(transform.position.x + v.x * dt * moveScale, bounds.xMin, bounds.xMax),
                        transform.position.y,
                        Mathf.Clamp(transform.position.z + v.y * dt * moveScale, bounds.yMin, bounds.yMax)
                    );
                    topDownPosition = newPos;
                    Camera.main.transform.SetPositionAndRotation(topDownPosition, topDownRotation);
                    break;

                case State.TO_CAR:
                case State.CAR:

                    if (Input.GetKey(KeyCode.Escape))
                    {
                        state = State.TOP_DOWN;
                        Camera.main.transform.SetPositionAndRotation(topDownPosition, topDownRotation);
                    }
                    
                    // carPosition = car.cameraPos.position;
                    // carRotation = car.cameraPos.rotation;
                    //
                    // if (time >= transitionDuration || state == State.CAR) {
                    // 	if (state == State.TO_CAR) {
                    // 		state = State.CAR;
                    // 		carRef.Hide();
                    // 	}
                    // 	transform.position = carPosition;
                    // 	transform.rotation = carRotation;
                    // } else {
                    //
                    // 	transform.position = Vector3.Lerp(topDownPosition, carPosition, time / transitionDuration);
                    // 	transform.rotation = Quaternion.Slerp(topDownRotation, carRotation, time / transitionDuration);
                    //
                    // }

                    break;
                
                // case State.TO_TOP_DOWN:
                //
                // 	transform.position = Vector3.Lerp (carPosition, topDownPosition, time / transitionDuration);
                // 	transform.rotation = Quaternion.Slerp (carRotation, topDownRotation, time / transitionDuration);
                //
                // 	if (time >= transitionDuration || state == State.CAR) {
                // 		state = State.TOP_DOWN;
                // 	} 
                // 	break;
                //
            }
        }
    }
}