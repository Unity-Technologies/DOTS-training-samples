/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace SWS
{
    /// <summary>
    /// Movement script: pathfinding using Unity NavMesh agents.
    /// <summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [AddComponentMenu("Simple Waypoint System/navMove")]
    public class navMove : MonoBehaviour
    {
        /// <summary>
        /// Path component to use for movement.
        /// <summary>
        public PathManager pathContainer;

        /// <summary>
        /// Whether this object should start its movement at game launch.
        /// <summary>
        public bool onStart = false;

        /// <summary>
        /// Whether this object should walk to the first waypoint or spawn there.
        /// <summary>
        public bool moveToPath = false;

        /// <summary>
        /// reverse the movement direction on the path, typically used for "pingPong" behavior.
        /// <summary>
        public bool reverse = false;

        /// <summary>
        /// Waypoint index where this object should start its path. 
        /// <summary>
        public int startPoint = 0;

        /// <summary>
        /// Current waypoint indicator on the path. 
        /// <summary>
        [HideInInspector]
        public int currentPoint = 0;

        /// <summary>
        /// Option for closing the path on the "loop" looptype.
        /// <summary>
        public bool closeLoop = false;

        /// <summary>
        /// Whether rotation should be overridden by the NavMesh agent.
        /// <summary>
        public bool updateRotation = true;

        /// <summary>
        /// List of Unity Events invoked when reaching waypoints.
        /// <summary>
        [HideInInspector]
        public List<UnityEvent> events = new List<UnityEvent>();

        /// <summary>
        /// Supported movement looptypes when moving on the path. 
        /// <summary>
        public enum LoopType
        {
            none,
            loop,
            pingPong,
            random
        }
        public LoopType loopType = LoopType.none;

        /// <summary>
        /// Waypoint array references of the requested path.
        /// <summary>
        [HideInInspector]
        public Transform[] waypoints;

        //used on loopType "pingPong" for determining forward or backward movement.
        private bool repeat = false;

        //reference to the agent component
        private NavMeshAgent agent;
        //looptype random generator
        private System.Random rand = new System.Random();
        //looptype random current waypoint index
        private int rndIndex = 0;
        //whether the tween was paused
        private bool waiting = false;


        //initialize components
        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }


        //check for automatic initialization
        void Start()
        {
            if (onStart)
                StartMove();
        }


        /// <summary>
        /// Starts movement. Can be called from other scripts to allow start delay.
        /// <summary>
        public void StartMove()
        {
            //don't continue without path container
            if (pathContainer == null)
            {
                Debug.LogWarning(gameObject.name + " has no path! Please set Path Container.");
                return;
            }

            //get Transform array with waypoint positions
            waypoints = new Transform[pathContainer.waypoints.Length];
            Array.Copy(pathContainer.waypoints, waypoints, pathContainer.waypoints.Length);
            
            //initialize waypoint positions
            startPoint = Mathf.Clamp(startPoint, 0, waypoints.Length - 1);
            int index = startPoint;
            if (reverse)
            {
                Array.Reverse(waypoints);
                index = waypoints.Length - 1 - index;
            }
            currentPoint = index;

            //event count is smaller than waypoint count,
            //add empty event per waypoint
            for (int i = events.Count; i <= waypoints.Length - 1; i++)
                events.Add(new UnityEvent());

            Stop();
            StartCoroutine(Move());
        }


        //constructs the tween and starts movement
        private IEnumerator Move()
        {
            //enable agent updates
            #if UNITY_5_6_OR_NEWER
            agent.isStopped = false;
            #else
            agent.Resume();
            #endif
            agent.updateRotation = updateRotation;

            //if move to path is enabled,
            //set an additional destination to the first waypoint
            if (moveToPath)
            {
                agent.SetDestination(waypoints[currentPoint].position);
                yield return StartCoroutine(WaitForDestination());
            }

            //we're now at the first waypoint position, so directly call the next waypoint.
            //on looptype random we have to initialize a random order of waypoints first.
            if (loopType == LoopType.random)
            {
                StartCoroutine(ReachedEnd());
                yield break;
            }

            if (moveToPath)
                StartCoroutine(NextWaypoint());
            else
                GoToWaypoint(startPoint);

            moveToPath = false;
        }


        //this method moves us one by one to the next waypoint
        //and executes all delay or tweening interaction
        private IEnumerator NextWaypoint()
        {
            //execute events for this waypoint
            OnWaypointChange(currentPoint);
            yield return new WaitForEndOfFrame();

            //check for pausing and wait until unpaused again
            while (waiting) yield return null;
            Transform next = null;

            //repeating mode is on: moving backwards
            if (loopType == LoopType.pingPong && repeat)
                currentPoint--;
            else if (loopType == LoopType.random)
            {
                //parse currentPoint value from waypoint
                rndIndex++;
                currentPoint = int.Parse(waypoints[rndIndex].name.Replace("Waypoint ", ""));
                next = waypoints[rndIndex];
            }
            else
                //default mode: move forwards
                currentPoint++;

            //just to make sure we don't run into an out of bounds exception
            currentPoint = Mathf.Clamp(currentPoint, 0, waypoints.Length - 1);
            //set the next waypoint based on calculated current point
            if (next == null) next = waypoints[currentPoint];

            //set destination to the next waypoint
            agent.SetDestination(next.position);
            yield return StartCoroutine(WaitForDestination());

            //determine if the agent reached the path's end
            if (loopType != LoopType.random && currentPoint == waypoints.Length - 1
                || rndIndex == waypoints.Length - 1 || repeat && currentPoint == 0)
                StartCoroutine(ReachedEnd());
            else
                StartCoroutine(NextWaypoint());
        }


        //wait until the agent reached its destination
        private IEnumerator WaitForDestination()
        {
            yield return new WaitForEndOfFrame();         
            while (agent.pathPending)
                yield return null;
            yield return new WaitForEndOfFrame();

            float remain = agent.remainingDistance;
            while (remain == Mathf.Infinity || remain - agent.stoppingDistance > float.Epsilon
            || agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                remain = agent.remainingDistance;
                yield return null;
            }
        }


        //called at every waypoint to invoke events
        private void OnWaypointChange(int index)
        {
            if (reverse) index = waypoints.Length - 1 - index;

            if (events == null || events.Count - 1 < index || events[index] == null)
                return;

            events[index].Invoke();
        }


        //object reached the end of its path
        private IEnumerator ReachedEnd()
        {
            //each looptype has specific properties
            switch (loopType)
            {
                //LoopType.none means there will be no repeat,
                //so we just execute the final event
                case LoopType.none:
                    OnWaypointChange(waypoints.Length - 1);
                    yield break;

                //in a loop we set our position indicator back to zero,
                //also executing last event
                case LoopType.loop:
                    OnWaypointChange(waypoints.Length - 1);

                    //additional option: if the path was closed, we move our object
                    //from the last to the first waypoint instead of just "appearing" there
                    if (closeLoop)
                    {
                        agent.SetDestination(waypoints[0].position);
                        yield return StartCoroutine(WaitForDestination());
                    }
                    else
                        agent.Warp(waypoints[0].position);

                    currentPoint = 0;
                    break;

                //on LoopType.pingPong, we have to invert currentPoint updates
                case LoopType.pingPong:
                    repeat = !repeat;
                    break;

                //on LoopType.random, we calculate a random order between all waypoints
                //and loop through them, for this case we use the Fisher-Yates algorithm
                case LoopType.random:
                    RandomizeWaypoints();
                    break;
            }

            //start moving to the next iteration
            StartCoroutine(NextWaypoint());
        }


        private void RandomizeWaypoints()
        {
            //reinitialize original waypoint positions
            Array.Copy(pathContainer.waypoints, waypoints, pathContainer.waypoints.Length);
            int n = waypoints.Length;

            //shuffle waypoints array
            while (n > 1)
            {
                int k = rand.Next(n--);
                Transform temp = waypoints[n];
                waypoints[n] = waypoints[k];
                waypoints[k] = temp;
            }

            //since all waypoints are shuffled the first waypoint does not
            //correspond with the actual current position, so we have to
            //swap the first waypoint with the actual waypoint
            Transform first = pathContainer.waypoints[currentPoint];
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == first)
                {
                    Transform temp = waypoints[0];
                    waypoints[0] = waypoints[i];
                    waypoints[i] = temp;
                    break;
                }
            }

            //reset random loop index
            rndIndex = 0;
        }


        /// <summary>
        /// Teleports to the defined waypoint index on the path.
        /// </summary>
        public void GoToWaypoint(int index)
        {
            if (reverse) index = waypoints.Length - 1 - index;

            Stop();
            currentPoint = index;
            agent.Warp(waypoints[index].position);
            StartCoroutine(NextWaypoint());
        }


        /// <summary>
        /// Pauses the current movement routine for a defined amount of time.
        /// <summary>
        public void Pause(float seconds = 0f)
        {
            StopCoroutine(Wait());
            waiting = true;

            #if UNITY_5_6_OR_NEWER
            agent.isStopped = true;
            #else
            agent.Stop();
            #endif

            if (seconds > 0)
                StartCoroutine(Wait(seconds));
        }


        //waiting routine
        private IEnumerator Wait(float secs = 0f)
        {
            yield return new WaitForSeconds(secs);
            Resume();
        }


        /// <summary>
        /// Resumes the current movement routine.
        /// <summary>
        public void Resume()
        {
			StopCoroutine(Wait());
            waiting = false;
            
            #if UNITY_5_6_OR_NEWER
            agent.isStopped = false;
            #else
            agent.Resume();
            #endif
        }
        
        
        /// <summary>
        /// Reverses movement at any time.
        /// <summary>
        public void Reverse()
        {
            //inverse direction toggle
            reverse = !reverse;
            
            //invert starting point from current waypoint
            if(reverse)
                startPoint = currentPoint - 1;
            else
            {
                //reverse path even if not currently in inversed state
                Array.Reverse(waypoints);
                startPoint = waypoints.Length - currentPoint;
            }
            
            //start new iteration
            moveToPath = true;
            StartMove();
        }


        /// <summary>
        /// Changes the current path of this walker object and starts movement.
        /// <summary>
        public void SetPath(PathManager newPath)
        {
            //disable any running movement methods
            Stop();
            //set new path container
            pathContainer = newPath;
            //restart movement
            StartMove();
        }


        /// <summary>
        /// Disables any running movement routines.
        /// <summary>
        public void Stop()
        {
            StopAllCoroutines();
            if (agent.enabled)
            {
                #if UNITY_5_6_OR_NEWER
                agent.isStopped = true;
                #else
                agent.Stop();
                #endif
            }
        }


        /// <summary>
        /// Stops movement and resets to the first waypoint.
        /// <summary>
        public void ResetToStart()
        {
            Stop();
            currentPoint = 0;
            if (pathContainer)
            {
                agent.Warp(pathContainer.waypoints[currentPoint].position);
            }
        }


        /// <summary>
        /// Wrapper to change agent speed.
        /// <summary>
        public void ChangeSpeed(float value)
        {
            agent.speed = value;
        }
    }
}