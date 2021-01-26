using UnityEngine;
using Unity.Entities;

public class PheromoneRendering : MonoBehaviour
{
    [SerializeField] private bool m_RandomizePheromones = false;
    
    Texture2D m_VisTexture = null;
    [SerializeField] private Material m_Mat = null;
    byte[] m_PheromoneArray = null;
    [SerializeField] private int m_SquareResolution = 128;
    //private int old_Resolution = 0;

    public bool AllowPheromoneDynamicBuffer = false;

    private void GenerateRandomData()
    {
        for (int i = 0; i < m_PheromoneArray.Length; i++)
        {
            m_PheromoneArray[i] = (byte)Random.Range(0, 255);
        }
    }

    private void OnDisable()
    {
        if (m_VisTexture == null) return;
        Texture2D.Destroy(m_VisTexture);
    }

    private void Start()
    {
        m_PheromoneArray = new byte[m_SquareResolution * m_SquareResolution];
    }

    private void Update()
    {
        if(m_RandomizePheromones)
        {
            //m_RandomizePheromones = false;
            GenerateRandomData();
            //SetMap();
            SetPheromoneArray(m_PheromoneArray);
        }
    }

    public void SetPheromoneArray(byte[] byteArray)
    {
        CheckTextureInit();

        Debug.Log(byteArray);
        Debug.Log(m_VisTexture);
        m_VisTexture.SetPixelData(byteArray, 0, 0);
        m_VisTexture.Apply();
    }


    public void SetPheromoneArray(DynamicBuffer<PheromoneStrength> pheromoneBuffer)
    {
        if (AllowPheromoneDynamicBuffer)
        {
            CheckTextureInit();

            byte[] newByteArray = new byte[pheromoneBuffer.Length];

            for (int i = 0; i < pheromoneBuffer.Length; i++)
            {
                newByteArray[i] = (byte)pheromoneBuffer[i];
            }

            m_VisTexture.SetPixelData(newByteArray, 0, 0);
            m_VisTexture.Apply();
        }
    }

    public void CheckTextureInit()
    {
        if(m_VisTexture != null && m_VisTexture.width != m_SquareResolution)
        {
            Texture2D.Destroy(m_VisTexture);
            m_VisTexture = null;
        }

        if (m_VisTexture == null)// || m_VisTexture.width != m_SquareResolution)
        {
            //if (m_VisTexture != null) Texture2D.Destroy(m_VisTexture);
            m_VisTexture = new Texture2D(m_SquareResolution, m_SquareResolution, TextureFormat.R8, mipChain: false, linear: true);
        }

        m_Mat.mainTexture =m_VisTexture;
    }
}
