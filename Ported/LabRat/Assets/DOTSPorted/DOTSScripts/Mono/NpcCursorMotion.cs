using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcCursorMotion : MonoBehaviour {
    public Canvas canvas;
    public int playerNumber = 1;
    Vector2 m_ScreenPos = new Vector2(0.0f, 0.0f);

    void Update()
    {
        var npcScreenPos = GameObject.Find("CursorManager").GetComponent<NpcCursorManager>().NpcScreenPos;
        m_ScreenPos = npcScreenPos[playerNumber];
        SetScreenPosition(m_ScreenPos);
    }
    
    private void SetScreenPosition(Vector2 screenPoint) {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);
    }
}