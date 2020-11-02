namespace Unity.Entities.Editor
{
    abstract class InspectorBinding<TValue> : PreferenceBinding<InspectorSettings, TValue>
    {
        protected sealed override string SettingsKey { get; } = Constants.Settings.Inspector;
    }
}
