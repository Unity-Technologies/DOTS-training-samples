/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using System.Collections;

namespace SWS
{
    /// <summary>
    /// Attached to a particle system, this component emits particles over time.
    /// Used with a movement script this makes the path partially visible.
    /// <summary>
    public class PathIndicator : MonoBehaviour
    {
        /// <summary>
        /// Rotation value that gets added to the initial particle rotation.
        /// <summary>
        public float modRotation = 0;
        //particle system reference
        private ParticleSystem pSys;

        //get references and start emit routine
        void Start()
        {
            pSys = GetComponentInChildren<ParticleSystem>();
            StartCoroutine("EmitParticles");
        }


        //endless loop for spawning particles in short delays
        IEnumerator EmitParticles()
        {
            //wait movement script to be ready
            yield return new WaitForEndOfFrame();
            //start loop
            while (true)
            {
                //set particle rotation
                float rot = (transform.eulerAngles.y + modRotation) * Mathf.Deg2Rad;
                #if UNITY_5_5_OR_NEWER
                var pMain = pSys.main;
                pMain.startRotation = rot;
                #else
                pSys.startRotation = rot;
                #endif
                
                //emit one particle
                pSys.Emit(1);
                //wait before emitting another one
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}