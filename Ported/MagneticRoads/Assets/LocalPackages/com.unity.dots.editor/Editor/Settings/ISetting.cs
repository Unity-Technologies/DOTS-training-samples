using Unity.Properties;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Interface that allows to declare a class type as a setting.
    /// </summary>
    interface ISetting
    {
        /// <summary>
        /// Method called when a change is detected in the UI.
        /// </summary>
        /// <param name="path">Path to the changed property.</param>
        void OnSettingChanged(PropertyPath path);
    }
}
