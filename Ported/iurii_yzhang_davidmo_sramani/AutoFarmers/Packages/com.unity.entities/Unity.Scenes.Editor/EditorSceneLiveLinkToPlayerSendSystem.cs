using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine.Networking.PlayerConnection;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes.Editor
{
    class EditorSceneLiveLinkToPlayerSendSystem : ScriptableSingleton<EditorSceneLiveLinkToPlayerSendSystem>
    {
        //@TODO: Multi-world connection support...
        Dictionary<int, LiveLinkConnection> _Connections = new Dictionary<int, LiveLinkConnection>();
        
        // Temp data cached to reduce gc allocations
        List<LiveLinkChangeSet>             _ChangeSets = new List<LiveLinkChangeSet>();
        NativeList<Hash128>                 _UnloadScenes;
        NativeList<Hash128>                 _LoadScenes;

        unsafe void SetLoadedScenes(MessageEventArgs args)
        {
            if (!_Connections.TryGetValue(args.playerId, out var connection))
            {
                Debug.LogError("SetLoadedScenes was sent but the connection has not been created");
                return;
            }

            var msg = LiveLinkSceneMsg.FromMsg(args.data, Allocator.TempJob);
            LiveLinkMsg.LogReceived($"SetLoadedScenes: Loaded {msg.LoadedScenes.ToDebugString()}, Removed {msg.RemovedScenes.ToDebugString()}");
            connection.ApplyLiveLinkSceneMsg(msg);
            msg.Dispose();
        }

        void ConnectLiveLink(MessageEventArgs args)
        {
            LiveLinkMsg.LogReceived("ConnectLiveLink");

            int player = args.playerId;
            var buildSettings = args.Receive<Hash128>();

            //@TODO: Implement this properly
            //system.World.GetExistingSystem<EditorSubSceneLiveLinkSystem>().CleanupAllScenes();

            //@TODO: How does this work with multiple connections?
            LiveLinkAssetBundleBuildSystem.instance.ClearUsedAssetsTargetHash();
            if (_Connections.TryGetValue(player, out var connection))
                connection.Dispose();
                
            _Connections[player] = new LiveLinkConnection(buildSettings);

            TimeBasedCallbackInvoker.SetCallback(DetectSceneChanges);
            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
        }

        void OnPlayerConnected(int playerID)
        {
            LiveLinkMsg.LogInfo("OnPlayerConnected " + playerID);
        }

        void OnPlayerDisconnected(int playerID)
        {
            LiveLinkMsg.LogInfo("OnPlayerDisconnected " + playerID);
                
            if (_Connections.TryGetValue(playerID, out var connection))
            {
                connection.Dispose();
                _Connections.Remove(playerID);
            }
        }

        static void SendChangeSet(LiveLinkChangeSet entityChangeSet, int playerID)
        {
            var buffer = entityChangeSet.Serialize();
            LiveLinkMsg.LogSend($"EntityChangeSet patch: '{buffer.Length}' bytes, scene '{entityChangeSet.SceneGUID}'");
            EditorConnection.instance.Send(LiveLinkMsg.ReceiveEntityChangeSet, buffer, playerID);
        }
        
        static void SendUnloadScenes(NativeArray<Hash128> unloadScenes, int playerID)
        {
            if (unloadScenes.Length == 0)
                return;

            LiveLinkMsg.LogSend($"UnloadScenes {unloadScenes.ToDebugString()}");
            EditorConnection.instance.SendArray(LiveLinkMsg.UnloadScenes, unloadScenes, playerID);
        }

        static void SendLoadScenes(NativeArray<Hash128> loadScenes, int playerID)
        {
            if (loadScenes.Length == 0)
                return;

            LiveLinkMsg.LogSend($"LoadScenes {loadScenes.ToDebugString()}");
            EditorConnection.instance.SendArray(LiveLinkMsg.LoadScenes, loadScenes, playerID);
        }
        
        void DetectSceneChanges()
        {
            if (_Connections.Count == 0)
            {
                TimeBasedCallbackInvoker.ClearCallback(DetectSceneChanges);
                LiveLinkAssetBundleBuildSystem.instance.ClearUsedAssetsTargetHash();
                return;
            }

            foreach (var c in _Connections)
            {
                try
                {
                    var connection = c.Value;
                    connection.Update(_ChangeSets, _LoadScenes, _UnloadScenes, LiveLinkMode.LiveConvertGameView);

                    // Load scenes that are not being edited live
                    SendLoadScenes(_LoadScenes.AsArray(), c.Key);
                    // Unload scenes that are no longer being edited / need to be reloaded etc
                    SendUnloadScenes(_UnloadScenes.AsArray(), c.Key);
                    
                    // Apply changes to scenes that are being edited
                    foreach (var change in _ChangeSets)
                    {
                        SendChangeSet(change, c.Key);
                        change.Dispose();
                    }
                }
                finally
                {
                    _ChangeSets.Clear();
                    _UnloadScenes.Clear();
                    _LoadScenes.Clear();
                }
            }
        }

        void OnEnable()
        {
            _UnloadScenes = new NativeList<Hash128>(Allocator.Persistent);
            _LoadScenes = new NativeList<Hash128>(Allocator.Persistent);

            EditorConnection.instance.Register(LiveLinkMsg.ConnectLiveLink, ConnectLiveLink);
            EditorConnection.instance.Register(LiveLinkMsg.SetLoadedScenes, SetLoadedScenes);
            EditorConnection.instance.RegisterConnection(OnPlayerConnected);
            EditorConnection.instance.RegisterDisconnection(OnPlayerDisconnected);

            // After domain reload we need to reconnect all data to the player.
            // Optimally we would keep all state alive across domain reload...
            LiveLinkMsg.LogSend("ResetGame");
            EditorConnection.instance.Send(LiveLinkMsg.ResetGame, new byte[0]);
        }

        void OnDisable()
        {
            EditorConnection.instance.Unregister(LiveLinkMsg.ConnectLiveLink, ConnectLiveLink);
            EditorConnection.instance.Unregister(LiveLinkMsg.SetLoadedScenes, SetLoadedScenes);
            EditorConnection.instance.UnregisterConnection(OnPlayerConnected);
            EditorConnection.instance.UnregisterDisconnection(OnPlayerDisconnected);

            foreach (var connection in _Connections)
                connection.Value.Dispose();
            _Connections.Clear();

            _UnloadScenes.Dispose();
            _LoadScenes.Dispose();
        }
    }
}