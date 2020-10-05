/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using DG.Tweening;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace SWS
{
    /// <summary>
    /// Mecanim motion animator for movement scripts.
    /// Passes speed and direction to the Mecanim controller. 
    /// <summary>
    public class MoveAnimator : MonoBehaviour
    {
        //movement script references
        private splineMove sMove;
        private NavMeshAgent nAgent;
        //Mecanim animator reference
        private Animator animator;
        //cached y-rotation on tweens
        private float lastRotY;


        //getting component references
        void Start()
        {
            animator = GetComponentInChildren<Animator>();

            sMove = GetComponent<splineMove>();
            if (!sMove)
                nAgent = GetComponent<NavMeshAgent>();

        }


        //method override for root motion on the animator
        void OnAnimatorMove()
        {
            //init variables
            float speed = 0f;
            float angle = 0f;

            //calculate variables based on movement script:
            //for splineMove and bezierMove, speed and rotation are regulated by the tween.
            //here we just get the tween's speed and calculate the rotation difference to the last frame.
            //navmesh agents have their own type of movement which has to be calculated separately.
            if (sMove)
            {
                speed = sMove.tween == null || !sMove.tween.IsPlaying() ? 0f : sMove.speed;
                angle = (transform.eulerAngles.y - lastRotY) * 10;
                lastRotY = transform.eulerAngles.y;
            }
            else
            {
                speed = nAgent.velocity.magnitude;
                Vector3 velocity = Quaternion.Inverse(transform.rotation) * nAgent.desiredVelocity;
                angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;
            }

            //push variables to the animator with some optional damping
            animator.SetFloat("Speed", speed);
            animator.SetFloat("Direction", angle, 0.15f, Time.deltaTime);
        }
    }
}