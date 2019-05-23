using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COLOUR;
using DATA;

public class WINDOW
{
    public const float DEFAULT_CONTENT_PADDING_X = 0.1f;
    public const float DEFAULT_CONTENT_PADDING_Y = 0.1f;
    public float x, y, w, h;
    public GL_DRAW.GL_MATTRIX_TRANSFORM transform;
    public float pad_X, pad_Y, start_X, start_Y, active_X, active_Y;
    public Palette palette;

    public WINDOW(float _x = 0, float _y = 0, float _w = 1, float _h = 1, float _padX = DEFAULT_CONTENT_PADDING_X, float _padY = DEFAULT_CONTENT_PADDING_Y)
    {
        pad_X = _padX;
        pad_Y = _padY;
        transform = new GL_DRAW.GL_MATTRIX_TRANSFORM();
        Set_X(_x);
        Set_Y(_y);
        Set_W(_w);
        Set_H(_h);
        Set_Palette(0);
    }
    public void Set_Palette(int _index)
    {
        palette = COL.Get_Palette(_index);
    }
    public Color GetColour(int _colourIndex)
    {
        return palette.Get(_colourIndex);
    }
    public virtual void Update()
    {

    }
    public virtual void Draw()
    {
        GL_DRAW.TransformMatrix(transform);
    }

    public void Set_X(float _x)
    {
        x = transform.x = _x;
        start_X = x + (w * pad_X);
    }
    public void Set_Y(float _y)
    {
        y = transform.y = _y;
        start_Y = y + (h * pad_Y);
    }
    public void Set_W(float _w)
    {
        w = _w;
        active_X = w - (pad_X * 2);
    }
    public void Set_H(float _h)
    {
        h = _h;
        active_Y = h - (pad_Y * 2);
    }
}

public class WINDOW_HEADER
{
}

public class WINDOW_HISTOGRAMS : WINDOW
{

    public Histogram[] histograms;
    public int totalHistograms;
    // noise settings
    float rateA, rateB, offsetA, offsetB, incrementA, incrementB;
    // layout settings
    public GL_DRAW.GL_MATTRIX_TRANSFORM transform_start, transform_end;
    Color colour_A, colour_B;

    public WINDOW_HISTOGRAMS(float _x = 0, float _y = 0, float _w = 1, float _h = 1, float _padX = DEFAULT_CONTENT_PADDING_X, float _padY = DEFAULT_CONTENT_PADDING_Y) : base(_x, _y, _w, _h, _padX, _padY)
    {
    }

    public void Init(
        int _totalHistograms = 10,
        int _bins_MIN = 10,
        int _bins_MAX = 10,

        // noise Settings
        float _rateA = 1,
        float _rateB = 1,
        float _offsetA = 0.1f,
        float _offsetB = 0.05f,
        float _incrementA = 0.01f,
        float _incrementB = 0.005f,

        // colour
        int _colour_A = 0,
        int _colour_B = 0
    )
    {

        totalHistograms = _totalHistograms;

        // layout
        transform_start = new GL_DRAW.GL_MATTRIX_TRANSFORM();
        transform_end = new GL_DRAW.GL_MATTRIX_TRANSFORM();

        // noise settings
        rateA = _rateA;
        offsetA = _offsetA;
        incrementA = _incrementA;
        rateB = _rateB;
        offsetB = _offsetB;
        incrementB = _incrementB;

        colour_A = GetColour(_colour_A);
        colour_B = GetColour(_colour_B);

        histograms = new Histogram[totalHistograms];
        for (int i = 0; i < _totalHistograms; i++)
        {
            histograms[i] = new Histogram(Random.Range(_bins_MIN, _bins_MAX));
        }
    }

    public override void Update()
    {
        for (int i = 0; i < totalHistograms; i++)
        {
            histograms[i].UpdateNoise(rateA, offsetA * i, incrementA, rateB, offsetB * i, incrementB);
        }
    }

    public override void Draw()
    {
        base.Draw();
        for (int i = 0; i < totalHistograms; i++)
        {
            GL.PushMatrix();
            GL_DRAW.TransformLerp(transform_start, transform_end, (float)i / totalHistograms);
            HUD.Draw_HISTOGRAM_POLY_FILL(0f, 0f, 0.5f, 0.2f, colour_A, colour_B, histograms[i].values);
            //GL_DRAW.Draw_LINE(0f, 0f, 0.1f, 0f, Color.cyan);
            GL.PopMatrix();
        }
    }
}
