using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

// https://preshing.com/20120515/memory-reordering-caught-in-the-act/
public class VolatileTest : MonoBehaviour
{
    int m_X;
    int m_Y;
    int m_X2;
    int m_Y2;
    
    Thread m_ThreadA;
    Thread m_ThreadB;
    long m_Iterations;
    long m_X0Y0, m_X1Y0, m_X0Y1, m_X1Y1;
    long m_AIterations = -1;
    long m_BIterations = -1;
    
    public bool addMemoryBarriers;
    
    bool m_reset;

    void Awake()
    {
        m_ThreadA = new Thread(WorkA);
        m_ThreadB = new Thread(WorkB);
        m_ThreadA.Start();
        m_ThreadB.Start();
    }

    void OnGUI()
    {
        var guiWidth = GUILayout.Width(500);
        
        var text = $"\nRESULTS\n\nX0Y0: {m_X0Y0:n0} ({m_X0Y0 / (double)m_Iterations:P1})\nX1Y0: {m_X1Y0:n0} ({m_X1Y0 / (double)m_Iterations:P1})\nX0Y1: {m_X0Y1:n0} ({m_X0Y1 / (double)m_Iterations:P1})\nX1Y1: {m_X1Y1:n0} ({m_X1Y1 / (double)m_Iterations:P1})\n\n{m_Iterations:n0} iterations total!\n\n";

        GUI.color = m_X0Y0 <= 0 ? Color.green : new Color(1f, 0.34f, 0.32f);
        if (GUILayout.Button(text, guiWidth))
        {
            Debug.LogError(text);
        }

        GUI.color = Color.cyan;
        if(GUILayout.Button(($"\nToggle Memory Barriers\nCurrently: {(addMemoryBarriers ? "ON" : "OFF")}\n"), guiWidth))
        {
            addMemoryBarriers ^= true;
            m_reset = true;
        }
    }
    
    void OnDestroy()
    {
        m_ThreadA.Abort();
        m_ThreadB.Abort();
    }

    void Update()
    {
        var start = Time.realtimeSinceStartupAsDouble;
        const float maxDt = 1f/30;
        while(Time.realtimeSinceStartupAsDouble - start < maxDt)
        {
            // Reset X and Y
            m_X = 0;
            m_Y = 0;

            if (m_reset)
            {
                m_reset = false;
                m_X0Y0 = m_X0Y1 = m_X1Y0 = m_X1Y1 = 0;
                m_Iterations = 0;
                m_AIterations = 0;
                m_BIterations = 0;
                
                Thread.MemoryBarrier();
            }
            
            m_Iterations++;
            
            // Wait for both threads
            while (m_BIterations != m_Iterations || m_AIterations != m_Iterations)
            {
            }
            
            // Check if there was a simultaneous reorder
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if(m_X2 == 0 && m_Y2 == 0) m_X0Y0++;
            else if (m_X2 == 1 && m_Y2 == 1) m_X1Y1++;
            else if (m_X2 == 1) m_X1Y0++;
            else m_X0Y1++;
        }
    }

    void WorkA()
    {
        for (;;)
        {
            while(m_AIterations >= m_Iterations) {}
            
            m_X = 1;
            if(addMemoryBarriers) Thread.MemoryBarrier();
            m_X2 = m_Y;
            
            if(addMemoryBarriers) Thread.MemoryBarrier();
            m_AIterations = m_Iterations;
        }
    }

    void WorkB()
    {
        for (;;)
        {
            while(m_BIterations >= m_Iterations) {}
            
            m_Y = 1;
            if(addMemoryBarriers) Thread.MemoryBarrier();
            m_Y2 = m_X;
            
            if(addMemoryBarriers) Thread.MemoryBarrier();
            m_BIterations = m_Iterations;
        }
    }
}
