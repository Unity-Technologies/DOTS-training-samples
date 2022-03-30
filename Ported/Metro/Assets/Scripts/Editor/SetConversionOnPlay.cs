using UnityEditor;


[InitializeOnLoadAttribute]
public static class SetConversionOnPlay
{
    private const string LIVE_CONVERSION_EDITOR_MENU = "DOTS/Conversion Settings/";
    private const string AUTHORING_STATE = LIVE_CONVERSION_EDITOR_MENU   + "Live Conversion: Authoring State in Scene View";
    private const string RUNTIME_STATE = LIVE_CONVERSION_EDITOR_MENU + "Live Conversion: Runtime State in Scene View";
    private const string SWITCH_STATE = LIVE_CONVERSION_EDITOR_MENU + "Live Conversion: Automatically Switch Authoring";

    private const string ENABLED_KEY = "DOTS_AUTOMATIC_AUTHORING";

    static SetConversionOnPlay()
    {
        var state = EditorPrefs.GetBool(ENABLED_KEY);
        SubscribeToEvent(state);
    }

    [MenuItem(SWITCH_STATE, false, 11)]
    private static void AutomaticAuthoring()
    {
        var state = !EditorPrefs.GetBool(ENABLED_KEY);
        EditorPrefs.SetBool(ENABLED_KEY, state);
        SubscribeToEvent(state);
    }
    
    [MenuItem(SWITCH_STATE, true)]
    private static bool ValidateToggleInEditMode()
    {
        var state = EditorPrefs.GetBool(ENABLED_KEY);
        Menu.SetChecked(SWITCH_STATE, state);
        return true;
    }

    private static void SubscribeToEvent(bool state)
    {
        if (state)
        {
            EditorApplication.playModeStateChanged += ModeStateChanged;
        }
        else
        {
            EditorApplication.playModeStateChanged -= ModeStateChanged;
        }
    }
    
    private static void ModeStateChanged (PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorApplication.ExecuteMenuItem(RUNTIME_STATE);
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.ExecuteMenuItem(AUTHORING_STATE);
        }
    }
}