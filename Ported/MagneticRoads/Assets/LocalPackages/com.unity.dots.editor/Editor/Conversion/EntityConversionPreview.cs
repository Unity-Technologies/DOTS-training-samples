using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Editor.Bridge;
using Unity.Editor.Legacy;
using Unity.Mathematics;
using Unity.Serialization.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Entities.Editor
{
    [CustomPreview(typeof(GameObject))]
    class EntityConversionPreview : ObjectPreview
    {
        const string k_SharedStateKey = nameof(EntityConversionPreview) + "." + nameof(SharedState);
        static readonly ComponentTypeNameComparer s_ComponentTypeNameComparer = new ComponentTypeNameComparer();

        // internal for tests
        internal static class Worlds
        {
            const WorldFlags MustMatchMask = WorldFlags.Live | WorldFlags.Conversion;
            const WorldFlags MustNotMatchMask = WorldFlags.Streaming | WorldFlags.Shadow;

            static readonly WorldsChangeDetector WorldsChangeDetector = new WorldsChangeDetector();
            static readonly List<World> s_FilteredWorlds = new List<World>();
            static string[] s_WorldNames = Array.Empty<string>();

            static void Update()
            {
                if (!WorldsChangeDetector.WorldsChanged())
                    return;

                s_FilteredWorlds.Clear();

                foreach (var world in World.All)
                {
                    if ((world.Flags & MustMatchMask) != 0 && (world.Flags & MustNotMatchMask) == 0)
                        s_FilteredWorlds.Add(world);
                }

                s_WorldNames = s_FilteredWorlds.Select(w => w.Name).ToArray();
            }

            public static List<World> FilteredWorlds
            {
                get
                {
                    Update();
                    return s_FilteredWorlds;
                }
            }

            public static string[] FilteredWorldNames
            {
                get
                {
                    Update();
                    return s_WorldNames;
                }
            }
        }

        static class Styles
        {
            public static readonly GUIStyle EntityConversionWarningMessage = EditorStyleUSSBridge.FromUSS(".EntityConversionWarningMessage");
            public static readonly GUIStyle EntityConversionCommonComponentMessage = EditorStyleUSSBridge.FromUSS(".EntityConversionCommonComponentMessage");
            public static readonly GUIStyle EntityConversionComponentTag = EditorStyleUSSBridge.FromUSS(".EntityConversionComponentTag");
            public static readonly GUIStyle AdditionalEntityTag = EditorStyleUSSBridge.FromUSS(".AdditionalEntityTag");
            public static readonly GUIStyle SelectedAdditionalEntityTag = EditorStyleUSSBridge.FromUSS(".SelectedAdditionalEntityTag");
            public static readonly GUIStyle AdditionalEntityIconTag = EditorStyleUSSBridge.FromUSS(".AdditionalEntityIconTag");
            public static readonly GUIStyle EntityConversionComponentArea = EditorStyleUSSBridge.FromUSS(".EntityConversionComponentArea");
            public static readonly GUIStyle AdditionalEntityToggle = EditorStyleUSSBridge.FromUSS(".AdditionalEntityToggle");
        }

        /// <summary>
        /// Helper container to store session state data per <see cref="GameObject"/>.
        /// </summary>
        class State
        {
            /// <summary>
            /// This field controls the toggle/foldout for additional entities. This is independent of the additional entity index.
            /// </summary>
            public bool ShowAdditionalEntities;

            /// <summary>
            /// The selected index for additional entities. This value is preserved even when toggling <see cref="ShowAdditionalEntities"/>
            /// </summary>
            public int AdditionalEntityIndex;

            /// <summary>
            /// The set of currently selected component type indices. These are the components that have fields drawn in the preview window.
            /// </summary>
            public readonly List<int> SelectedComponentTypes = new List<int>();
        }

        /// <summary>
        /// Helper container to store session state data for the all instances of <see cref="EntityConversionPreview"/>.
        /// </summary>
        class SharedState
        {
            /// <summary>
            /// The currently selected <see cref="World"/> in the drop-down.
            /// </summary>
            public int SelectedWorldIndex;
        }

        /// <summary>
        /// State data per <see cref="GameObject"/>. This data is persisted between domain reloads.
        /// </summary>
        State m_State;

        /// <summary>
        /// State data for all instances of <see cref="EntityConversionPreview"/>. This data is persisted between domain reloads.
        /// </summary>
        SharedState m_SharedState;

        /// <summary>
        /// Helper structure to drawn runtime component data.
        /// </summary>
        RuntimeComponentsDrawer m_RuntimeComponentsDrawer;

        /// <summary>
        /// Helper class to detect changes to entities that derive from a given set of gameObjects.
        /// </summary>
        GameObjectConversionChangeTracker m_ChangeTracker;

        /// <summary>
        /// This is used to keep the current targets of the preview. This should be used instead of `ObjectPreview.target`
        /// and `ObjectPreview.m_Targets` because we need to override the default behaviour of multi-selection. This is
        /// achieved by "forcing" single selection on the preview <see cref="Initialize"/>.
        /// </summary>
        readonly List<GameObject> m_GameObjectTargets = new List<GameObject>();

        Rect m_PreviewRect;
        Vector2 m_ScrollPosition = Vector2.zero;
        Vector2 m_ScrollHeaderPosition = Vector2.zero;
        int m_LastSelectedComponentIdx;

        /// <summary>
        /// Gets the currently selected <see cref="World"/> in the preview window for the current session.
        /// </summary>
        /// <returns>The currently selected <see cref="World"/>.</returns>
        public static World GetCurrentlySelectedWorld()
        {
            var state = SessionState<SharedState>.GetOrCreate(k_SharedStateKey);

            if (Worlds.FilteredWorlds.Count == 0)
            {
                state.SelectedWorldIndex = -1;
                return null;
            }

            state.SelectedWorldIndex = state.SelectedWorldIndex >= 0 && state.SelectedWorldIndex < Worlds.FilteredWorlds.Count ? state.SelectedWorldIndex : 0;
            return Worlds.FilteredWorlds[state.SelectedWorldIndex];
        }

        /// <summary>
        /// Called when the preview gets created.
        /// </summary>
        /// <param name="targets">The selected <see cref="UnityEngine.Object"/> to preview.</param>
        public override void Initialize(Object[] targets)
        {
            m_GameObjectTargets.Clear();
            m_GameObjectTargets.AddRange(targets.OfType<GameObject>());

            var mainTarget = m_GameObjectTargets.FirstOrDefault();
            var instanceId = mainTarget.GetInstanceID();

            m_State = SessionState<State>.GetOrCreate($"{nameof(EntityConversionPreview)}.{nameof(State)}.{instanceId}");
            m_SharedState = SessionState<SharedState>.GetOrCreate(k_SharedStateKey);
            m_RuntimeComponentsDrawer = new RuntimeComponentsDrawer();
            m_RuntimeComponentsDrawer.OnDeselectComponent += typeIndex => m_State.SelectedComponentTypes.Remove(typeIndex);
            m_ChangeTracker = new GameObjectConversionChangeTracker();
            m_Targets = new Object[] { mainTarget };
            m_LastSelectedComponentIdx = -1;

            EditorApplication.update += Update;
        }

        /// <summary>
        /// Called to determine if the targeted <see cref="UnityEngine.Object"/> can be previewed in its current state.
        /// </summary>
        public override bool HasPreviewGUI()
        {
            return m_GameObjectTargets.Any(GameObjectConversionEditorUtility.IsConverted);
        }

        /// <summary>
        /// Called to get the content label of the preview header.
        /// </summary>
        public override GUIContent GetPreviewTitle()
        {
            return EditorGUIUtility.TrTextContent("Entity Conversion");
        }

        /// <summary>
        /// Called to implement custom controls in the preview header.
        /// </summary>
        public override void OnPreviewSettings()
        {
            m_SharedState.SelectedWorldIndex = EditorGUILayout.Popup(GUIContent.none, m_SharedState.SelectedWorldIndex, Worlds.FilteredWorldNames);
        }

        /// <summary>
        /// Called to implement custom controls in the preview window.
        /// </summary>
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                m_PreviewRect = r;
            }

            using (var conversionDataPooledList = EntityConversionUtility.GetConversionData(m_GameObjectTargets, GetCurrentlySelectedWorld()).ToPooledList())
            using (new GUILayout.AreaScope(m_PreviewRect, string.Empty, Styles.EntityConversionComponentArea))
            using (new EditorGUILayout.VerticalScope())
            {
                const string liveConversion = "Unity.Entities.Streaming.SubScene.LiveLinkEnabledInEditMode";

                if (!EditorPrefs.GetBool(liveConversion, false))
                {
                    GUILayout.Label(EditorGUIUtility.TrTempContent("Live Conversion is disabled. Enable it in the DOTS file menu to see conversion preview."), Styles.EntityConversionWarningMessage);
                    return;
                }

                var conversionData = conversionDataPooledList.List;

                if (m_GameObjectTargets.Count != conversionData.Count)
                {
                    GUILayout.Label(EditorGUIUtility.TrTempContent("Entity conversion can only be previewed for GameObjects converted by a SubScene."), Styles.EntityConversionWarningMessage);
                    return;
                }

                if (!ValidateGameObjectConversionData(conversionData, out var errorMessage))
                {
                    GUILayout.Label(EditorGUIUtility.TrTempContent(errorMessage), Styles.EntityConversionWarningMessage);
                    return;
                }

                var primary = conversionData[0];
                var isSingleSelection = conversionData.Count == 1;
                var additionalEntitiesCount = primary.AdditionalEntities.Count - 1;
                var hasAdditionalEntities = isSingleSelection && additionalEntitiesCount > 0;

                var entityName = isSingleSelection
                    ? $"{primary.EntityManager.GetName(primary.PrimaryEntity)} (Entity)"
                    : $"{EditorGUIBridge.mixedValueContent.text}({conversionData.Count} entities)";

                if (m_State.AdditionalEntityIndex > additionalEntitiesCount)
                    m_State.AdditionalEntityIndex = additionalEntitiesCount;

                if (hasAdditionalEntities)
                {
                    m_State.ShowAdditionalEntities = EditorGUILayout.Foldout(m_State.ShowAdditionalEntities,
                        EditorGUIUtility.TrTextContentWithIcon($"{entityName} + {additionalEntitiesCount} {(additionalEntitiesCount > 1 ? "Entities" : "Entity")}", EditorIcons.Entity),
                        new GUIStyle(EditorStyles.foldout)
                        {
                            fixedWidth = 650.0f,
                            fontStyle = FontStyle.Bold
                        });

                    GUILayout.Space(4);

                    if (m_State.ShowAdditionalEntities)
                    {
                        using (var scroll = new EditorGUILayout.ScrollViewScope(m_ScrollHeaderPosition, GUILayout.ExpandWidth(true), GUILayout.Height(56)))
                        {
                            m_ScrollHeaderPosition = scroll.scrollPosition;

                            var additionalEntityLabel = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight + 3f);
                            additionalEntityLabel.height -= 3.0f;
                            additionalEntityLabel.x = 4.0f;

                            for (var i = 1; i < primary.AdditionalEntities.Count; ++i)
                            {
                                var entity = primary.AdditionalEntities[i];

                                if (ShowAdditionalEntityToggle(ref additionalEntityLabel, i == m_State.AdditionalEntityIndex, primary.EntityManager, entity))
                                {
                                    m_State.AdditionalEntityIndex = i;
                                }
                                else if (i == m_State.AdditionalEntityIndex)
                                {
                                    m_State.AdditionalEntityIndex = -1;
                                }
                            }
                        }

                        GUILayout.Space(8);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(EditorGUIUtility.TrTextContentWithIcon(entityName, EditorIcons.Entity), EditorStyles.boldLabel);
                    GUILayout.Space(4);
                }

                using (var scroll = new EditorGUILayout.ScrollViewScope(m_ScrollPosition))
                using (var inspectorTargets = GetInspectorTargets(conversionData, m_State).ToPooledList())
                using (var commonComponentTypes = GetCommonComponentTypes(inspectorTargets.List).ToPooledList())
                {
                    m_ScrollPosition = scroll.scrollPosition;

                    var labelRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight + 3f);
                    labelRect.x = 4f;
                    labelRect.height = EditorGUIUtility.singleLineHeight;

                    for (var i = 0; i < commonComponentTypes.List.Count; i++)
                    {
                        var componentType = commonComponentTypes.List[i];
                        var isSelected = m_State.SelectedComponentTypes.Contains(componentType.TypeIndex);
                        var isSelectedInUi = ShowComponentTypeToggle(ref labelRect, isSelected, componentType);

                        HandleComponentSelection(commonComponentTypes, componentType, i, isSelected, isSelectedInUi);
                    }

                    GUILayout.Space(6);

                    m_RuntimeComponentsDrawer.SetTargets(inspectorTargets.List);
                    m_RuntimeComponentsDrawer.SetComponentTypes(m_State.SelectedComponentTypes);

                    if (m_State.SelectedComponentTypes.Count > 0 && commonComponentTypes.List.Count(type => m_State.SelectedComponentTypes.Contains(type.TypeIndex)) > 0)
                    {
                        using (new WideModeScope(338))
                        {
                            m_RuntimeComponentsDrawer.OnGUI();
                        }
                    }

                    GUILayout.Space(4);

                    if (!isSingleSelection)
                    {
                        GUILayout.Label(EditorGUIUtility.TrTempContent("Components that are only on some of the converted entities are not shown."), Styles.EntityConversionCommonComponentMessage);
                    }
                }
            }
        }

        void HandleComponentSelection(PooledList<ComponentType> componentTypes, ComponentType componentType, int indexInOrderedComponentList, bool isSelected, bool isSelectedInUi)
        {
            var isMultiSelectionEnabled = EditorGUI.actionKey;
            var isWideSelectionEnabled = Event.current.shift;

            if (isSelected && !isSelectedInUi)
            {
                if (isWideSelectionEnabled && m_LastSelectedComponentIdx != -1)
                {
                    m_State.SelectedComponentTypes.Clear();
                    SelectRange(componentTypes, m_LastSelectedComponentIdx, indexInOrderedComponentList);
                }
                else if (m_State.SelectedComponentTypes.Count > 1 && !isMultiSelectionEnabled)
                {
                    m_State.SelectedComponentTypes.Clear();
                    m_State.SelectedComponentTypes.Add(componentType.TypeIndex);
                    m_LastSelectedComponentIdx = indexInOrderedComponentList;
                }
                else
                {
                    m_State.SelectedComponentTypes.Remove(componentType.TypeIndex);
                }
            }
            else if (!isSelected && isSelectedInUi)
            {
                if (!isMultiSelectionEnabled)
                    m_State.SelectedComponentTypes.Clear();

                if (isWideSelectionEnabled && m_LastSelectedComponentIdx != -1 && m_LastSelectedComponentIdx != indexInOrderedComponentList)
                {
                    SelectRange(componentTypes, m_LastSelectedComponentIdx, indexInOrderedComponentList);
                }
                else
                {
                    m_State.SelectedComponentTypes.Add(componentType.TypeIndex);
                    m_LastSelectedComponentIdx = indexInOrderedComponentList;
                }
            }
        }

        void SelectRange(PooledList<ComponentType> commonComponentTypes, int boundA, int boundB)
        {
            for (var i = math.min(boundA, boundB); i <= math.max(boundA, boundB); i++)
            {
                m_State.SelectedComponentTypes.Add(commonComponentTypes.List[i].TypeIndex);
            }
        }

        /// <summary>
        /// Called by <see cref="EditorApplication.update"/> ~100 per second.
        /// </summary>
        void Update()
        {
            if (m_ChangeTracker.DidChange(GetCurrentlySelectedWorld(), m_GameObjectTargets, m_State.SelectedComponentTypes))
            {
                InspectorWindowBridge.RepaintAllInspectors();
            }
        }

        static bool ValidateGameObjectConversionData(IReadOnlyList<EntityConversionData> conversionData, out string message)
        {
            if (conversionData.Count == 0)
            {
                message = "This game object will be converted at runtime.";
                return false;
            }

            for (var i = 0; i < conversionData.Count; i++)
            {
                var entityManager = conversionData[i].EntityManager;
                var primaryEntity = conversionData[i].PrimaryEntity;

                if (!entityManager.Exists(primaryEntity))
                {
                    message = conversionData.Count == 0
                        ? "The primary entity has been destroyed during conversion."
                        : "One or more primary entities have been destroyed during conversion.";

                    return false;
                }
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Given a set of <see cref="EntityConversionData"/> and the current preview state, this method will return a set of <see cref="EntityInspectorTarget"/> which should be inspected.
        /// </summary>
        /// <param name="conversionData">The set of currently selected objects.</param>
        /// <param name="state">The current <see cref="EntityConversionPreview"/> state.</param>
        /// <returns>The set of targets which should be inspected.</returns>
        static IEnumerable<EntityContainer> GetInspectorTargets(IReadOnlyList<EntityConversionData> conversionData, State state)
        {
            var root = conversionData[0];

            // @TODO (UX) Figure out how we want to show additional entities during multi-selection.
            if (conversionData.Count == 1 && state.ShowAdditionalEntities && state.AdditionalEntityIndex != -1)
            {
                yield return new EntityContainer(root.EntityManager, root.AdditionalEntities[state.AdditionalEntityIndex]);
                yield break;
            }

            foreach (var data in conversionData)
            {
                yield return new EntityContainer(data.EntityManager, data.PrimaryEntity);
            }
        }

        /// <summary>
        /// Given a set of <see cref="EntityContainer"/> this method will return the set of <see cref="ComponentType"/> that are common to all of them.
        /// </summary>
        IEnumerable<ComponentType> GetCommonComponentTypes(IReadOnlyList<EntityContainer> targets)
        {
            if (targets.Count == 0)
                yield break;

            if (targets.Count == 1)
            {
                // Fast path for single target.
                using (var componentTypes = targets[0].EntityManager.GetComponentTypes(targets[0].Entity))
                {
                    componentTypes.Sort(s_ComponentTypeNameComparer);

                    foreach (var componentType in componentTypes)
                    {
                        yield return componentType;
                    }
                }
            }
            else
            {
                // Slow path for multi target using the intersection.
                using (var intersection = Pooling.GetList<ComponentType>())
                using (var hash = Pooling.GetHashSet<ComponentType>())
                {
                    for (var i = 0; i < targets.Count; i++)
                    {
                        using (var componentTypes = targets[i].EntityManager.GetComponentTypes(targets[i].Entity))
                        {
                            if (i == 0)
                            {
                                foreach (var type in componentTypes)
                                {
                                    intersection.List.Add(type);
                                    hash.Set.Add(type);
                                }
                            }
                            else
                            {
                                foreach (var type in hash.Set)
                                {
                                    if (!componentTypes.Contains(type))
                                    {
                                        intersection.List.Remove(type);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var type in intersection.List.OrderBy(e => e, s_ComponentTypeNameComparer))
                        yield return type;
                }
            }
        }

        static bool ShowAdditionalEntityToggle(ref Rect labelRect, bool isSelected, EntityManager entityManager, Entity entity)
        {
            const int kIconWidth = 20;
            var name = entityManager.GetName(entity);
            var labelWidth = Styles.AdditionalEntityTag.CalcSize((new GUIContent(name))).x;
            var totalWidth = kIconWidth + labelWidth;
            var maxWidth = EditorGUIUtility.currentViewWidth - 8.0f;
            if (labelRect.x + totalWidth > maxWidth)
            {
                labelRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight + 3f);
                labelRect.height = EditorGUIUtility.singleLineHeight;
                labelRect.x = 4.0f;
            }

            // Draw toggle here to cover icon and label area, so that icon is also clickable.
            labelRect.width = totalWidth;
            isSelected = GUI.Toggle(labelRect, isSelected, "", Styles.AdditionalEntityToggle);

            // Draw icon
            labelRect.width = kIconWidth;
            GUI.Label(labelRect, EditorIcons.Entity, Styles.AdditionalEntityIconTag);
            labelRect.x += labelRect.width;
            labelRect.width = labelWidth;

            // Draw label
            GUI.Label(labelRect, name, isSelected ? Styles.SelectedAdditionalEntityTag : Styles.AdditionalEntityTag);

            labelRect.x += labelRect.width + 6f;
            return isSelected;
        }

        static bool ShowComponentTypeToggle(ref Rect labelRect, bool state, ComponentType type)
        {
            var maxWidth = EditorGUIUtility.currentViewWidth - 8.0f;
            var width = Styles.EntityConversionComponentTag.CalcSize((new GUIContent(type.ToString()))).x;

            if (labelRect.x + width > maxWidth)
            {
                labelRect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight + 3f);
                labelRect.height = EditorGUIUtility.singleLineHeight;
                labelRect.x = 4f;
            }

            labelRect.width = width;
            state = GUI.Toggle(labelRect, state, type.ToString(), Styles.EntityConversionComponentTag);
            labelRect.x += labelRect.width + 4f;
            return state;
        }

        struct WideModeScope : IDisposable
        {
            readonly bool m_Previous;

            public WideModeScope(int viewWidth)
            {
                m_Previous = EditorGUIUtility.wideMode;
                EditorGUIUtility.wideMode = EditorGUIUtility.currentViewWidth > viewWidth;
            }

            public void Dispose()
            {
                EditorGUIUtility.wideMode = m_Previous;
            }
        }

        class GameObjectConversionChangeTracker
        {
            World m_LastWorld;
            EntityConversionData m_LastConversionData;
            ArchetypeChunk m_LastChunk;
            uint m_LastGlobalSystemVersion;

            public bool DidChange(World world, IEnumerable<GameObject> targets, IEnumerable<int> selectedComponentTypes)
            {
                if (null == world)
                    return false;

                var result = DidChangeInternal(world, targets, selectedComponentTypes);
                m_LastGlobalSystemVersion = world.EntityManager.GlobalSystemVersion;
                return result;
            }

            unsafe bool DidChangeInternal(World world, IEnumerable<GameObject> targets, IEnumerable<int> selectedComponentTypes)
            {
                if (world != m_LastWorld)
                {
                    m_LastWorld = world;
                    return true;
                }

                if (m_LastConversionData == EntityConversionData.Null)
                {
                    // @TODO Handle multiple targets correctly.
                    m_LastConversionData = EntityConversionUtility.GetConversionData(targets, world).FirstOrDefault();

                    if (m_LastConversionData == EntityConversionData.Null)
                        return false;

                    m_LastChunk = world.EntityManager.GetChunk(m_LastConversionData.PrimaryEntity);
                    return true;

                }

                var chunk = world.EntityManager.GetChunk(m_LastConversionData.PrimaryEntity);

                if (null == chunk.m_Chunk || chunk != m_LastChunk)
                {
                    m_LastChunk = chunk;
                    return true;
                }

                foreach (var typeIndex in selectedComponentTypes)
                {
                    var typeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(m_LastChunk.m_Chunk->Archetype, typeIndex);
                    if (typeIndexInArchetype == -1) continue;
                    var typeChangeVersion = m_LastChunk.m_Chunk->GetChangeVersion(typeIndexInArchetype);

                    if (ChangeVersionUtility.DidChange(typeChangeVersion, m_LastGlobalSystemVersion))
                    {
                        m_LastGlobalSystemVersion = world.EntityManager.GlobalSystemVersion;

                        return true;
                    }
                }

                return false;
            }
        }

        class ComponentTypeNameComparer : IComparer<ComponentType>
        {
            readonly Dictionary<int, string> m_ComponentNameByTypeIndex = new Dictionary<int, string>();

            public int Compare(ComponentType x, ComponentType y)
            {
                var xName = GetTypeName(x);
                var yName = GetTypeName(y);

                return string.Compare(xName, yName, StringComparison.OrdinalIgnoreCase);
            }

            string GetTypeName(ComponentType componentType)
            {
                if (!m_ComponentNameByTypeIndex.TryGetValue(componentType.TypeIndex, out var typeName))
                {
                    typeName = componentType.GetManagedType().Name;
                    m_ComponentNameByTypeIndex.Add(componentType.TypeIndex, typeName);
                }

                return typeName;
            }
        }
    }
}
