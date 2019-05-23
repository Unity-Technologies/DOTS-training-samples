using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COLOUR;
using DATA;

#region SCREEN 1 - Welcome

public class SCREEN_1_WELCOME : SCREEN
{
    public SCREEN_1_WELCOME()
    {
        duration = 10f;
        title = "welcome";
        P = COL.Get_Palette(1);
    }

    public override void BG()
    {
        GL_DRAW.Draw_BG(P.Get(0));
    }

    public override void Draw()
    {
        base.Draw();

        TXT("Hi there!", 0.2f, 0.8f, P.Get(Anim.Runtime_int(3f) % P.totalColours), PRESENTATION.DEFAULT_TXT_CELL_HEIGHT * 2);
        TXT("Here is some info on the GL drawing tools", 0.2f, 0.75f, P.Get(4));

        TXT("draw primitives!", 0.2f, 0.6f, P.Get(4));

        int _CELLS_X = 10;
        int _CELLS_Y = 3;
        float _XDIV = 0.6f / _CELLS_X;
        float _YDIV = 0.4f / _CELLS_Y;
        float _CELL_SIZE = _XDIV * 0.75f;
        float _CS2 = _CELL_SIZE * 0.5f;

        int _COUNT = 0;
        for (int x = 0; x < _CELLS_X; x++)
        {
            for (int y = 0; y < _CELLS_Y; y++)
            {
                _COUNT++;
                Color _COL = P.Get(_COUNT % P.totalColours);
                GL.PushMatrix();
                //GL_DRAW.TransformMatrix(0.1f + (x * _XDIV), 0.1f + (y * _YDIV), _rotationZ: Anim.PNoise(0.1f, _offsetA: x, _offsetB: y));
                float _CS = Anim.Sin_Time(10f, _CS2, _CELL_SIZE, _COUNT);
                GL_DRAW.Translate(0.1f + (x * _XDIV), 0.1f + (y * _YDIV));

                switch (_COUNT % 6)
                {
                    case 0:
                        GL_DRAW.Draw_RECT(0, 0, _CS, _CS, _COL);
                        break;
                    case 1:
                        GL_DRAW.Draw_RECT_FILL(0, 0, _CS, _CS, _COL);
                        break;
                    case 2:
                        GL_DRAW.Draw_CIRCLE_LINE(3, 0, 0, _CS, _COL);
                        break;
                    case 3:
                        GL_DRAW.Draw_CIRCLE_FILL(3, 0, 0, _CS, _COL);
                        break;
                    case 4:
                        GL_DRAW.Draw_CIRCLE_LINE(20, 0, 0, _CS, _COL);
                        break;
                    case 5:
                        GL_DRAW.Draw_CIRCLE_FILL(20, 0, 0, _CS, _COL);
                        break;
                }
                GL.PopMatrix();
            }
        }
    }
}
#endregion
#region SCREEN 2 - Drawing + Anim
public class SCREEN_2_DRAWING : SCREEN
{
    public SCREEN_2_DRAWING()
    {
        duration = 10f;
        title = "anim";
        P = COL.Get_Palette(0);
    }

    public override void BG()
    {
        GL_DRAW.Draw_BG_Y(P.Get(0), P.Get(1));
    }

    public override void Draw()
    {
        base.Draw();



        int _COUNT = 20;
        float _MIN = 0.02f;
        float _MAX = 0.05f;
        float _OFFSET = -0.2f;
        for (int i = 0; i < _COUNT; i++)
        {
            GL_DRAW.Draw_CIRCLE_FILL(20, Anim.Sin_Time(3f, 0.1f, 0.8f, i * _OFFSET), Anim.Cos_Time(1.5f, 0.1f, 0.8f, i * _OFFSET), Anim.Cos_Time(1.5f, _MIN, _MAX, i * _OFFSET), P.Lerp(0, 3, (float)i / _COUNT));
        }
        GL_DRAW.Draw_CHEVRON(mx_eased + 0.05f, my_eased + Mathf.Abs(Anim.Cos_Time(10f, 0f, 0.05f)), 0.1f, Anim.Sin_Time(10f, 0.15f, 0.17f), P.Lerp(3, 4, mx_eased));
        TXT("Use the \"anim\" class for simple animations", 0.2f, 0.75f, P.Get(4));
    }
}
#endregion
#region SCREEN 3 - GRIDS
public class SCREEN_3_GRIDS : SCREEN
{
    public SCREEN_3_GRIDS()
    {
        duration = 10f;
        title = "grids";
        P = COL.Get_Palette(0);
    }
    
    public override void BG()
    {
        GL_DRAW.Draw_BG_Y(P.Get(0), P.Get(1));
    }
    
    public override void Draw()
    {
        base.Draw();
        int _DIVS_X = 20;
        int _DIVS_Y = 20;
        float _txt_x = 0.1f;
        float _txt_y = 0.9f;
        float _start_x = 0.1f;
        float _start_y = 0.1f;
        float _w = 0.7f;
        float _h = 0.7f;
        float _grid_offset_x = Anim.PNoise(0.1f, 0.05f);
        float _grid_offset_y = Anim.PNoise(0.1f, 0.05f, _offsetA: 0.5f, _offsetB: 0.1f);
        Color _gridColour = P.Get(2);
        Color _TXT_COL = P.Lerp(3, 4, Anim.Sin_Time(10));
        float _TXT_SIZE = Anim.Cos_Time(15, 0.0035f, 0.005f);
        if (timeRemaining < 0.2f)
        {
            TXT("DOTS", _txt_x, _txt_y, _TXT_COL, _TXT_SIZE);
            GL_DRAW.Draw_GRID_DOT(_start_x, _start_y, _w, _h, _DIVS_X, _DIVS_Y, _gridColour);
        }
        else if (timeRemaining < 0.4f)
        {
            TXT("LINES", _txt_x, _txt_y, _TXT_COL, _TXT_SIZE);
            GL_DRAW.Draw_GRID_LINE(_start_x, _start_y, _w, _h, _DIVS_X, _DIVS_Y, _gridColour, _grid_offset_x, _grid_offset_y);
        }
        else if (timeRemaining < 0.6f)
        {
            TXT("triangles", _txt_x, _txt_y, _TXT_COL, _TXT_SIZE);
            GL_DRAW.Draw_GRID_NGON(_start_x, _start_y, _w, _h, _DIVS_X, _DIVS_Y, 3, 0.01f, _gridColour);
        }
        else
        {
            TXT("zoom!!", _txt_x, _txt_y, _TXT_COL, _TXT_SIZE);
            GL_DRAW.Draw_ZOOM_GRID(_start_x, _start_y, _w, _h, _gridColour, _DIVS_X, _DIVS_Y, mx_eased, my_eased, 0.1f);
        }
    }
}
#region SCREEN 4 - Graphs
#endregion
public class SCREEN_4_GRAPHS : SCREEN
{
    Graph _arcGraph1, _arcGraph2;
    public SCREEN_4_GRAPHS()
    {
        duration = 10f;
        title = "graphing";
        P = COL.Get_Palette(0);
        _arcGraph1 = new Graph(5);
        _arcGraph2 = new Graph(20);
    }

    public override void BG()
    {
        GL_DRAW.Draw_BG_Y(P.Get(1), P.Get(0));
    }

    public override void Draw()
    {
        base.Draw();

        float _startX = 0.1f;
        float _w = 0.8f;
        float _h = 0.075f;
        float _offsetA = 0.1f;
        float _offsetB = 0.05f;
        Color _colA = P.Get(3);
        Color _colB = P.Get(4);
        HUD.Draw_HISTOGRAM_LINE_X(_startX, 0.05f, _w, _h, _colA, _colB, VALUES.RandomValues_NOISE_TIME(15, _offsetA: _offsetA, _offsetB: _offsetB));
        HUD.Draw_HISTOGRAM_BAR_X(_startX, 0.14f, _w, _h, _colA, _colB, 1f, false, VALUES.RandomValues_NOISE_TIME(30, _offsetA: _offsetA, _offsetB: _offsetB));
        HUD.Draw_HISTOGRAM_BAR_X(_startX, 0.21f, _w, _h, _colB, _colA, 0.9f, true, VALUES.RandomValues_NOISE_TIME(20, _offsetA: _offsetA, _offsetB: _offsetB));
        HUD.Draw_HISTOGRAM_POLY(_startX, 0.29f, _w, _h, _colB, VALUES.RandomValues_NOISE_TIME(5, _offsetA: _offsetA * 2f, _offsetB: _offsetB * 2f));
        HUD.Draw_HISTOGRAM_POLY_FILL(_startX, 0.36f, _w, _h, _colA, _colB, VALUES.RandomValues_NOISE_TIME(5, _offsetA: _offsetA * 2f, _offsetB: _offsetB * 2f));

        HUD.Draw_HISTOGRAM_RADIAL(VALUES.RandomValues_NOISE_TIME(10, _offsetA: _offsetA, _offsetB: _offsetB), 0.2f, 0.6f, 0.05f, 0.15f, _colA, _colB);
        HUD.Draw_HISTOGRAM_RADIAL(VALUES.RandomValues_NOISE_TIME(40, _rateA: 1f, _rateB: 3f, _offsetA: _offsetA, _offsetB: _offsetB), 0.4f, 0.6f, 0.05f, 0.15f, _colB, _colA);

        _arcGraph1.UpdateNoise();
        _arcGraph2.UpdateNoise();

        HUD.Draw_ArcGraph(_arcGraph1, 0.6f, 0.6f, 0.05f, 0.1f, 0, 1, _colA, _colB);
        HUD.Draw_ArcGraph(_arcGraph2, 0.8f, 0.6f, 0.01f, 0.1f, 0, 0.5f, _colB, _colA, _gutterRatio: 1f);

        TXT("use methods in \"Values\" to create graphs", 0.1f, 0.9f, P.Get(4));
    }
}
#endregion
#region SCREEN 5 - More fun
public class SCREEN_5_MORE_FUN : SCREEN
{
    List<DataSprawl> sprawls;
    List<Partitions> partitionList;
    int totalSprawls = 10;
    int partitionListCount = 10;
    public SCREEN_5_MORE_FUN()
    {
        duration = 15f;
        title = "more fun stuff";
        P = COL.Get_Palette(1);

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
    void DrawSprawl(int _index, float _x, float _y, float _w, float _h, Color _col){
        DataSprawl _sp = sprawls[_index];
        _sp.Update();
        _sp.Draw(_x, _y, _w, _h, _col);

    }
    public override void BG()
    {
        GL_DRAW.Draw_BG_Y(P.Get(0), P.Lerp(2, 3, Anim.Sin_Time(5f)));
    }

    public override void Draw()
    {
        base.Draw();

        // sprawls
        TXT("data sprawls :)", 0.1f, 0.9f, P.Get(4));
        float _DIVX = 0.8f / totalSprawls;
        for (int i = 0; i < totalSprawls; i++)
        {
            DataSprawl _sp = sprawls[i];
            _sp.Update();
            _sp.Draw(0.1f + (_DIVX * i), 0.7f, _DIVX, 0.1f, P.Get(i % P.totalColours));
        }


        // partitions
        TXT("partitions D:", 0.1f, 0.6f, P.Get(4));
        float _DIV_P = 0.2f / partitionListCount;
        float _DIV_RAD = 0.2f / partitionListCount;
        float _DIV_ANGLE = 0.5f / partitionListCount;
        for (int i = 0; i < partitionListCount; i++)
        {
            Partitions _P = partitionList[i];
            _P.AddRandom(0.001f, 1f, 1.2f, i, 1.5f * i, 0.01f * i, 0.05f * i);
            _P.ColourByShare(Color.red, Color.white);
            HUD.Draw_BAR_PARTITIONS_FILL_X(0.5f, 0.4f-(i*_DIV_P), 0.4f, _DIV_P, partitionList[i]);
            GL.PushMatrix();
            GL_DRAW.Rotate(Anim.Runtime(i * 0.005f));
            HUD.Draw_ARC_PARTITIONS_FILL(partitionList[i], 20, 0.2f, 0.3f, 0.01f + (i * _DIV_RAD), _DIV_RAD, 0f, Anim.Sin_Time(1,0.25f, _DIV_ANGLE*i, i));
            GL.PopMatrix();
        }
    }
}
#endregion