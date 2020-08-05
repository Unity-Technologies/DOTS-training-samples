using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathResult : MonoBehaviour
{
    public Transform[] m_Path;

    void Start()
    {
        
    }

    void Update()
    {
        if (m_Path == null)
            return;

        for (int i = 0; i < m_Path.Length - 1; i++)
        {
            Debug.DrawLine(m_Path[i].transform.position, m_Path[i + 1].transform.position, Color.red);
        }

        for (int i = 0; i < m_Path.Length; i++)
        {
            Debug.DrawLine(m_Path[i].transform.position, m_Path[i].transform.position + Vector3.up, Color.green);
        }
    }
}
