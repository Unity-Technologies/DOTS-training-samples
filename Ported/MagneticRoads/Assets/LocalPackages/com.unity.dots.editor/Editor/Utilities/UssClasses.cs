namespace Unity.Entities.Editor
{
    static class UssClasses
    {
        public static class Resources
        {
            public const string SystemSchedule = "system-schedule__resources";
            public const string EntityHierarchy = "entity-hierarchy__resources";
            public const string Inspector = "inspector__resources";
            public const string ComponentIcons = "component__icons";
        }

        public static class DotsEditorCommon
        {
            public const string CommonResources = "common-resources";
            public const string SettingsIcon = "settings-icon";
            public const string SearchIconContainer = "search-icon-container";
            public const string SearchIcon = "search-icon";

            public const string SearchFieldContainer = "search-field-container";
            public const string SearchField = "search-field";
            public const string SearchFieldCancelButton = SearchField + "__cancel-button";

            public const string CustomToolbarToggle = "toolbar-toggle";
            public const string CustomToolbarToggleLabelParent = CustomToolbarToggle + "__label-parent";
            public const string CustomToolbarToggleLabel = CustomToolbarToggle + "__label-with-icon";
            public const string CustomToolbarToggleOnlyLabel = CustomToolbarToggle + "__label-without-icon";
            public const string CustomToolbarToggleIcon = CustomToolbarToggle + "__icon";

            public const string CustomLabelUnderline = "label-with-underline";

            const string CenteredMessageElementBase = "centered-message-element";
            public const string CenteredMessageElementTitle = CenteredMessageElementBase + "__title";
            public const string CenteredMessageElementMessage = CenteredMessageElementBase + "__message";
        }

        public static class SystemScheduleWindow
        {
            public const string SystemSchedule = "system-schedule";
            public const string ToolbarContainer = SystemSchedule + "__toolbar-container";
            public const string ToolbarRightSideContainer = SystemSchedule + "__toolbar-right-side-container";

            public static class TreeView
            {
                public const string Header = SystemSchedule + "__tree-view__header";
                public const string System = SystemSchedule + "__tree-view__system-label";
                public const string Matches = SystemSchedule + "__tree-view__matches-label";
                public const string Time = SystemSchedule + "__tree-view__time-label";
            }

            public static class Items
            {
                const string Base = SystemSchedule + "-item";
                public const string Icon = Base + "__icon";
                public const string Enabled = Base + "__enabled-toggle";
                public const string SystemName = Base + "__name-label";
                public const string Matches = Base + "__matches-label";
                public const string Time = Base + "__time-label";

                public const string SystemIcon = Icon + "--system";
                public const string SystemGroupIcon = Icon + "--system-group";
                public const string CommandBufferIcon = Icon + "--command-buffer";
                public const string UnmanagedSystemIcon = Icon + "--unmanaged-system";

                public const string SystemNameNormal = Base + "__name-label-normal";
                public const string SystemNameBold = Base + "__name-label-bold";
            }

            public static class Detail
            {
                const string Base = SystemSchedule + "-detail";
                const string Header = Base + "__header";
                const string Icon = Base + "__icon";

                public const string Content = Base + "__content";
                public const string SystemIcon = Icon + "--system";
                public const string UnmanagedSystemIcon = Icon + "--unmanaged-system";
                public const string GroupIcon = Icon + "--group";
                public const string CommandBufferIcon = Icon + "--command-buffer";
                public const string ScriptsIcon = Icon + "--scripts";
                public const string QueryIcon = Icon + "--query";
                public const string ReadOnlyIcon = Icon + "--read-only";
                public const string ReadWriteIcon = Icon + "--read-write";
                public const string WriteOnlyIcon = Icon + "--write-only";
                public const string ExcludeIcon = Icon + "--exclude";

                public const string SystemIconName = Header + "-system-icon";
                public const string ScriptsIconName = Header + "-scripts-icon";
                public const string SystemNameLabel = Header + "-system-name-label";
                public const string MiddleSection = Header + "-middle-section";
                public const string ResizeBar = Header + "-resize-bar";

                public const string QueryTitleLabel = Content + "-query-title-label";
                public const string MatchTitleLabel = Content + "-match-title-label";
                public const string QueryRow2 = Content + "-query-row-2";
                public const string QueryIconName = Content + "-query-icon";
                public const string AllComponentContainer = Content + "-all-component-container";
                public const string EachComponentContainer = Content + "-each-component-container";
                public const string ComponentAccessModeIcon = Content + "-component-access-icon";
                public const string EntityMatchCountContainer = Content + "-match-container";
                public const string ShowMoreLessLabel = Content + "-show-more-less-label";

                public const string SchedulingTitle = Content + "-scheduling-title";
                public const string SchedulingToggle = Content + "-scheduling-toggle";
            }
        }

        public static class EntityHierarchyWindow
        {
            const string k_EntityHierarchyBase = "entity-hierarchy";

            public static class Toolbar
            {
                const string k_Base = k_EntityHierarchyBase + "-toolbar";
                public const string Container = k_Base + "__container";
                public const string LeftSide = k_Base + "__left";
                public const string RightSide = k_Base + "__right";
                public const string SearchField = k_Base + "__search-field";
            }

            public static class Item
            {
                const string k_Base = k_EntityHierarchyBase + "-item";

                public const string SceneNode = k_Base + "__scene-node";

                public const string Icon = k_Base + "__icon";
                public const string IconScene = Icon + "--scene";
                public const string IconEntity = Icon + "--entity";

                public const string NameLabel = k_Base + "__name-label";
                public const string NameScene = NameLabel + "--scene";

                public const string SystemButton = k_Base + "__system-button";
                public const string PingGameObjectButton = k_Base + "__ping-gameobject-button";

                public const string VisibleOnHover = k_Base + "__visible-on-hover";
            }
        }

        public static class Inspector
        {
            public static class EntityHeader
            {
                public const string OriginatingGameObject = "originating-game-object";
            }

            public static class Icons
            {
                const string k_Base = "inspector-icon";
                public const string Small = k_Base + "--small";
                public const string Medium = k_Base + "--medium";
                public const string Big = k_Base + "--big";
            }

            public static class Component
            {
                const string k_Base = "component";
                public const string Container = k_Base + "-container";
                public const string Header = k_Base + "-header";
                public const string Name = k_Base + "-name";
                public const string Icon = k_Base + "-icon";
                public const string Category = k_Base + "-category";
                public const string Menu = k_Base + "-menu";
            }

            public static class ComponentTypes
            {
                const string k_PostFix = "-data";
                public const string Component = "component" + k_PostFix;
                public const string Tag = "tag-component" + k_PostFix;
                public const string SharedComponent = "shared-component" + k_PostFix;
                public const string ChunkComponent = "chunk-component" + k_PostFix;
                public const string ManagedComponent = "managed-component" + k_PostFix;
                public const string BufferComponent = "buffer-component" + k_PostFix;
            }
        }

        public static class UIToolkit
        {
            public const string Disabled = "unity-disabled";

            public static class BaseField
            {
                const string k_Base = "unity-base-field";
                public const string Input = k_Base + "__input";
            }

            public static class ObjectField
            {
                public const string ObjectSelector = "unity-object-field__selector";
                public const string Display = "unity-object-field-display";
            }

            public static class Toggle
            {
                const string k_Base = "unity-toggle";
                public const string Text = k_Base + "__text";
                public const string Input = k_Base + "__input";
            }
        }
    }
}
