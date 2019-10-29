using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

#pragma warning disable 414

namespace Unity.Rendering
{
    [UnityEngine.ExecuteAlways]
    [RequireComponent(typeof(LODGroup))]
    public class HLOD : MonoBehaviour
    {
        [Tooltip("Store lowest LOD in scene section 0, and the rest in section 1.")]
        public bool autoLODSections = true;

        [SerializeField] Transform[] m_LODParentTransforms;

        [NonSerialized] int m_Index = -1;

        static List<HLOD> ms_HLODS = null;

        float4 lodDistances;
        Vector3 m_CachedWorldReferencePoint;

        LODGroup[][] m_LodGroups;

        int m_LastActiveLodLevel;
        static bool m_DidSelectionChange;


        public Transform[] LODParentTransforms
        {
            get { return m_LODParentTransforms; }
            set { m_LODParentTransforms = value; }
        }

        public LODGroup[] CalculateLODGroups(int index)
        {
            if (m_LODParentTransforms[index] == null)
                return new LODGroup[0];
            else
                return m_LODParentTransforms[index].GetComponentsInChildren<LODGroup>();
        }

        public static void RemoveAtSwapBack<T>(List<T> list, int index)
        {
            int lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
        }

        void OnEnable()
        {
            m_LastActiveLodLevel = -1;
            m_Index = -1;

            var lodGroup = GetComponent<LODGroup>();

            var lods = GetComponent<LODGroup>().GetLODs();

            //* Validate
            if (m_LODParentTransforms == null || m_LODParentTransforms.Length != lods.Length)
            {
                Debug.LogWarning("HLOD: LODParentTransforms must match LODGroup array");
                return;
            }

            for (int i = 0; i != m_LODParentTransforms.Length; i++)
            {
                if (m_LODParentTransforms[i] == null)
                {
                    Debug.LogWarning("HLOD: LOD parent transforms must be empty");
                    return;
                }
            }

            if (lods.Length > 4)
            {
                Debug.LogWarning("Only 4 LOD levels are supported");
                return;
            }

            // Register HLOD System to run before culling
            if (ms_HLODS == null)
            {
                RenderPipelineManager.beginCameraRendering += OnBeforeCull;
                Camera.onPreCull += OnBeforeCull;
                ms_HLODS = new List<HLOD>();
            }

            // Register and cache values...
            m_Index = ms_HLODS.Count;
            ms_HLODS.Add(this);

            m_LodGroups = new LODGroup[m_LODParentTransforms.Length][];
            for (int i = 0; i != m_LodGroups.Length; i++)
                m_LodGroups[i] = CalculateLODGroups(i);

            m_CachedWorldReferencePoint = transform.TransformPoint(lodGroup.localReferencePoint);

            CacheLODDistances(lodGroup, lods);

#if UNITY_EDITOR
            if (!Application.isPlaying)
                Selection.selectionChanged += SelectionChange;
#endif
        }

        void CacheLODDistances(LODGroup lodGroup, LOD[] lods)
        {
            var worldSpaceSize = LODGroupExtensions.GetWorldSpaceSize(lodGroup);
            lodDistances = new float4(float.PositiveInfinity);
            for (int i = 0; i != lods.Length; i++)
                lodDistances[i] = worldSpaceSize / lods[i].screenRelativeTransitionHeight;
        }

        void CacheLODDistances()
        {
            var group = GetComponent<LODGroup>();
            CacheLODDistances(group, group.GetLODs());
        }

        void OnDisable()
        {
            if (m_Index != -1)
            {
                ms_HLODS[ms_HLODS.Count - 1].m_Index = m_Index;
                RemoveAtSwapBack(ms_HLODS, m_Index);

                m_Index = -1;
            }
        }

        // When selection changes editor code might use ForceLOD api as well, resulting in conflicts.
        // Thus just force override in that case.
        static void SelectionChange()
        {
            m_DidSelectionChange = true;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/HLOD/Invalidate Caches %l")]
        public static void InvalidateHLODCache()
        {
            var hlods = Resources.FindObjectsOfTypeAll<HLOD>();
            foreach (var hlod in hlods)
            {
                if (hlod.isActiveAndEnabled)
                {
                    hlod.OnDisable();
                    hlod.OnEnable();
                }
            }

            SceneView.RepaintAll();
        }

#endif

        static void OnBeforeCull(ScriptableRenderContext _, Camera camera) => OnBeforeCull(camera);

        static void OnBeforeCull(Camera camera)
        {
            Profiler.BeginSample("HLOD");
            var lodParams = LODGroupExtensions.CalculateLODParams(camera);

#if UNITY_EDITOR
            // Updated cached lod distances for active object (Too expensive for everything...)
            var activeGameObject = Selection.activeGameObject;
            if (activeGameObject != null && activeGameObject.GetComponent<HLOD>())
                activeGameObject.GetComponent<HLOD>().CacheLODDistances();
#endif

            for (int h = 0; h != ms_HLODS.Count; h++)
            {
                var hlod = ms_HLODS[h];
                int lodIndex = LODGroupExtensions.CalculateCurrentLODIndex(hlod.lodDistances,
                    hlod.m_CachedWorldReferencePoint, ref lodParams);

                if (lodIndex == hlod.m_LastActiveLodLevel && !m_DidSelectionChange)
                    continue;
                hlod.m_LastActiveLodLevel = lodIndex;

                for (int i = 0; i != hlod.m_LODParentTransforms.Length; i++)
                {
                    bool visible = i == lodIndex;
                    var childGroups = hlod.m_LodGroups[i];

                    for (var g = 0; g != childGroups.Length; g++)
                    {
                        var subGroup = childGroups[g];
                        if (visible)
                            subGroup.ForceLOD(-1);
                        else
                            subGroup.ForceLOD(int.MaxValue);
                    }
                }
            }

            m_DidSelectionChange = false;
            Profiler.EndSample();
        }
    }
}
