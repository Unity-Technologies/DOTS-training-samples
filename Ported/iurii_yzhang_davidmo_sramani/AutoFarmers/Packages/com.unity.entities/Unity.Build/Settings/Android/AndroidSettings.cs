using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Unity.Properties;

namespace Unity.Build
{
    // Todo: should live inside com.unity.platforms.android
    public sealed class AndroidSettings : IBuildSettingsComponent
    {
        string m_PackageName = "com.unity.DefaultPackage";
        int m_MinAPILevel = 19;
        int m_TargetAPILevel = 21;

        internal static readonly Dictionary<int, string> s_AndroidCodeNames = new Dictionary<int, string>
        {
            { 19, "Android 4.4 'KitKat' (API level 19)" },
            { 20, "Android 4.4W 'KitKat' (API level 20)" },
            { 21, "Android 5.0 'Lollipop' (API level 21)" },
            { 22, "Android 5.1 'Lollipop' (API level 22)" },
            { 23, "Android 6.0 'Marshmallow' (API level 23)" },
            { 24, "Android 7.0 'Nougat' (API level 24)" },
            { 25, "Android 7.1 'Nougat' (API level 25)" },
            { 26, "Android 8.0 'Oreo' (API level 26)" },
            { 27, "Android 8.1 'Oreo' (API level 27)" },
            { 28, "Android 9.0 'Pie' (API level 28)" },
        };

        [Property]
        public string PackageName
        {
            get => m_PackageName;
            set => m_PackageName = value ?? "com.unity.DefaultPackage";
        }

        [Property]
        public int MinAPILevel
        {
            get => m_MinAPILevel;
            set => m_MinAPILevel = s_AndroidCodeNames.ContainsKey(value) ? value : s_AndroidCodeNames.Keys.First();
        }

        [Property]
        public int TargetAPILevel
        {
            get => m_TargetAPILevel;
            set => m_TargetAPILevel = s_AndroidCodeNames.ContainsKey(value) ? value : s_AndroidCodeNames.Keys.First();
        }

        [Property]
        public AndroidArchitecture TargetArchitectures { get; set; } = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
    }
}
 