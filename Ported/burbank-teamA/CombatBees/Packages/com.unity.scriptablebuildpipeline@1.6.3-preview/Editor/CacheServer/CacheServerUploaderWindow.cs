using System;
using UnityEngine;

namespace UnityEditor.Build.CacheServer
{
    /// <summary>
    /// The Cache Server Uploader window.  This interface will upload your assets to a the given address of a Cache Server.
    /// </summary>
    public class CacheServerUploaderWindow : EditorWindow
    {
        private string m_address;

        private void Awake()
        {
            m_address = Util.ConfigCacheServerAddress;
            titleContent = new GUIContent("CS Upload");
        }

        private bool ValidateAddress()
        {
            string host;
            int port;
            Util.ParseCacheServerIpAddress(m_address, out host, out port);
            
            var c = new Client(host, port);
            try
            {
                c.Connect();
                c.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }

            return true;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Cache Server Address: ");
            m_address = GUILayout.TextField(m_address);
            EditorGUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Upload") && EditorUtility.DisplayDialog("Upload to Cache Server", 
                    "This will upload all assets in your Library folder to the specified Cache Server.", "Continue", "Cancel"))
            {
                GetWindow<CacheServerUploaderWindow>().Close();
                if (!ValidateAddress())
                {
                    Debug.LogError("Could not connect to Cache Server");
                    return;
                }
              
                string host;
                int port;
                Util.ParseCacheServerIpAddress(m_address, out host, out port);
                CacheServerUploader.UploadAllFilesToCacheServer(host, port);
            }

            if (GUILayout.Button("Cancel"))
            {
                GetWindow<CacheServerUploaderWindow>().Close();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
        
        
        [MenuItem("Assets/Cache Server/Upload All Assets")]
        public static void UploadAllFilesToCacheServerMenuItem()
        {
            var window = GetWindow<CacheServerUploaderWindow>();
            window.ShowUtility();
        }
    }
}