using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Scenes;
using Unity.Scenes.Editor;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace Unity.Entities.Editor
{
    class LiveLinkConnectionsDropdown : PopupWindowContent, IDisposable
    {
        readonly List<LiveLinkConnection> m_LinkConnections = new List<LiveLinkConnection>();
        EditorConnectionWatcher m_EditorConnectionWatcher;

        public LiveLinkConnectionsDropdown()
        {
            m_EditorConnectionWatcher = EditorConnectionWatcher.instance;

            m_EditorConnectionWatcher.PlayerConnected += OnPlayerConnected;
            m_EditorConnectionWatcher.PlayerDisconnected += OnPlayerDisconnected;

            foreach (var connectedPlayer in EditorConnection.instance.ConnectedPlayers)
            {
                m_LinkConnections.Add(new LiveLinkConnection(connectedPlayer.playerId, connectedPlayer.name, LiveLinkConnectionStatus.Connected));
            }
        }

        void OnPlayerDisconnected(int playerId)
        {
            m_LinkConnections.RemoveAll(x => x.PlayerId == playerId);
            LiveLinkToolbar.RepaintPlaybar();
        }

        void OnPlayerConnected(int playerId)
        {
            if (m_LinkConnections.Any(x => x.PlayerId == playerId))
                return;

            var connectedPlayer = EditorConnection.instance.ConnectedPlayers.Find(x => x.playerId == playerId);
            if (connectedPlayer == null)
                return;

            m_LinkConnections.Add(new LiveLinkConnection(connectedPlayer.playerId, connectedPlayer.name, LiveLinkConnectionStatus.Connected));
            LiveLinkToolbar.RepaintPlaybar();
        }

        public void DrawDropdown()
        {
            var dropdownRect = new Rect(172, 0, 40, 22);
            var hasConnectedDevices = m_LinkConnections.Any(c => c.Status == LiveLinkConnectionStatus.Connected);
            var icon = hasConnectedDevices ? Icons.LiveLinkOn : Icons.LiveLink;
            icon.tooltip = hasConnectedDevices
                    ? "View linked devices."
                    : "No devices currently linked. Create a Live Link build to connect a device.";

            if (EditorGUI.DropdownButton(dropdownRect, icon, FocusType.Keyboard, LiveLinkStyles.Dropdown))
            {
                PopupWindow.Show(dropdownRect, this);
            }
        }

        public override Vector2 GetWindowSize() => SizeHelper.GetDropdownSize(m_LinkConnections.Count);

        public override void OnOpen()
        {
            const string basePath = "Packages/com.unity.entities/Editor/Resources/LiveLink";
            var template = m_LinkConnections.Count == 0
                ? UIElementHelpers.LoadTemplate(basePath, "LiveLinkConnectionsDropdown.Empty", "LiveLinkConnectionsDropdown")
                : UIElementHelpers.LoadTemplate(basePath, "LiveLinkConnectionsDropdown");

            if (m_LinkConnections.Count > 0)
            {
                var placeholder = template.Q<VisualElement>("devices");
                var tpl = UIElementHelpers.LoadClonableTemplate(basePath, "LiveLinkConnectionsDropdown.ItemTemplate");
                foreach (var connection in m_LinkConnections)
                {
                    var item = tpl.GetNewInstance();
                    item.Q<Image>().AddToClassList(GetStatusClass(connection.Status));
                    var label = item.Q<Label>();
                    label.text = connection.Name.Length <= SizeHelper.MaxCharCount ? connection.Name : connection.Name.Substring(0, SizeHelper.MaxCharCount) + "...";
                    if (connection.Name.Length > SizeHelper.MaxCharCount) label.tooltip = connection.Name;
                    placeholder.Add(item);
                }
            }

            var footer = UIElementHelpers.LoadTemplate(basePath, "LiveLinkConnectionsDropdown.Footer", "LiveLinkConnectionsDropdown");
            var buildButton = footer.Q<Button>("live-link-connections-dropdown__footer__build");
            var resetButton = footer.Q<Button>("live-link-connections-dropdown__footer__reset");
            var clearButton = footer.Q<Button>("live-link-connections-dropdown__footer__clear");
            if (LiveLinkBuildSettings.CurrentLiveLinkBuildSettings != null)
            {
                buildButton.clickable.clicked += LiveLinkCommands.BuildAndRunLiveLinkPlayer;
            }
            else
            {
                buildButton.SetEnabled(false);
                buildButton.tooltip = "Please first select a build setting.";
            }

            if (m_LinkConnections.Count > 0)
            {
                resetButton.clickable.clicked += LiveLinkCommands.ResetPlayer;
                clearButton.clickable.clicked += LiveLinkCommands.ClearLiveLinkCache;
            }
            else
            {
                resetButton.SetEnabled(false);
                clearButton.SetEnabled(false);
            }

            template.Add(footer);
            editorWindow.rootVisualElement.Add(template);
        }

        static string GetStatusClass(LiveLinkConnectionStatus connectionStatus)
        {
            if ((connectionStatus & LiveLinkConnectionStatus.Error) == LiveLinkConnectionStatus.Error)
                return "live-link-connections-dropdown__status--error";
            if ((connectionStatus & LiveLinkConnectionStatus.Connected) == LiveLinkConnectionStatus.Connected)
                return "live-link-connections-dropdown__status--connected";
            if ((connectionStatus & LiveLinkConnectionStatus.SoftDisconnected) == LiveLinkConnectionStatus.SoftDisconnected)
                return "live-link-connections-dropdown__status--soft-disconnected";

            return null;
        }

        public override void OnGUI(Rect rect) { }

        public void Dispose()
        {
            if (m_EditorConnectionWatcher == null)
                return;

            m_EditorConnectionWatcher.PlayerConnected -= OnPlayerConnected;
            m_EditorConnectionWatcher.PlayerDisconnected -= OnPlayerDisconnected;
            m_EditorConnectionWatcher = null;
        }

        class LiveLinkConnection
        {
            public int PlayerId { get; }
            public string Name { get; }

            public LiveLinkConnectionStatus Status { get; }

            public LiveLinkConnection(int playerId, string name, LiveLinkConnectionStatus status)
            {
                PlayerId = playerId;
                Name = name;
                Status = status;
            }
        }

        [Flags]
        enum LiveLinkConnectionStatus
        {
            None = 0,
            Connected = 1,
            SoftDisconnected = 2,
            Error = 4,
        }

        class EditorConnectionWatcher : ScriptableSingleton<EditorConnectionWatcher>
        {
            EditorConnection m_EditorConnection;

            public event Action<int> PlayerConnected = delegate { };
            public event Action<int> PlayerDisconnected = delegate { };

            void OnPlayerConnected(int playerId) => PlayerConnected(playerId);
            void OnPlayerDisconnected(int playerId) => PlayerDisconnected(playerId);

            void OnEnable()
            {
                m_EditorConnection = EditorConnection.instance;
                m_EditorConnection.RegisterConnection(OnPlayerConnected);
                m_EditorConnection.RegisterDisconnection(OnPlayerDisconnected);
            }

            void OnDestroy() => Unregister();

            void Unregister()
            {
                if (m_EditorConnection == null)
                    return;

                m_EditorConnection.UnregisterConnection(OnPlayerConnected);
                m_EditorConnection.UnregisterDisconnection(OnPlayerDisconnected);
                m_EditorConnection = null;
            }

            ~EditorConnectionWatcher() => Unregister();
        }

    }

    static class SizeHelper
    {
        static readonly Vector2 s_EmptyDropdownSize = new Vector2(205, 110);
        const int Width = 250;
        const int ItemHeight = 19;
        const int PaddingHeight = 110;

        internal const int MaxCharCount = 30;

        public static Vector2 GetDropdownSize(int itemCount)
            => itemCount == 0 ? s_EmptyDropdownSize : new Vector2(Width, itemCount * ItemHeight + PaddingHeight);
    }
}