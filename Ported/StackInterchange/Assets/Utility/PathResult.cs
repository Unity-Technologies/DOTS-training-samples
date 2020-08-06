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
        if (m_Path == null || m_Path.Length == 0)
            return;

        for (int i = 0; i < m_Path.Length - 1; i++)
        {
            Debug.DrawLine(m_Path[i].transform.position, m_Path[i + 1].transform.position, Color.red);
        }

        for (int i = 0; i < m_Path.Length; i++)
        {
            Debug.DrawLine(m_Path[i].transform.position, m_Path[i].transform.position + Vector3.up, Color.green);
        }

        var lastWayPoint = m_Path[m_Path.Length - 1].transform.position;
        for (int i = -1; i <= 1; i+=2) for (int j = -1; j <= 1; j += 2) for (int k = -1; k <= 1; k += 2)
            Debug.DrawLine(lastWayPoint, lastWayPoint + new Vector3(i, j, k), Color.yellow);
    }
}
