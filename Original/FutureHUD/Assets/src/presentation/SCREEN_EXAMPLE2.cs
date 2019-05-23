using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COLOUR;
using DATA;

public class SCREEN_EXAMPLE2 : SCREEN
{
    const int _HOOP_SIDES = 24;
    const float _HOOP_SIZE = 0.5f;
    float skewStrength = 0.5f;
    DataSprawl[] sprawls;
    int sprawlCount = 20;
    public SCREEN_EXAMPLE2()
    {
        duration = 30f;
        title = "hud example 2";
        P = COL.Get_Palette(0);

        sprawls = new DataSprawl[sprawlCount];
        for (int i = 0; i < sprawlCount; i++)
        {
            sprawls[i] = new DataSprawl(10, 10, 4, 10);
        }
    }

    public override void BG()
    {
        GL_DRAW.Draw_BG_Y(P.Get(0), P.Get(1));
    }

    public override void Draw()
    {
        base.Draw();

        // update data
        for (int i = 0; i < sprawlCount; i++)
        {
            sprawls[i].Update();
        }

        Floor();

        Skew_left();
        //Skew_right();

        int _COUNT = 6;
        float _CELLS_X = 0.1f;
        float _CELLS_Y = 0.5f;
        float _SPAN_X = 0.6f;
        float _CELL_W = _SPAN_X / _COUNT;
        float _CELL_H = 0.05f;
        float _PAD = 0.025f;
        GL_DRAW.Draw_RECT_FILL(_CELLS_X - _PAD, _CELLS_Y - _PAD * 0.5f, _SPAN_X, (_CELL_H * 2) + (_PAD * 2f), P.Get(0, 0.75f));
        for (int i = 0; i < _COUNT; i++)
        {
            float _FACTOR = (float)i / _COUNT;
            WallCell(_CELLS_X + (i * _CELL_W), _CELLS_Y, _CELL_W, _CELL_H, P.Lerp(2, 3, _FACTOR), i, i * .1f, i * 0.1f);
            int _ii = i + _COUNT;
            WallCell(_CELLS_X + (i * _CELL_W), _CELLS_Y + _CELL_H * 1.1f, _CELL_W, _CELL_H, P.Lerp(3, 2, _FACTOR), _ii, _ii * .1f, _ii * 0.1f);
        }

        // floor markers
        Skew_Floor();
        int _markerCount = 4;

        for (int i = 0; i < _markerCount; i++)
        {
            FloorMarker(0.1f + (i * 0.2f), 0.4f + (i * 0.098f), 0.04f, Anim.Sin_Time(_timeOffset: i * 0.1f), i, i * 0.2f);
        }
    }
    void Skew_Floor()
    {
        GL_DRAW.SKEW(skewStrength, -skewStrength);
    }
    void Skew_left()
    {
        GL_DRAW.SKEW(skewStrength, 0f);
    }
    void Skew_right()
    {
        GL_DRAW.SKEW(-skewStrength, 0f);
    }
    void Floor()
    {
        Skew_Floor();
        GL_DRAW.Draw_GRID_BOX_FILL(-0.5f, -0.5f, 2, 2, 50, 50, P.Get(0, 0.05f), Anim.Sin_Time(10f, 0.52f, .55f));
        GL_DRAW.Draw_GRID_BOX(-0.5f, -0.5f, 2, 2, 50, 50, P.Get(3, Anim.Sin_Time(2, 0.1f, 0.3f)), 0.75f);
    }
    void FloorMarker(float _x, float _y, float _size, float _value, int _dataIndex = 0, float _timeOffset = 0)
    {
        Color _COL = P.Get(3);
        float _markerSize = _HOOP_SIZE * _size;
        float _valueInv = 1f - _value;
        float _SIZE2 = _size * 0.5f;
        float _Y_OFFSET = Mathf.Lerp(0.01f * _size, 0.4f * _size, _value);

        // floor hoop
        DrawHoop(_x, _y, _size, 0.05f + (_valueInv * 0.2f), _angleOffset: 0);
        DrawHoop(_x, _y, _SIZE2, 0.075f + (_valueInv * 0.05f), _angleOffset: 0);

        for (int i = 0; i < 3; i++)
        {
            float _ANGLE_OFF = 0.33f * i;
            DrawHoop(_x, _y, _size * 1.5f, 0.3f, _ANGLE_OFF, _ANGLE_OFF + .1666666f, _angleOffset: Anim.Runtime(-0.1f));
        }

        // raised
        DrawHoop(_x, _y + _Y_OFFSET, _size, 0.3f, _angleOffset: 0);

        for (int i = 0; i < 4; i++)
        {
            float _ANGLE_OFF = 0.25f * i;
            DrawHoop(_x, _y + _Y_OFFSET, _size * 0.75f, 0.3f, _ANGLE_OFF, _ANGLE_OFF + .125f, _angleOffset: Anim.Runtime(0.25f, _timeOffset * 0.2f));
        }

        HUD.Draw_LABEL_LINE_X("pt :: " + _dataIndex, _x, _y, _size * 2f, _size * 0.1f, P.Get(3, 0f), _COL, _COL);

        float _statPad = _size * 0.2f;
        float _statPad2 = _statPad * 2;
        float _stat_W = _size * 2;
        float _stat_H = _size * 4;
        float _stat_X = _size + _statPad;
        float _stat_Y = -_stat_H - (_statPad * 2);
        GL.PushMatrix();
        GL_DRAW.TransformMatrix(_x, _y);
        GL_DRAW.Draw_RECT(_stat_X, _stat_Y, _stat_W, _stat_H, _COL);
        float[] _values = VALUES.RandomValues_NOISE_TIME(5, _offsetA: _timeOffset, _offsetB: _timeOffset * 1.4f);
        sprawls[_dataIndex].Draw(_stat_X + _statPad, _stat_Y + (_stat_H * 0.75f), _stat_W - _statPad2, _stat_H * 0.2f, _COL);
        HUD.Draw_HISTOGRAM_BAR_X(_stat_X + _statPad, _stat_Y + _statPad, _stat_W - _statPad2, _stat_H * 0.5f, _COL, _COL, 0.75f, false, _values);
        HUD.Draw_HISTOGRAM_BAR_X(_stat_X + _statPad, _stat_Y - (_statPad - (_stat_H * 0.75f)), _stat_W - _statPad2, _stat_H * -0.2f, P.Get(3, 0), P.Get(2), 0.75f, false, _values);
        GL.PopMatrix();
    }
    void DrawHoop(float _x, float _y, float _size, float _alphaStrength = 1f, float _start = 0, float _end = 1, float _angleOffset = 0)
    {
        float _HOOP_START = _HOOP_SIZE * _size;
        float _HOOP_THICKNESS = 0.25f * _size;
        float _HOOP_END = _HOOP_START + _HOOP_THICKNESS;
        float _ANGLE_START = _start + _angleOffset;
        float _ANGLE_END = _end + _angleOffset;

        GL_DRAW.Draw_ARC_FILL(_HOOP_SIDES, _x, _y, _ANGLE_START, _ANGLE_END, _HOOP_START, _HOOP_END, P.Get(3, _alphaStrength * 0.5f));
        GL_DRAW.Draw_ARC_FILL(_HOOP_SIDES, _x, _y, _ANGLE_START, _ANGLE_END, _HOOP_START, _HOOP_START + (_HOOP_THICKNESS * 0.25f), P.Get(3, _alphaStrength));
    }
    void WallCell(float _x, float _y, float _w, float _h, Color _col, int _sprawlIndex, float _animOffset, float _tickerOffset)
    {
        bool _TICKER = Anim.Runtime_int(5f, _tickerOffset) % 2 == 0;
        float _TOP = _y + _h;
        float _lw = _w * 0.5f;
        GL_DRAW.Draw_RECT(_x - (_w * 0.1f), _y + (_h * 0.1f), _lw, _h, COL.Set_alphaStrength(_col, 0.2f));
        GL_DRAW.Draw_RECT(_x - (_w * 0.05f), _y + (_h * 0.05f), _lw, _h, COL.Set_alphaStrength(_col, 0.2f));
        GL_DRAW.Draw_RECT_FILL(_x, _y, _lw, _h, COL.Set_alphaStrength(_col, 0.2f));
        GL_DRAW.Draw_RECT_FILL(_x, _TOP, _lw, _h * -0.1f, COL.Set_alphaStrength(_col, _TICKER ? 0.05f : 0.3f));
        HUD.Draw_LABEL_BOX("a" + _sprawlIndex, _x + (_lw * 0.1f), _y + (_h * 0.1f), _lw * 0.6f, _h * 0.6f, 0.01f, 0.1f, 0.5f, _col, P.Get(0));
        GL_MATRIX_ANIMS.Draw(GL_MATRIX_ANIMS.NAME_INC_3X3, Anim.Runtime_int(3f, _animOffset + Anim.Sin_Time(1.1f, 0f, 3f)), _x + _lw, _y + (_h * 0.75f), _h * 0.25f, _col);
        sprawls[_sprawlIndex].Draw(_x + _lw, _y + (_h * 0.75f), _lw * 0.25f, _h * -0.75f, COL.Set_alphaStrength(_col, 0.25f));
    }
}