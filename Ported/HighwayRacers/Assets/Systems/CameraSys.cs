using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace HighwayRacer
{
    public enum State
    {
        TOP_DOWN,
        CAR,
    }

    [UpdateAfter(typeof(RoadSys))]
    public class CameraSys : SystemBase
    {
        public Rect bounds = new Rect(-500, -500, 1000, 1000);
        public float moveSpeed = 20;
        public const float transitionDuration = 1.5f;

        public float transitionTimer;

        private Vector3 topDownPosition = new Vector3();
        private Quaternion topDownRotation = Quaternion.Euler(90, 0, 0);

        public const float minClickDist = 3f;
        public readonly float3 carCamOffset = new float3(0, 0.797f, 1.67f);

        public Entity car;
        public State state = State.TOP_DOWN;
        
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void ResetCamera()
        {
            state = State.TOP_DOWN;
            Camera.main.transform.position = topDownPosition;
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
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
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
                                transitionTimer = 0;
                                CarProperties.instance.Show(closestEnt);
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
                
                case State.CAR:

                    if (Input.GetKey(KeyCode.Escape))
                    {
                        state = State.TOP_DOWN;
                        Camera.main.transform.SetPositionAndRotation(topDownPosition, topDownRotation);
                        return;
                    }

                    transitionTimer += Time.DeltaTime;
                    
                    var trans = EntityManager.GetComponentData<Translation>(car);
                    var rot = EntityManager.GetComponentData<Rotation>(car);

                    var camPos = math.rotate(rot.Value, carCamOffset);
                    camPos += trans.Value;

                    if (transitionTimer < transitionDuration)
                    {
                        var fraction = transitionTimer / transitionDuration;
                        camPos = math.lerp(topDownPosition, camPos, fraction);
                        rot.Value = math.nlerp(topDownRotation, rot.Value, fraction);   // use slerp?
                    }
                    Camera.main.transform.SetPositionAndRotation(camPos, rot.Value);    

                    break;
            }
        }
    }
}