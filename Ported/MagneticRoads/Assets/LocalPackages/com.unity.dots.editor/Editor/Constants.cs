namespace Unity.Entities.Editor
{
    static class Constants
    {
        public const string PackageName = "com.unity.dots.editor";
        public const string PackagePath = "Packages/" + PackageName;

        public const string EditorDefaultResourcesPath = PackagePath + "/Editor Default Resources/";

        public static class Conversion
        {
            public const string SelectedComponentSessionKey = "Conversion.Selected.{0}.{1}";
            public const string ShowAdditionalEntitySessionKey = "Conversion.ShowAdditional.{0}";
            public const string SelectedAdditionalEntitySessionKey = "Conversion.Additional.{0}";
        }

        public static class MenuItems
        {
            public const int WindowPriority = 3006;
            public const string SystemScheduleWindow = "Window/DOTS/Systems";
            public const string EntityHierarchyWindow = "Window/DOTS/Entities";
        }

        public static class ListView
        {
            public const int ItemHeight = 16;
        }

        public static class Settings
        {
            public const string Inspector = "Entity Inspector Settings";
            public const string Advanced = "Advanced Settings";
        }

        public static class SystemSchedule
        {
            public const string k_ComponentToken = "c:";
            public const int k_ComponentTokenLength = 2;
            public const string k_SystemDependencyToken = "sd:";
            public const int k_SystemDependencyTokenLength = 3;
            public const string k_ScriptType = " t:Script";
            public const int k_ShowMinimumQueryCount = 2;
            public const string k_Dash = "-";
        }

        public static class EntityHierarchy
        {
            public const string FullViewName = "FullView";
            public const string SearchViewName = "SearchView";
        }

        public static class State
        {
            public const string ViewDataKeyPrefix = "dots-editor__";
        }
    }
}
