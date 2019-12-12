// Put this in an editor folder

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Profiling;

public class PlayerLoopWindow : EditorWindow
{
    [MenuItem("Window/Player Loop Visualizer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PlayerLoopWindow window = (PlayerLoopWindow)EditorWindow.GetWindow(typeof(PlayerLoopWindow));
        window.titleContent = new GUIContent("PlayerLoop");
        window.Show();
    }

    private void OnEnable()
    {
        hasCustomPlayerLoop = EditorPrefs.GetBool("PlayerLoopWin_hasCustom", false);

        this.autoRepaintOnSceneChange = true;
    }


    private void OnDisable()
    {
        EditorPrefs.SetBool("PlayerLoopWin_hasCustom", hasCustomPlayerLoop);
    }


    private static bool hasCustomPlayerLoop = false;
    private static bool getProfileTimingInfo = false;
    private static PlayerLoopSystem currentPlayerLoop = new PlayerLoopSystem();
    private static PlayerLoopSystem nextPlayerLoop; // used for storing changes to the loop. Applied at the end of the GUI draw.
    private static bool hasUpdated = false;
    Vector2 scroll;
    void OnGUI()
    {
        GUILayout.Label("Player Loop Visualizer", EditorStyles.boldLabel);

        // Check to see if we need to initialize the PlayerLoopSystem. 
        if (currentPlayerLoop.subSystemList == null)
        {
            if (hasCustomPlayerLoop)
            {
                // if were expecting a custom loop, use Generate Custom
                currentPlayerLoop = GenerateCustomLoop();
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            }
            else
            {
                // Otherwise grab the default loop
                currentPlayerLoop = PlayerLoop.GetDefaultPlayerLoop();
            }
        }

        // Draw the entier list out in a scrollable area (it gets really big!)
        scroll = EditorGUILayout.BeginScrollView(scroll, GUIStyle.none, GUI.skin.verticalScrollbar);
        foreach (var loopSystem in currentPlayerLoop.subSystemList)
        {
            DrawSubsystemList(loopSystem, 0);

            if (hasUpdated)
            {
                hasUpdated = false;
                currentPlayerLoop = nextPlayerLoop;
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            }
        }
        EditorGUILayout.EndScrollView();

        // Draw out a documentation help box
        EditorGUILayout.HelpBox(infoBox, MessageType.Info, true);

        // and finally draw our demo interaction buttons!
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (!hasCustomPlayerLoop)
        {
            if (GUILayout.Button("Add Custom System"))
            {
                hasCustomPlayerLoop = true;
                currentPlayerLoop = GenerateCustomLoop();
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            }
        }
        else
        {
            if (GUILayout.Button("Remove Custom System"))
            {
                hasCustomPlayerLoop = false;
                currentPlayerLoop = PlayerLoop.GetDefaultPlayerLoop();
                PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
        if (GUILayout.Button(getProfileTimingInfo ? "Disable Profiler" : "Enable Profiler"))
        {
            // simply toggle the profiler bool
            getProfileTimingInfo = !getProfileTimingInfo;
        }

        if (GUILayout.Button("Reset"))
        {
            // nulls the cached player loop system so the default system is grabbed again
            hasCustomPlayerLoop = false;
            currentPlayerLoop = new PlayerLoopSystem();
        }
        if (GUILayout.Button("Open Docs"))
        {
            Application.OpenURL("https://docs.unity3d.com/2018.1/Documentation/ScriptReference/Experimental.LowLevel.PlayerLoopSystem.html");
        }
        GUILayout.EndHorizontal();
    }

    PlayerLoopSystem GenerateCustomLoop()
    {
        // Note: this also resets the loop to its defalt state first.
        var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
        hasCustomPlayerLoop = true;

        // Grab the 4th subsystem - This is the man Update Phase
        var update = playerLoop.subSystemList[4];

        // convert the subsytem array to a List to make it easier to work with...
        var newList = new List<PlayerLoopSystem>(update.subSystemList);

        // add a demo system to the start of it (implementation at the end of this file)
        PlayerLoopSystem beginUpdateSystem = new PlayerLoopSystem();
        beginUpdateSystem.type = typeof(CustomPlayerLoopStartUpdate); // Unity uses the name of the type here to identify the System - we would see this show up in the Profiler for example
        beginUpdateSystem.updateDelegate = CustomPlayerLoopStartUpdate.UpdateFunction; // we can plug a C# method into this delegate to control what actually happens when this System updates
        newList.Insert(0, beginUpdateSystem); // Finally lets insert it into the front!

        // Also lets put one on the end (implementation at the end of this file)
        newList.Add(CustomPlayerLoopEndUpdate.GetNewSystem()); // this time, lets use a small static helper method i added to the Systems type to generate the PlayerLoopSystem. this pattern is much cleaner :)

        // convert the list back to an array and plug it into the Update system.
        update.subSystemList = newList.ToArray();

        // dont forget to put our newly edited System back into the main player loop system!!
        playerLoop.subSystemList[4] = update;
        return playerLoop;
    }

    private Stack<string> pathStack = new Stack<string>();
    private Stack<PlayerLoopSystem> systemStack = new Stack<PlayerLoopSystem>();
    void DrawSubsystemList(PlayerLoopSystem system, int increment = 1)
    {
        // here were using a stack to generate a path name for the PlayerLoopSystem were currently trying to draw
        // e.g Update.ScriptRunBehaviourUpdate. Unity uses these path names when storing profiler data on a step
        // that means  we can use these path names to retrieve profiler samples!
        if (pathStack.Count == 0)
        {
            // if this is a root object, add its name to the stack
            pathStack.Push(system.type.Name);
        }
        else
        {
            // otherwise add its name to its parents name...
            pathStack.Push(pathStack.Peek() + "." + system.type.Name);
        }

        using (new EditorGUI.IndentLevelScope(increment))
        {
            // if this System has Subsystems, draw a foldout
            bool header = system.subSystemList != null;
            if (header)
            {
                var name = system.type.Name; var fullName = system.type.FullName;
                // check fold

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                bool fold = EditorGUILayout.Foldout(GetFoldout(fullName), name, true); // use the GetFoldout helper method to see if its open or closed
                EditorGUILayout.EndHorizontal();

                if (fold)
                {
                    // if the fold is open, draw all the Subsytems~
                    foreach (var loopSystem in system.subSystemList)
                    {
                        // store the current system Useful if we need to know the parent of a system later
                        systemStack.Push(system);
                        DrawSubsystemList(loopSystem);
                        systemStack.Pop();
                    }
                }

                SetFoldout(fullName, fold);
            }
            else
            {
                // at the moment, all the defaut 'native' Systems update via a updateFunction (essentally, a pointer into the unmanaged C++ side of the engine.
                // So we can tell if a system is a custom one because it has a value in updateDelegate instead. So if this is a custom system, make note of that
                // so we can change how its drawn later
                bool custom = system.updateDelegate != null;
                using (new EditorGUI.DisabledScope(custom))
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space((float)EditorGUI.indentLevel * 18f); // indent the entry nicley. We have to do this manually cos the flexible space at the end conflicts with GUI.Indent

                    // draw the remove button...
                    if (GUILayout.Button("x"))
                    {
                        RemoveSystem(system, systemStack.Peek());
                    }
                    GUILayout.Label(system.type.Name); // draw the name out....

                    // If the profiling mode is enabled, get the profiler sampler for this System and display its execution times!
                    if (getProfileTimingInfo)
                    {
                        var sampler = Sampler.Get(pathStack.Peek());

                        var info = "";
                        if (sampler.GetRecorder().elapsedNanoseconds != 0)
                        {
                            info = (sampler.GetRecorder().elapsedNanoseconds / 1000000f) + "ms";
                        }
                        else
                        {
                            info = "0.000000ms";
                        }

                        using (new EditorGUI.DisabledScope(true))
                        {
                            GUILayout.Label("[" + info + "]");
                        }
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    //EditorGUILayout.LabelField(new GUIContent(/*"custom"*/));//, EditorGUIUtility.IconContent("cs Script Icon"));
                }
            }
        }

        pathStack.Pop();
    }

    private void RemoveSystem(PlayerLoopSystem target, PlayerLoopSystem playerLoopSystem)
    {
        // LIMITATION assumes that systems are never stacked more than one level deep (e.g Update.CustomThing.CoolSystem will not work! one level too deep
        // Hell im not even sure if that works in general? it seems like it should but ive not tried it... still thought it was best to flag it up here...
        for (int i = 0; i < playerLoopSystem.subSystemList.Length; i++)
        {
            var system = playerLoopSystem.subSystemList[i];
            if (system.type == target.type)
            {
                // create a list of the subsystems, its easier to work with 
                var newList = new List<PlayerLoopSystem>(playerLoopSystem.subSystemList);

                // remove the target
                newList.RemoveAt(i);
                playerLoopSystem.subSystemList = newList.ToArray();

                nextPlayerLoop = currentPlayerLoop; // copy the current loop...
                // and plug in our updated parent into it.
                for (int j = 0; j < currentPlayerLoop.subSystemList.Length; j++)
                {
                    if (currentPlayerLoop.subSystemList[j].type == playerLoopSystem.type)
                    {
                        currentPlayerLoop.subSystemList[j] = playerLoopSystem;
                    }
                }
                // then flag that it needs to be applied at the end of the GUI draw
                hasUpdated = true;
            }
        }
    }

    public bool GetFoldout(string key)
    {
        return EditorPrefs.GetBool("PlayerLoopWin_Foldout_" + key, false);
    }

    public void SetFoldout(string key, bool value)
    {
        EditorPrefs.SetBool("PlayerLoopWin_Foldout_" + key, value);
    }

    private static string infoBox = @"The PlayerLoopSystem Struct represents a single system in the Player Loop. The loop is actually a tree-like structure, each System can store a list of subsystems (PlayerLoopSystem.subSystemList). In this visualization, we have drawn each top-level system as a fold out containing its subsystems.  This makes it super easy for us to inspect exactly what is happening in each main phase of a frame update.

We can plug in our own PlayerLoopSystem by adding it to the subsystem list for whichever update phase we want it to execute in. Click the 'Add/Remove Custom System button' to add two demo systems to the Update Phase. We can even remove whole subsystems from the update! to experiment with this click the [x] button next to a systems entry.";
}

public struct CustomPlayerLoopStartUpdate
{
    public static PlayerLoopSystem GetNewSystem()
    {
        return new PlayerLoopSystem()
        {
            type = typeof(CustomPlayerLoopStartUpdate),
            updateDelegate = UpdateFunction
        };
    }

    public static void UpdateFunction()
    {
        // TODO: something useful here!
        Debug.Log("Starting Update");
    }
}

public struct CustomPlayerLoopEndUpdate
{
    public static PlayerLoopSystem GetNewSystem()
    {
        return new PlayerLoopSystem()
        {
            type = typeof(CustomPlayerLoopEndUpdate),
            updateDelegate = UpdateFunction
        };
    }

    public static void UpdateFunction()
    {
        // TODO: something useful here!
        Debug.Log("Ending Update");
    }
}