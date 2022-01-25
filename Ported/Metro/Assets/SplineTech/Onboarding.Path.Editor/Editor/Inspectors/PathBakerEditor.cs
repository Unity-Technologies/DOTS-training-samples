using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Onboarding.BezierPath
{
    [CustomEditor(typeof(PathBaker))]
    public class PathBakerEditor : Editor
    {
        // ----------------------------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Bake Single Threaded"))
            {
                Bake( (PathBaker baker, int bezierSegmentCount) => { 
                    BakeSplinesMultithreaded.BakingProcess(baker, bezierSegmentCount, true); 
                });
            }

            if (GUILayout.Button("Bake Threadpooled"))
            {
                Bake((PathBaker baker, int bezierSegmentCount) => {
                    BakeSplinesMultithreaded.BakingProcess(baker, bezierSegmentCount);
                });
            }

            if (GUILayout.Button("Bake With Jobs"))
            {
                Bake((PathBaker baker, int bezierSegmentCount) => {
                    BakeSplinesJobified.BakingProcess(baker, bezierSegmentCount);
                });
            }
        }

        // ----------------------------------------------------------------------------------------
        private void Bake(System.Action<PathBaker,int> bakingAction)
        {
            var bakers = targets.Cast<PathBaker>().ToArray();
            foreach (var baker in bakers)
            {
                int controlPoints = baker.m_ControlPoints.Length;
                int bezierSegmentCount = (controlPoints - 1) / 3;

                if (0 != (controlPoints - 1) % 3)
                {
                    Debug.LogError($"The baker {baker.name} needs 3n+1 points to bake in order to bake 'n' splines. Skipping...");
                    continue;
                }

                if (baker.m_PathData == null)
                {
                    Debug.LogError($"The baker {baker.name} needs a valid Path ");
                    continue;
                }

                // Bake that baker
                Undo.RegisterCompleteObjectUndo(baker.m_PathData, $"Baking Path for {baker.name}");

                bakingAction(baker, bezierSegmentCount);

                // Make sure the object gets save, since we don't go through SerializeObjects API
                // to modify the object
                EditorUtility.SetDirty(baker.m_PathData);
            }
        }
    }
}
