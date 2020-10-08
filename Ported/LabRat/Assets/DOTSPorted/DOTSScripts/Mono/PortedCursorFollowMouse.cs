using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PortedCursorFollowMouse : MonoBehaviour {
    public Canvas canvas;

    void Update() {
        var screenPos = Input.mousePosition;
        SetScreenPosition(screenPos);
    }
    
    public void SetScreenPosition(Vector2 screenPoint) {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);
    }
}

