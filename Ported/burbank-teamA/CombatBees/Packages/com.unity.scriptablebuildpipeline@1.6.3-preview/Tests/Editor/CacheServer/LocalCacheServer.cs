using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.CacheServerTests
{
    internal class LocalCacheServer : ScriptableSingleton<LocalCacheServer>
    {
        [SerializeField] public string m_path;
        [SerializeField] public int m_port;
        [SerializeField] public int m_pid = -1;

        private void Create(int port, ulong size, string cachePath)
        {
            var nodeExecutable = Utils.Paths.Combine(EditorApplication.applicationContentsPath, "Tools", "nodejs");
            nodeExecutable = Application.platform == RuntimePlatform.WindowsEditor 
                ? Utils.Paths.Combine(nodeExecutable, "node.exe") 
                : Utils.Paths.Combine(nodeExecutable, "bin", "node");

            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);
            
            m_path = cachePath;
            
            var cacheServerJs = Utils.Paths.Combine(EditorApplication.applicationContentsPath, "Tools", "CacheServer", "main.js");
            var processStartInfo = new ProcessStartInfo(nodeExecutable)
            {
                Arguments = "\"" + cacheServerJs + "\""
                    + " --port " + port
                    + " --path \"" + m_path
                    + "\" --nolegacy"
                    + " --monitor-parent-process " + Process.GetCurrentProcess().Id
                    // node.js has issues running on windows with stdout not redirected.
                    // so we silence logging to avoid that. And also to avoid CacheServer
                    // spamming the editor logs on OS X.
                    + " --silent"
                    + " --size " + size,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var p = new Process {StartInfo = processStartInfo};
            p.Start();

            m_port = port;
            m_pid = p.Id;
            Save(true);
        }

        public static string CachePath
        {
            get { return instance.m_path; }
        }

        public static int Port
        {
            get { return instance.m_port; }
        }

        public static void Setup(ulong size, string cachePath)
        {
            Kill();
            instance.Create(GetRandomUnusedPort(), size, cachePath);
            WaitForServerToComeAlive(instance.m_port);
        }
        
        public static void Kill()
        {
            if (instance.m_pid == -1)
                return;

            try
            {
                var p = Process.GetProcessById(instance.m_pid);
                p.Kill();
                instance.m_pid = -1;
            }
            catch
            {
                // if we could not get a process, there is non alive. continue.
            }
        }

        public static void Clear()
        {
            Kill();
            if (Directory.Exists(instance.m_path))
                Directory.Delete(instance.m_path, true);
        }
        
        private static void WaitForServerToComeAlive(int port)
        {
            var start = DateTime.Now;
            var maximum = start.AddSeconds(5);
            while (DateTime.Now < maximum)
            {
                if (!PingHost("localhost", port, 10)) continue;
                Console.WriteLine("Server Came alive after {0} ms", (DateTime.Now - start).TotalMilliseconds);
                break;
            }
        }
       
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static bool PingHost(string host, int port, int timeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(timeout));
                    return client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
