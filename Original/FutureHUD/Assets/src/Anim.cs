using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Anim
{
    public static float time = 100;
    public static float Sin_Time(float _rate = 1, float _min = 0.0f, float _max = 1.0f, float _timeOffset = 0)
    {
        float _HALF_RANGE = (_max - _min) * 0.5f;
        return (_min + _HALF_RANGE) + Mathf.Sin(Runtime(_rate, _timeOffset)) * _HALF_RANGE;
    }

    public static float Cos_Time(float _rate = 1, float _min = 0.0f, float _max = 1.0f, float _timeOffset = 0)
    {
        float _HALF_RANGE = (_max - _min) * 0.5f;
        return (_min + _HALF_RANGE) + Mathf.Cos(Runtime(_rate, _timeOffset)) * _HALF_RANGE;
    }

    public static float Runtime(float _rate = 1f, float _offset = 0)
    {
        return (time + _offset) * _rate;
    }
    public static int Runtime_int(float _rate = 1f, float _offset = 0)
    {
        return Mathf.FloorToInt((time + _offset) * _rate);
    }

    public static float PNoise(float _rateA = 1, float _rateB = 1.1f, float _offsetA = 0.1f, float _offsetB = 0.2f, float _incrementA = 0.1f, float _incrementB = 0.2f, float _min = 0, float _max = 1)
    {
        return _min + (_max - _min) * Mathf.PerlinNoise(Anim.Runtime(_rateA, _incrementA) + _offsetA, Anim.Runtime(_rateB, _incrementB) + _offsetB);
    }
    public static Color Colour_OSCILLATOR(Color _A, Color _B, float _rate = 1, float _offset = 0)
    {
        return Color.Lerp(_A, _B, Sin_Time(_rate: _rate, _timeOffset: +_offset));
    }
    public static Color Colour_SWITCH(Color _A, Color _B, float _ratio = 0.5f, float _rate = 1, float _offset = 0)
    {
        return Color.Lerp(_A, _B, ((Anim.Runtime(_rate, _offset) % 1) > _ratio) ? 0 : 1);
    }
}
public class GL_MATRIX_ANIM
{
    public string name;
    public int totalFrames = 0;
    public int cellsX, cellsY;
    public List<BitArray> frames;
    public GL_MATRIX_ANIM(string _name, int _cellsX = 3, int _cellsY = 3)
    {
        name = _name;
        cellsX = _cellsX;
        cellsY = _cellsY;
        frames = new List<BitArray>();
    }
    public void AddFrame(params int[] _cells)
    {
        BitArray result = new BitArray(_cells.Length);
        for (int i = 0; i < _cells.Length; i++)
        {
            result[i] = (_cells[i] == 1) ? true : false;
        }
        frames.Add(result);
        totalFrames = frames.Count;
    }
    public BitArray Get(int _frame)
    {
        return frames[_frame];
    }
}
public class GL_MATRIX_ANIMS
{
    public static string NAME_SPINNER_3X3 = "SPINNER_3X3";
    public static string NAME_INC_3X3 = "INC_3X3";

    public static Dictionary<string, GL_MATRIX_ANIM> animations = new Dictionary<string, GL_MATRIX_ANIM>();
    public static void Init()
    {
        GL_MATRIX_ANIM _ANIM_SPINNER = new GL_MATRIX_ANIM(NAME_SPINNER_3X3);
        _ANIM_SPINNER.AddFrame(1, 0, 0, 0, 1, 0, 0, 0, 1);
        _ANIM_SPINNER.AddFrame(0, 1, 0, 0, 1, 0, 0, 1, 0);
        _ANIM_SPINNER.AddFrame(0, 0, 1, 0, 1, 0, 1, 0, 0);
        _ANIM_SPINNER.AddFrame(0, 0, 0, 1, 1, 1, 0, 0, 0);
        animations.Add(_ANIM_SPINNER.name, _ANIM_SPINNER);

        GL_MATRIX_ANIM _ANIM_INC = new GL_MATRIX_ANIM(NAME_INC_3X3);
        _ANIM_INC.AddFrame(0, 0, 0, 0, 0, 0, 0, 0, 0);
        _ANIM_INC.AddFrame(0, 1, 0, 0, 0, 0, 0, 0, 0);
        _ANIM_INC.AddFrame(0, 0, 1, 0, 0, 0, 0, 0, 0);
        _ANIM_INC.AddFrame(0, 0, 0, 1, 0, 0, 0, 0, 0);
        _ANIM_INC.AddFrame(0, 0, 0, 0, 1, 0, 0, 0, 0);
        _ANIM_INC.AddFrame(0, 0, 0, 0, 0, 1, 0, 0, 0);
        _ANIM_INC.AddFrame(0, 0, 0, 0, 0, 0, 1, 0, 0);
        _ANIM_INC.AddFrame(0, 0, 0, 0, 0, 0, 0, 1, 0);
        _ANIM_INC.AddFrame(0, 0, 0, 0, 0, 0, 0, 0, 1);
        animations.Add(_ANIM_INC.name, _ANIM_INC);
    }
    public static void Draw(string _animName, int _frame, float _x, float _y, float _w, Color _col, float _rotation = 0)
    {
        GL_MATRIX_ANIM _ANIM = animations[_animName];
        Draw_MATRIX_ANIM_FRAME(_ANIM.frames, _frame, _ANIM.cellsX, _ANIM.cellsY, _ANIM.totalFrames, _x, _y, _w, _col);
    }
    public static void Draw_RADIAL(string _animName, int _frame, float _x, float _y, float _radius_START, float _radius_END, Color _col, float _angle_START = 0, float _angle_END = 1, int _arcSides = HUD.DEFAULT_ARC_SIDES, float _gutterRatio_angle = HUD.DEFAULT_GUTTER_RATIO, float _gutterRatio_radius = HUD.DEFAULT_GUTTER_RATIO, float _rotation = 0)
    {
        GL_MATRIX_ANIM _ANIM = animations[_animName];
        Draw_MATRIX_ANIM_FRAME_RADIAL(_ANIM.frames, _frame, _ANIM.cellsX, _ANIM.cellsY, _ANIM.totalFrames, _x, _y, _radius_START, _radius_END, _col, _arcSides, _angle_START, _angle_END, _gutterRatio_angle, _gutterRatio_radius, _rotation);
    }
    public static void Draw_MATRIX_ANIM_FRAME(List<BitArray> _frames, int _frame, int _cellsX, int _cellsY, int _totalFrames, float _x, float _y, float _w, Color _col, float _rotation = 0)
    {
        GL_DRAW.Draw_MATRIX_RECT(_x, _y, _w, GL_DRAW.LockAspect_Y(_w), _cellsX, _cellsY, _col, _frames[_frame % _totalFrames], _rotation);
    }
    public static void Draw_MATRIX_ANIM_FRAME_RADIAL(
        List<BitArray> _frames,
        int _frame,
        int _cells_angle,
        int _cells_radius,
        int _totalFrames,
        float _x,
        float _y,
        float _radius_start,
        float _radius_end,
        Color _col,
        int _arcSides = HUD.DEFAULT_ARC_SIDES,
        float _angle_start = 0, float _angle_end = 1,
        float _gutterRatio_ANGLE = HUD.DEFAULT_GUTTER_RATIO,
        float _gutterRatio_RADIUS = HUD.DEFAULT_GUTTER_RATIO,
        float _rotation = 0)
    {
        GL_DRAW.Draw_MATRIX_RADIAL(_x, _y, _radius_start, _radius_end, _cells_angle, _cells_radius, _col, _frames[_frame % _totalFrames], _arcSides, _angle_start, _angle_end, _gutterRatio_ANGLE, _gutterRatio_RADIUS, _rotation);
    }
}
