#if USE_BATCH_RENDERER_GROUP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Unity.Rendering
{
#if UNITY_EDITOR
    [ExecuteAlways]
    public class CullingStatsDrawer : MonoBehaviour
    {
        private bool m_Enabled = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                m_Enabled = !m_Enabled;
            }
        }
        private unsafe void OnGUI()
        {
            if (m_Enabled)
            {
                var sys = World.Active.GetExistingSystem<RenderMeshSystemV2>();

                var stats = sys.ComputeCullingStats();

                GUILayout.BeginArea(new Rect { x = 10, y = 10, width = 1024, height = 200 }, "Culling Stats", GUI.skin.window);

                GUILayout.Label("Culling stats:");
                GUILayout.Label($"  Chunks: Total={stats.Stats[CullingStats.kChunkTotal]} AnyLOD={stats.Stats[CullingStats.kChunkCountAnyLod]} FullIn={stats.Stats[CullingStats.kChunkCountFullyIn]} w/Instance Culling={stats.Stats[CullingStats.kChunkCountInstancesProcessed]}");
                GUILayout.Label($"  Instances tests: {stats.Stats[CullingStats.kInstanceTests]}");
                GUILayout.Label($"  Select LOD: Total={stats.Stats[CullingStats.kLodTotal]} No Requirements={stats.Stats[CullingStats.kLodNoRequirements]} Chunks Tested={stats.Stats[CullingStats.kLodChunksTested]} Changed={stats.Stats[CullingStats.kLodChanged]}");
                GUILayout.Label($"  Root LODs selected: {stats.Stats[CullingStats.kCountRootLodsSelected]} Failed: {stats.Stats[CullingStats.kCountRootLodsFailed]}");
                GUILayout.Label($"  Camera Move Distance: {stats.CameraMoveDistance} meters");

                GUILayout.EndArea();
            }
        }
    }
#endif
}
#endif
