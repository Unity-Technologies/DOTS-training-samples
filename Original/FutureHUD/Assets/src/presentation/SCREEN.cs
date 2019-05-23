using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COLOUR;

public class SCREEN
{
    public Palette P;
    public string title;
    public float duration, timeRemaining;
    public float mx, my, mx_eased, my_eased;
    public virtual void BG() { }
    public void SetMouse(float _mx, float _my, float _mx_eased, float _my_eased)
    {
        mx = _mx;
        my = _my;
        mx_eased = _mx_eased;
        my_eased = _my_eased;
    }
    public virtual void Draw()
    {
        BG();
    }
    public void TXT(string _str, float _x, float _y, Color _col, float _cellSize = PRESENTATION.DEFAULT_TXT_CELL_HEIGHT)
    {
        GL_TXT.Txt(_str.ToLower(), _x, _y, _cellSize, _col);
    }
}

