using UnityEngine;
using Unity.Entities;

public class PheremoneVisualization : MonoBehaviour
{
    Texture2D m_VisTexture;
    Material m_Mat;

    public void SetMap(PheromoneMap map, DynamicBuffer<PheromoneStrength> pheromones) {
        if (m_VisTexture == null || m_VisTexture.width != map.Resolution) {
            if (m_VisTexture != null) Texture2D.Destroy(m_VisTexture);
            m_VisTexture = new Texture2D(map.Resolution, map.Resolution, TextureFormat.RFloat, mipChain: false, linear: true);
        }
        transform.localScale = new Vector3(map.WorldSpaceSize, map.WorldSpaceSize, map.WorldSpaceSize);
        transform.localPosition = new Vector3(map.WorldSpaceSize / 2f, 0, map.WorldSpaceSize / 2f);

        m_VisTexture.SetPixelData(pheromones.AsNativeArray(), mipLevel: 0);
        m_VisTexture.Apply();

        m_Mat = GetComponent<MeshRenderer>().material;
        m_Mat.mainTexture = m_VisTexture;
    }

    private void OnDisable() {
        if (m_VisTexture == null) return;
        Texture2D.Destroy(m_VisTexture);
    }
}
