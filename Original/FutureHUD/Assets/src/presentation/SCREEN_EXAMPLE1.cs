using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COLOUR;
using DATA;

public class SCREEN_EXAMPLE1 : SCREEN
{
    List<DataSprawl> sprawls;
    List<Partitions> partitionList;
    int totalSprawls = 20;
    int partitionListCount = 20;

    public SCREEN_EXAMPLE1()
    {
        duration = 30f;
        title = "hud example 1";
        P = COL.Get_Palette(0);

        // data sprawls
        sprawls = new List<DataSprawl>();
        int _minRows = 5;
        int _maxRows = 20;
        int _minRowLength = 10;
        int _maxRowLength = 50;
        int _minTickRate = 5;
        int _maxTickRate = 20;
        int _minCellRate = 4;
        int _maxCellRate = 10;


        for (int i = 0; i < totalSprawls; i++)
        {
            sprawls.Add(new DataSprawl(
                Random.Range(_minRows, _maxRows),
                Random.Range(_minRowLength, _maxRowLength),
                Random.Range(_minTickRate, _maxTickRate),
                Random.Range(_minCellRate, _maxCellRate)
            ));
        }

        // partitions
        float gutterRatio = 1;
        partitionList = new List<Partitions>();
        for (int i = 0; i < partitionListCount; i++)
        {
            partitionList.Add(new Partitions(Random.Range(3, 10), 0.7f));
        }
    }

    public override void BG()
    {
        GL_DRAW.Draw_BG_Y(P.Get(0), P.Get(1));
    }

    public override void Draw()
    {
        base.Draw();
        for (int i = 0; i < totalSprawls; i++)
        {
            DataSprawl _sp = sprawls[i];
            _sp.Update();
        }
        for (int i = 0; i < partitionListCount; i++)
        {
        }

        GL_DRAW.Draw_GRID_DOT(0, 0, 1, 1, 40, 40, P.Get(2, Anim.Sin_Time(25, 0.5f, 0.6f)));
        GL_DRAW.Draw_ZOOM_GRID(0, 0, 1, 1, P.Lerp(2, 3, my_eased), 20, 20, mx, my, 0.01f);

        // skewed edge rects
        GL_DRAW.SKEW(0.2f, 0f);


        HUD.Draw_LABEL_LINE_X("example: 1", 0.1f, 0.9f, Anim.Sin_Time(2f, 0.1f, 0.25f), 0.01f, P.Get(2), P.Get(3), P.Get(3));
        int _BOXES = 6;
        float _BOX_STARTX = 0.1f;
        float _BOX_SIZE = 0.05f;
        float _BOX_SPACING = 0.01f;
        for (int i = 0; i < _BOXES; i++)
        {
            HUD.Draw_LABEL_BOX(
                "exe_" + i,
                _BOX_STARTX + (i * _BOX_SIZE) + (i * _BOX_SPACING),
                0.8f,
                _BOX_SIZE,
                _BOX_SIZE,
                Anim.Sin_Time(10, 0.005f, 0.01f, i * 0.1f),
                0.1f,
                0.5f,
                P.Lerp(2, 3, (float)i / _BOXES),
                P.Get(0));
        }

        sprawls[0].Draw(0.05f, 0.5f, 0.05f, 0.2f, P.Get(3, Anim.Cos_Time(20, 0.1f, 0.15f, 0.1f)));
        sprawls[1].Draw(0.05f, 0.45f, 0.1f, 0.05f, P.Get(4, Anim.Cos_Time(20, 0.1f, 0.15f, 0.2f)));

        //GL_DRAW.Draw_RECT_FILL(0.1f, 0.5f, 0.1f, 0.1f, P.Get(2,Anim.Sin_Time(30,0.1f, 0.12f)));

        GL_DRAW.Draw_AXIS(0.1f, 0.1f, 0.1f, 0.5f, 0.01f, 0.005f, 20, 5, P.Get(2), P.Get(3));
        GL_DRAW.Draw_AXIS(0.11f, 0.1f, 0.11f, 0.5f, 0.01f, 0.005f, 40, 10, P.Get(2), P.Get(3));

        // histograms
        int _HIST_COUNT = 10;
        int _BIN_COUNT = 20;
        float _HIST_START_X = 0.25f;
        float _HIST_START_Y = 0.4f;
        float _HIST_END_X = 0.4f;
        float _HIST_END_Y = 0.1f;
        float _HIST_W = 0.4f;
        float _BIN_W = _HIST_W / _BIN_COUNT;
        float _HIST_H_MIN = 0.2f;
        float _HIST_H_MAX = 0.05f;
        float _HIST_H_DIV = (_HIST_H_MAX - _HIST_H_MIN) / _HIST_COUNT;
        float _HIST_OFFSET_X = (_HIST_END_X - _HIST_START_X) / _HIST_COUNT;
        float _HIST_OFFSET_Y = (_HIST_END_Y - _HIST_START_Y) / _HIST_COUNT;

        for (int i = 0; i < _HIST_COUNT; i++)
        {
            int i1 = i + 1;
            float _F = (float)i / _HIST_COUNT;
            float _RF = 1f - _F;
            HUD.Draw_HISTOGRAM_BAR_X(_HIST_START_X + (i * _HIST_OFFSET_X), _HIST_START_Y + (i * _HIST_OFFSET_Y), _HIST_W, _HIST_H_MIN + (i * _HIST_H_DIV),
                                     P.Lerp(1, 3, _F), P.Lerp(1, 4, _F), 1, false,
                                     VALUES.RandomValues_NOISE_TIME(_BIN_COUNT, 1f, 1.1f, i1 * 0.01f, i1 * 0.02f));

        }
        float[] _polyValues = VALUES.RandomValues_NOISE_TIME(10, 1f, 1.1f, 0.1f, 0.2f, 0.1f, 0.5f);
        float _polyX = _HIST_END_X + (_HIST_OFFSET_X);
        float _polyY = _HIST_END_Y + (_HIST_OFFSET_Y);
        HUD.Draw_HISTOGRAM_POLY(_polyX, _polyY, _HIST_W, _HIST_H_MAX, P.Get(2), _polyValues);
        HUD.Draw_HISTOGRAM_LINE_X(_polyX, _polyY, _HIST_W, _HIST_H_MAX, P.Get(2), P.Get(2), _polyValues);



        for (int i = 0; i < _BIN_COUNT; i++)
        {
            DataSprawl _SPRAWL = sprawls[i];
            _SPRAWL.Draw(_HIST_END_X + (i * _BIN_W), _HIST_END_Y - 0.1f, _BIN_W, 0.05f, P.Lerp(3, 4, Anim.Sin_Time(3f, _timeOffset: i * 0.1f)));
        }

        for (int i = 0; i < partitionListCount; i++)
        {
        }
    }
}
