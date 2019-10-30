using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEditor;

namespace Unity.Platforms
{
    internal static class NodeTools
    {
        private static string ToolsManagerNativeName
        {
            get
            {
                var toolsManager = "DotsEditorTools";
#if UNITY_EDITOR_WIN
                return $"{toolsManager}-win";
#elif UNITY_EDITOR_OSX
                return $"{toolsManager}-macos";
#elif UNITY_EDITOR_LINUX
                return $"{toolsManager}-linux";
#else
#error not implemented
#endif
            }
        }

        public static bool Run(string name, params string[] arguments)
        {
            var toolDir = new DirectoryInfo(Path.Combine("Library", "DotsEditorTools", "manager"));
            var result = Shell.Run(new ShellProcessArgs()
            {
                Executable = Path.Combine(toolDir.FullName, ToolsManagerNativeName),
                Arguments = new[] { name }.Concat(arguments),
                WorkingDirectory = toolDir
            });
            return result.Succeeded;
        }

        public static Process RunAsync(string name, params string[] arguments)
        {
            var toolDir = new DirectoryInfo(Path.Combine("Library", "DotsEditorTools", "manager"));
            return Shell.RunAsync(new ShellProcessArgs()
            {
                Executable = Path.Combine(toolDir.FullName, ToolsManagerNativeName),
                Arguments = new[] { name }.Concat(arguments),
                WorkingDirectory = toolDir
            });
        }
    }

    public abstract class BasicServer
    {
        protected BasicServer(string name)
        {
            Name = name;

            // Restore process if its already running
            try
            {
                ServerProcess = BackupPID > 0 ? Process.GetProcessById(BackupPID) : null;
            }
            catch (ArgumentException)
            {
                ServerProcess = null;
            }

            if (ServerProcess == null || ServerProcess.HasExited)
            {
                BackupPID = 0;
                BackupPort = 0;
            }
            else
            {
                Port = BackupPort;
                Listening = true;
            }
        }

        protected enum ServerEvent { Connected, DataReceived, Disconnected, Broadcast, Reconnect };
        private Process ServerProcess { get; set; }
        protected string Name { get; set; }
        protected abstract string[] ShellArgs { get; }
        public bool Listening { get; private set; }
        public int Port { get; private set; }
        public abstract Uri URL { get; }

        private int BackupPID
        {
            get => EditorPrefs.GetInt($"Unity.Editor.Tools.{Name}.PID", 0);
            set => EditorPrefs.SetInt($"Unity.Editor.Tools.{Name}.PID", value);
        }

        private int BackupPort
        {
            get => EditorPrefs.GetInt($"Unity.Editor.Tools.{Name}.Port", 0);
            set => EditorPrefs.SetInt($"Unity.Editor.Tools.{Name}.Port", value);
        }

        public static string LocalIP
        {
            get
            {
                string localIP;
                try
                {
                    // Connect a UDP socket and read its local endpoint. This is more accurate
                    // way when there are multi ip addresses available on local machine.
                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Connect("8.8.8.8", 65530);
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        localIP = endPoint.Address.ToString();
                    }
                }
                catch (SocketException)
                {
                    // Network unreachable? Use loopback address
                    localIP = "127.0.0.1";
                }
                return localIP;
            }
        }

        private bool SetupProcess()
        {
            // Start server
            ServerProcess = NodeTools.RunAsync(Name.ToLower(), ShellArgs);

            // Check if server process state is valid
            if (ServerProcess == null || ServerProcess.HasExited)
            {
                var msg = $"Failed to start {Name}.";
                UnityEngine.Debug.LogError(msg);
                Close();
                return false;
            }
            return true;
        }

        public virtual bool Listen(int port)
        {
            if (Listening)
            {
                return true;
            }

            Port = port;
            if (!SetupProcess())
            {
                return false;
            }

            BackupPID = ServerProcess?.Id ?? 0;
            BackupPort = Port;
            Listening = true;
            return true;
        }

        public virtual void Close()
        {
            if (!Listening)
            {
                return;
            }

            Listening = false;
            BackupPort = 0;
            BackupPID = 0;

            if (ServerProcess != null)
            {
                if (!ServerProcess.HasExited)
                {
                    ServerProcess.Kill();
                }
                ServerProcess.Dispose();
                ServerProcess = null;
            }
        }
    }
}
