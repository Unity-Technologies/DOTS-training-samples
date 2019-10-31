using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    static readonly int k_NumPlayers = 4;
    public GameObject CursorPrefab;
    public Canvas Canvas;

    GameManagementSystem m_gameManager;
    GameObject[] m_cursors = new GameObject[k_NumPlayers];
    Image[] m_cursorImages = new Image[k_NumPlayers];

    void OnEnable()
    {
        m_gameManager = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameManagementSystem>();
        for (int i=0; i < k_NumPlayers; i++)
        {
            m_cursors[i] = Instantiate<GameObject>(CursorPrefab, Vector3.zero, Quaternion.identity, Canvas.transform);
            m_cursorImages[i] = m_cursors[i].GetComponentInChildren<Image>();
            m_cursorImages[i].color = m_gameManager.GetPlayerColor(i);
        }
    }

    void Update()
    {
        Vector2 pos;
        for (uint i = 0; i < k_NumPlayers; ++i)
        {
            var screenPoint = m_gameManager.GetPlayerCursorScreenPos(i);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Canvas.transform as RectTransform, screenPoint, Canvas.worldCamera, out pos);
            m_cursors[i].transform.position = Canvas.transform.TransformPoint(pos);
        }
    }
}
