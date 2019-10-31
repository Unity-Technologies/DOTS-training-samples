using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Rendering
{
    [CreateAssetMenu(menuName = "ScriptableObjects/MeshRendererScriptableObject")]
    public class MeshRendererScriptableObject : ScriptableObject
    {
        public MeshRenderer drone;
        public MeshRenderer farmer;
        public MeshRenderer ground;
        public MeshRenderer plant;
        public MeshRenderer rock;
        public MeshRenderer store;

        private static MeshRendererScriptableObject m_instance;
        public static MeshRendererScriptableObject instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = Resources.FindObjectsOfTypeAll<MeshRendererScriptableObject>().FirstOrDefault();
                if (m_instance == null)
                    m_instance = Resources.Load<MeshRendererScriptableObject>("Meshes");
                Assert.IsNotNull(m_instance, "MeshRendererScriptableObject is not found");
                return m_instance;
            }
        }
    }
}