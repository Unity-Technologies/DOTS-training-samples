using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring {

    public static float DEFAULT_ACCELERATION = 0.01f;
    public static float DEFAULT_FRICTION = 0.5f;
    public static float DEFAULT_ARRIVAL_THRESHOLD = 0.01f;
    public static float DEFAULT_VELOCITY_THRESHOLD = 0.005f;
    public static float DEFAULT_MAX_VELOCITY= 0.1f;

    public float current, target,velocity, arrivalThreshold, velocityThreshold, maxVelocity, acceleration, friction;
    public bool arrived = true;

    public Spring(float _initValue, float _acceleration = 0, float _friction = 0, float _arrivalThreshold = 0, float _velocityThreshold = 0, float _maxVelocity = 0){
        velocity = 0;
        current = target = _initValue;
        acceleration = (_acceleration == 0) ? DEFAULT_ACCELERATION : _acceleration;
        friction = (_friction == 0) ? DEFAULT_FRICTION : _friction;
        arrivalThreshold = (_arrivalThreshold == 0) ? DEFAULT_ARRIVAL_THRESHOLD : _arrivalThreshold;
        velocityThreshold = (_velocityThreshold == 0) ? DEFAULT_VELOCITY_THRESHOLD : _velocityThreshold;
        maxVelocity = (_maxVelocity == 0) ? DEFAULT_MAX_VELOCITY : _maxVelocity;
    }
    public void Update(){
        if (!arrived)
        {
            if (current < (target - arrivalThreshold))
            {
                velocity += acceleration;
            }
            else if (current > (target + arrivalThreshold))
            {
                velocity -= acceleration;
            }
            else
            {
                if (Mathf.Abs(velocity) < velocityThreshold)
                {
                    arrived = true;
                }
            }

            velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
            current += velocity;
            velocity *= friction;
        }
    }

    public void SetTarget(float _newTarget){
        arrived = false;
        target = _newTarget;
    }
}
