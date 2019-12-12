using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
using static UnityEngine.PlayerLoop.FixedUpdate;

public class RuntimePlayerLoopDisable
{
    [RuntimeInitializeOnLoadMethod]
    public static void DisableSystems()
    {
        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        var subsystems = playerLoop.subSystemList;

        var types = "";

        var systemsToRemove = new HashSet<Type>
        {
            typeof(PhysicsFixedUpdate)
        };

        foreach(var curSub in subsystems)
        {
            if(curSub.type == typeof(FixedUpdate))
            {
                foreach(var targetSub in curSub.subSystemList)
                {
                    if(targetSub.type == typeof(PhysicsFixedUpdate))
                    {
                        types += targetSub.type + "\n";
                    }
                }
            }

        }

        Debug.Log(types);
        
    }
}