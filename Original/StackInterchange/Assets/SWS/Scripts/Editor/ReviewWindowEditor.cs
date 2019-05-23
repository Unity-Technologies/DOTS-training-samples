using System.IO;
using UnityEditor;
using UnityEngine;

namespace SWS
{
    [InitializeOnLoad]
    public class ReviewWindowEditor : EditorWindow
    {
		private static Texture2D reviewWindowImage;
		private static string imagePath = "/EditorFiles/Asset_smallLogo.png";
		private static string keyName = "SimpleWaypointSystem-Review";

		//data:
		//active, counter, lastCheck
		
        static ReviewWindowEditor()
        {
			EditorApplication.update += Startup;
		}
		
		
		static void Startup()
		{
			EditorApplication.update -= Startup;
			
			if (!EditorPrefs.HasKey(keyName))
			{
				string[] data = new string[3];
                data[0] = "true;";
				data[1] = "0;";
				data[2] = "0";
				EditorPrefs.SetString(keyName, data[0] + data[1] + data[2]);
			}
			
			Count();
		}


        [MenuItem("Window/Simple Waypoint System/Review Asset")]
        static void Init()
        {
            EditorWindow.GetWindowWithRect(typeof(ReviewWindowEditor), new Rect(0, 0, 256, 260), false, "Review Window");
        }


        void OnGUI()
        {		
			if(reviewWindowImage == null)
			{
				var script = MonoScript.FromScriptableObject(this);
				string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(script));
				reviewWindowImage = AssetDatabase.LoadAssetAtPath(path + imagePath, typeof(Texture2D)) as Texture2D;
			}
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(-2);
			GUILayout.Label(reviewWindowImage);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(30);
			EditorGUILayout.LabelField("Review Simple Waypoint System", GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please consider giving us a rating on the");
            EditorGUILayout.LabelField("Unity Asset Store. Your support helps us");
			EditorGUILayout.LabelField("to improve this product. Thank you!");

			GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Rate now!"))
            {
				Help.BrowseURL("https://www.assetstore.unity3d.com/#!/content/2506");
                DisableRating();
            }
            if (GUILayout.Button("Later"))
            {			
				string[] data = new string[3];
				data = EditorPrefs.GetString(keyName).Split(';');
				data[0] = data[0] + ";";
				data[1] = "5;";
				
				EditorPrefs.SetString(keyName, data[0] + data[1] + data[2]);
				this.Close();
            }
			if (GUILayout.Button("Never"))
            {
                DisableRating();
            }
            EditorGUILayout.EndHorizontal();
        }
		
		
		static void Count()
		{
			string[] data = new string[3];
			data = EditorPrefs.GetString(keyName).Split(';');
						
			if(data[0] == "false")
				return;
			
			double time = GenerateUnixTime();
			double diff = time - double.Parse(data[2]);
			int counter = int.Parse(data[1]);
			if(diff < 20) return;
			
			data[0] = data[0] + ";";
			data[1] = (counter + 1) + ";";
			data[2] = time.ToString();
			EditorPrefs.SetString(keyName, data[0] + data[1] + data[2]);
						
			if(counter > 6)
				Init();
		}
		
		
        static double GenerateUnixTime()
        {
            var epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            return (System.DateTime.UtcNow - epochStart).TotalHours;
        }
	

        void DisableRating()
        {
			string[] data = new string[3];
			data = EditorPrefs.GetString(keyName).Split(';');
			
			data[0] = "false;";
			data[1] = "0;";
			EditorPrefs.SetString(keyName, data[0] + data[1] + data[2]);
			this.Close();
        }
    }
}