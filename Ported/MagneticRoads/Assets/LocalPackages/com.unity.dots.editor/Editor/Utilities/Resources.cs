using UnityEditor;

namespace Unity.Entities.Editor
{
    static class Resources
    {
        public const string Uxml = Constants.EditorDefaultResourcesPath + "uxml/";
        public const string Uss = Constants.EditorDefaultResourcesPath + "uss/";
        public const string Icons = Constants.EditorDefaultResourcesPath + "icons/";

        const string k_ProSuffix = "_dark";
        const string k_PersonalSuffix = "_light";

        public static string SkinSuffix => EditorGUIUtility.isProSkin ? k_ProSuffix : k_PersonalSuffix;

        public static string UxmlFromName(string name)
        {
            return Uxml + name + ".uxml";
        }

        public static string UssFromName(string name)
        {
            return Uss + name + ".uss";
        }

        public static class Templates
        {
            public static readonly UITemplate CommonResources = new UITemplate("Common/common-resources");

            public static readonly UITemplate SystemSchedule = new UITemplate("SystemSchedule/system-schedule");
            public static readonly UITemplate SystemScheduleItem = new UITemplate("SystemSchedule/system-schedule-item");
            public static readonly UITemplate SystemScheduleDetailHeader = new UITemplate("SystemSchedule/system-schedule-detail-header");
            public static readonly UITemplate SystemScheduleDetailContent = new UITemplate("SystemSchedule/system-schedule-detail-content");
            public static readonly UITemplate SystemScheduleDetailQuery = new UITemplate("SystemSchedule/system-schedule-detail-query");
            public static readonly UITemplate DotsEditorCommon = new UITemplate("Common/dots-editor-common");
            public static readonly UITemplate CenteredMessageElement = new UITemplate("Common/centered-message-element");
            public static readonly UITemplate Settings = new UITemplate("Settings/settings");
            public static readonly UITemplate CustomQueryResults = new UITemplate("Common/custom-query-results");
            public static readonly UITemplate EntityHierarchyToolbar = new UITemplate("EntityHierarchy/entity-hierarchy-toolbar");
            public static readonly UITemplate EntityHierarchyEnableLiveLinkMessage = new UITemplate("EntityHierarchy/entity-hierarchy-enable-live-link-message");
            public static readonly UITemplate EntityHierarchyItem = new UITemplate("EntityHierarchy/entity-hierarchy-item");

            public static class Inspector
            {
                public static readonly UITemplate EntityHeader = new UITemplate("Inspector/entity-header");
                public static readonly UITemplate ComponentsRoot = new UITemplate("Inspector/components-root");
                public static readonly UITemplate InspectorStyle = new UITemplate("Inspector/inspector");
                public static readonly UITemplate ComponentHeader = new UITemplate("Inspector/component-header");
                public static readonly UITemplate TagComponentElement = new UITemplate("Inspector/tag-component-element");
                public static readonly UITemplate EntityField = new UITemplate("Inspector/entity-field");
            }
        }
    }
}
