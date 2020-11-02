using System;

namespace Unity.Entities.Editor
{
    static class LiveLinkConfigHelper
    {
        static readonly Action<Action> k_AddHandler;
        static readonly Action<Action> k_RemoveHandler;
        static readonly Func<bool> k_GetLiveLinkEnabledInEditMode;
        static readonly Action<bool> k_SetLiveLinkEnabledInEditMode;

        static LiveLinkConfigHelper()
        {
            var type = Type.GetType("Unity.Scenes.Editor.SubSceneInspectorUtility, Unity.Scenes.Editor");
            if (type == null)
                return;

            var property = type.GetProperty("LiveLinkEnabledInEditMode");
            if (property == null)
                return;

            var @event = type.GetEvent("LiveLinkModeChanged");
            if (@event == null)
                return;

            k_AddHandler = (Action<Action>)@event.AddMethod.CreateDelegate(typeof(Action<Action>));
            k_RemoveHandler = (Action<Action>)@event.RemoveMethod.CreateDelegate(typeof(Action<Action>));
            k_GetLiveLinkEnabledInEditMode = (Func<bool>)property.GetMethod.CreateDelegate(typeof(Func<bool>));
            k_SetLiveLinkEnabledInEditMode = (Action<bool>)property.SetMethod.CreateDelegate(typeof(Action<bool>));
        }

        internal static bool IsProperlyInitialized => k_GetLiveLinkEnabledInEditMode != null
                                                      && k_SetLiveLinkEnabledInEditMode != null
                                                      && k_AddHandler != null
                                                      && k_RemoveHandler != null;

        public static bool LiveLinkEnabledInEditMode
        {
            get => k_GetLiveLinkEnabledInEditMode();
            set => k_SetLiveLinkEnabledInEditMode(value);
        }

        public static event Action LiveLinkEnabledChanged
        {
            add => k_AddHandler(value);
            remove => k_RemoveHandler(value);
        }
    }
}
