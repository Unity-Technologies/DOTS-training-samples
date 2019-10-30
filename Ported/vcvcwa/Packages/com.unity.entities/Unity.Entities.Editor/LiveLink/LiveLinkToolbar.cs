using Unity.Scenes.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.Entities.Editor
{
    class LiveLinkToolbar
    {
        static readonly LiveLinkConnectionsDropdown s_LinkConnectionsDropdown = new LiveLinkConnectionsDropdown();
        static readonly LiveLinkBuildSettingsDropdown s_LiveLinkBuildSettingsDropdown = new LiveLinkBuildSettingsDropdown();
        static readonly GUIContent[] s_PlayIcons = s_PlayIcons = new []
        {
            EditorGUIUtility.TrIconContent("PlayButton", "Play"),
            EditorGUIUtility.TrIconContent("PauseButton", "Pause"),
            EditorGUIUtility.TrIconContent("StepButton", "Step"),
            EditorGUIUtility.TrIconContent("PlayButtonProfile", "Profiler Play"),
            EditorGUIUtility.IconContent("PlayButton On"),
            EditorGUIUtility.IconContent("PauseButton On"),
            EditorGUIUtility.IconContent("StepButton On"),
            EditorGUIUtility.IconContent("PlayButtonProfile On")
        };

        ~LiveLinkToolbar()
        {
            s_LinkConnectionsDropdown.Dispose();
        }

        internal static void RepaintPlaybar()
        {
            InternalEditorUtility.RepaintAllViews();
        }

        [CommandHandler("DOTS/GUI/LiveLinkToolbar", CommandHint.UI)]
        static void DrawPlaybar(CommandExecuteContext ctx)
        {
            // Enter / Exit Playmode
            var isOrWillEnterPlaymode = EditorApplication.isPlayingOrWillChangePlaymode;
            var isPlaying = EditorApplication.isPlaying;
            GUI.changed = false;

            var buttonOffset = isPlaying ? 4 : 0;

            var c = GUI.color + new Color(.01f, .01f, .01f, .01f);
            GUI.contentColor = new Color(1.0f / c.r, 1.0f / c.g, 1.0f / c.g, 1.0f / c.a);
            GUI.SetNextControlName("ToolbarPlayModePlayButton");
            GUILayout.Toggle(isOrWillEnterPlaymode, s_PlayIcons[buttonOffset], isPlaying ? LiveLinkStyles.CommandLeftOn : LiveLinkStyles.CommandLeft);
            GUI.backgroundColor = Color.white;
            if (GUI.changed)
            {
                TogglePlaying();
                GUIUtility.ExitGUI();
            }

            // Pause game
            GUI.changed = false;

            buttonOffset = EditorApplication.isPaused ? 4 : 0;
            GUI.SetNextControlName("ToolbarPlayModePauseButton");
            var isPaused = GUILayout.Toggle(EditorApplication.isPaused, s_PlayIcons[buttonOffset + 1], LiveLinkStyles.CommandMid);
            if (GUI.changed)
            {
                EditorApplication.isPaused = isPaused;
                GUIUtility.ExitGUI();
            }

            using (new EditorGUI.DisabledScope(!isPlaying))
            {
                // Step playmode
                GUI.SetNextControlName("ToolbarPlayModeStepButton");
                if (GUILayout.Button(s_PlayIcons[2], LiveLinkStyles.CommandRight))
                {
                    EditorApplication.Step();
                    GUIUtility.ExitGUI();
                }
            }

            s_LiveLinkBuildSettingsDropdown.DrawDropdown();
            s_LinkConnectionsDropdown.DrawDropdown();
        }

        static void TogglePlaying()
        {
            bool willPlay = !EditorApplication.isPlaying;
            EditorApplication.isPlaying = willPlay;

            InternalEditorUtility.RepaintAllViews();
        }
    }
}