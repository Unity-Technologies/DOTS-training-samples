using System;

namespace Unity.Build
{
    /// <summary>
    /// Attribute for hiding build steps from the GUI and specifying a display name with only a Type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BuildStepAttribute : Attribute
    {
        /// <summary>
        /// Flags types for build steps.
        /// </summary>
        public enum Flags
        {
            None = 0,
            Hidden = 1
        }

        /// <summary>
        /// Flags for the build step.
        /// </summary>
        public Flags flags = Flags.None;

        /// <summary>
        /// Description name for type.  If set, this will be used instead of the class name when selecting new steps in the GUI.
        /// </summary>
        public string description = "";

        /// <summary>
        /// Optional category used to put build step in its own sub menu when selectiong from the dropdown.
        /// </summary>
        public string category = "";
    }
}
