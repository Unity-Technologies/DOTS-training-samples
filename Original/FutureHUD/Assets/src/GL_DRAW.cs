using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_DRAW
{

    public static float PI2 = Mathf.PI * 2.0f;
    static float Z = 0;
    public static float SKEW_X = 0f;
    public static float SKEW_Y = 0f;
    public static void RESET_SKEW()
    {
        SKEW_X = 0f;
        SKEW_Y = 0f;
    }
    public static void SKEW(float _x = 0, float _y = 0)
    {
        SKEW_X = _x;
        SKEW_Y = _y;
    }

    public struct Vert
    {
        public float x, y;
        public Color c;
        public Vert(float _x, float _y, Color _col)
        {
            x = _x;
            y = _y;
            c = _col;
        }
    }
    public class GL_MATTRIX_TRANSFORM
    {
        public float x, y, z, rotX, rotY, rotZ, sclX, sclY, sclZ;
        public GL_MATTRIX_TRANSFORM(
            float _x = 0, float _y = 0, float _z = 0,
            float _rotX = 0, float _rotY = 0, float _rotZ = 0,
            float _sclX = 1f, float _sclY = 1f, float _sclZ = 1f)
        {
            x = _x;
            y = _y;
            z = _z;
            rotX = _rotX;
            rotY = _rotY;
            rotZ = _rotZ;
            sclX = _sclX;
            sclY = _sclY;
            sclZ = _sclZ;
        }
    }

    public static float LockAspect_Y(float _y)
    {
        return _y * ((float)Screen.width / Screen.height);
    }
    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }
    public static Vector2 PolarCoord(float _theta, float _radius)
    {
        float _X = Mathf.Sin(_theta) * _radius;
        float _Y = Mathf.Cos(_theta) * _radius;
        return new Vector2(_X, _Y);
    }
    public static Vector2 PolarCoord_1to1(float _theta, float _radius)
    {
        float _X = Mathf.Sin(_theta) * _radius;
        float _Y = Mathf.Cos(_theta) * _radius;
        return new Vector2(_X, LockAspect_Y(_Y));
    }
    public static Vector2 PolarCoord2(float _theta, float _radiusX, float _radiusY)
    {
        float _X = Mathf.Sin(_theta) * _radiusX;
        float _Y = Mathf.Cos(_theta) * _radiusY;
        return new Vector2(_X, _Y);
    }

    public static void TransformMatrix(float _x, float _y, float _rotationX = 0, float _rotationY = 0, float _rotationZ = 0, float _scaleX = 1, float _scaleY = 1, float _scaleZ = 1)
    {
        Matrix4x4 model = GL.modelview;
        Matrix4x4 m = Matrix4x4.TRS(new Vector3(_x, _y, Z), Quaternion.Euler(_rotationX * 360f, _rotationY * 360f, _rotationZ * 360f), new Vector3(_scaleX, _scaleY, _scaleZ));

        GL.MultMatrix(m * model);
    }
    public static void TransformMatrix(GL_MATTRIX_TRANSFORM _t)
    {
        Matrix4x4 model = GL.modelview;
        Matrix4x4 m = Matrix4x4.TRS(new Vector3(_t.x, _t.y, _t.z), Quaternion.Euler(_t.rotX * 360f, _t.rotY * 360f, _t.rotZ * 360f), new Vector3(_t.sclX, _t.sclY, _t.sclZ));

        GL.MultMatrix(m * model);
    }
    public static void Rotate(float _angle)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_rotZ: _angle);
        TransformMatrix(_TRANSFORM);
    }
    public static void RotateX(float _angle)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_rotX: _angle);
        TransformMatrix(_TRANSFORM);
    }
    public static void RotateY(float _angle)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_rotY: _angle);
        TransformMatrix(_TRANSFORM);
    }

    // TRANSLATE POSITION
    public static void TranslateX(float _value)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_x: _value);
        TransformMatrix(_TRANSFORM);
    }
    public static void TranslateY(float _value)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_y: _value);
        TransformMatrix(_TRANSFORM);
    }
    public static void TranslateZ(float _value)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_z: _value);
        TransformMatrix(_TRANSFORM);
    }
    public static void Translate(float _x, float _y, float _z = 0)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_x: _x, _y: _y, _z: _z);
        TransformMatrix(_TRANSFORM);
    }

    // SCALE
    public static void ScaleX(float _value)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_sclX: _value);
        TransformMatrix(_TRANSFORM);
    }
    public static void ScaleY(float _value)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_sclY: _value);
        TransformMatrix(_TRANSFORM);
    }
    public static void ScaleZ(float _value)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_sclZ: _value);
        TransformMatrix(_TRANSFORM);
    }
    public static void Scale(float _x, float _y, float _z = 1)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_sclX: _x, _sclY: _y, _sclZ: _z);
        TransformMatrix(_TRANSFORM);
    }
    public static void Scale(float _value)
    {
        GL_MATTRIX_TRANSFORM _TRANSFORM = new GL_MATTRIX_TRANSFORM(_sclX: _value, _sclY: _value, _sclZ: _value);
        TransformMatrix(_TRANSFORM);
    }

    public static void TransformLerp(GL_MATTRIX_TRANSFORM _A, GL_MATTRIX_TRANSFORM _B, float _value)
    {
        float _X = Mathf.Lerp(_A.x, _B.x, _value);
        float _Y = Mathf.Lerp(_A.y, _B.y, _value);
        float _Z = Mathf.Lerp(_A.z, _B.z, _value);

        float _ROT_X = Mathf.Lerp(_A.rotX, _B.rotX, _value);
        float _ROT_Y = Mathf.Lerp(_A.rotY, _B.rotY, _value);
        float _ROT_Z = Mathf.Lerp(_A.rotZ, _B.rotZ, _value);

        float _SCL_X = Mathf.Lerp(_A.sclX, _B.sclX, _value);
        float _SCL_Y = Mathf.Lerp(_A.sclY, _B.sclY, _value);
        float _SCL_Z = Mathf.Lerp(_A.sclZ, _B.sclZ, _value);
        TransformMatrix(new GL_MATTRIX_TRANSFORM(_X, _Y, _Z, _ROT_X, _ROT_Y, _ROT_Z, _SCL_X, _SCL_Y, _SCL_Z));
    }
    public static void Add_VERT(float _x, float _y, Color _col)
    {
        GL.Color(_col);
        GL.Vertex3(_x + (_y * SKEW_Y), _y + (_x * SKEW_X), Z);
    }
    public static void Add_VERT(Vert _v)
    {
        GL.Color(_v.c);
        GL.Vertex3(_v.x + (_v.y * SKEW_Y), _v.y + (_v.x * SKEW_X), Z);
    }
    public static void Add_VERT_1to1(float _x, float _y, Color _col)
    {
        GL.Color(_col);
        GL.Vertex3(_x + (_y * SKEW_Y), _y + (_x * SKEW_X), Z);
    }
    public static void Add_VERT_1to1(Vert _v)
    {
        GL.Color(_v.c);
        GL.Vertex3(_v.x + (_v.y * SKEW_Y), _v.y * SKEW_Y, Z);
    }

    public static void Draw_LINE(float _startX, float _startY, float _endX, float _endY, Color _col_start, Color _col_end)
    {
        GL.Begin(GL.LINES);
        Add_VERT(_startX, _startY, _col_start);
        Add_VERT(_endX, _endY, _col_end);
        GL.End();
    }
    public static void Draw_LINE(float _startX, float _startY, float _endX, float _endY, Color _col)
    {
        Draw_LINE(_startX, _startY, _endX, _endY, _col, _col);
    }
    public static void DRAW_POINT(float _x, float _y, Color _col)
    {
        Draw_LINE(_x, _y, _x + 0.001f, _y + 0.001f, _col);
    }
    public static void Draw_BG(Color _col)
    {
        Draw_RECT_FILL(0, 0, 1, 1, _col);
    }
    public static void Draw_BG(Color _topLeft, Color _topRight, Color _bottomRight, Color _bottomLeft)
    {
        Draw_GRADIENT_RECT_4(0, 0, 1, 1, _topLeft, _topRight, _bottomRight, _bottomLeft);
    }
    public static void Draw_BG_X(Color _left, Color _right)
    {
        Draw_GRADIENT_RECT_4(0, 0, 1, 1, _left, _right, _right, _left);
    }
    public static void Draw_BG_Y(Color _top, Color _btm)
    {
        Draw_GRADIENT_RECT_4(0, 0, 1, 1, _top, _top, _btm, _btm);
    }
    public static void Draw_GRADIENT_RECT_4(float _x, float _y, float _w, float _h, Color _topLeft, Color _topRight, Color _bottomRight, Color _bottomLeft)
    {
        GL.Begin(GL.QUADS);
        Add_VERT(_x, _y, _bottomLeft);
        Add_VERT(_x + _w, _y, _bottomRight);
        Add_VERT(_x + _w, _y + _h, _topRight);
        Add_VERT(_x, _y + _h, _topLeft);
        GL.End();
    }
    public static void Draw_GRADIENT_RECT_X(float _x, float _y, float _w, float _h, Color _colA, Color _colB)
    {
        GL.Begin(GL.QUADS);
        Add_VERT(_x, _y, _colA);
        Add_VERT(_x + _w, _y, _colB);
        Add_VERT(_x + _w, _y + _h, _colB);
        Add_VERT(_x, _y + _h, _colA);
        GL.End();
    }
    public static void Draw_GRADIENT_RECT_Y(float _x, float _y, float _w, float _h, Color _colA, Color _colB)
    {
        GL.Begin(GL.QUADS);
        Add_VERT(_x, _y, _colA);
        Add_VERT(_x + _w, _y, _colA);
        Add_VERT(_x + _w, _y + _h, _colB);
        Add_VERT(_x, _y + _h, _colB);
        GL.End();
    }
    public static void Draw_POLY_LINE(Color _col, params Vector2[] _verts)
    {
        GL.Begin(GL.LINE_STRIP);

        for (int vertIndex = 0; vertIndex < _verts.Length; vertIndex++)
        {
            Vector2 _tempPos = _verts[vertIndex];
            Add_VERT(_tempPos.x, _tempPos.y, _col);
        }
        GL.End();
    }
    public static void Draw_POLY_LINE(params Vert[] _verts)
    {
        GL.Begin(GL.LINE_STRIP);

        for (int vertIndex = 0; vertIndex < _verts.Length; vertIndex++)
        {
            Vert _V = _verts[vertIndex];
            Add_VERT(_V); ;
        }
        GL.End();
    }
    public static void Draw_POLY_LINE_CLOSED(Color _col, params Vector2[] _verts)
    {
        GL.Begin(GL.LINE_STRIP);
        Vector2 _start = _verts[0];
        Add_VERT(_start.x, _start.y, _col);
        for (int vertIndex = 1; vertIndex < _verts.Length; vertIndex++)
        {
            Vector2 _tempPos = _verts[vertIndex];
            Add_VERT(_tempPos.x, _tempPos.y, _col);
        }
        Add_VERT(_start.x, _start.y, _col);
        GL.End();
    }
    public static void Draw_POLY_LINE_CLOSE(params Vert[] _verts)
    {
        GL.Begin(GL.LINE_STRIP);
        Vert _start = _verts[0];
        Add_VERT(_start);
        for (int vertIndex = 1; vertIndex < _verts.Length; vertIndex++)
        {
            Vert _V = _verts[vertIndex];
            Add_VERT(_V);
        }
        Add_VERT(_start);
        GL.End();
    }

    public static void Draw_TRIANGLE(float _x, float _y, float _size, Color _col, float _rotation = 0)
    {
        float _SIZE2 = _size * 0.5f;

        GL.PushMatrix();
        TransformMatrix(_x, _y, _rotation);
        GL.Begin(GL.LINE_STRIP);

        Add_VERT(0, 0, _col);
        Add_VERT(-_SIZE2, -_size, _col);
        Add_VERT(_SIZE2, -_size, _col);
        Add_VERT(0, 0, _col);

        GL.End();
        GL.PopMatrix();
    }

    public static void Draw_TRIANGLE_FILL(float _x, float _y, float _size, Color _col, float _rotation)
    {
        float _SIZE2 = _size * 0.5f;

        GL.PushMatrix();
        TransformMatrix(_x, _y, _rotation);
        GL.Begin(GL.TRIANGLES);

        Add_VERT(0, 0, _col);
        Add_VERT(-_SIZE2, -_size, _col);
        Add_VERT(_SIZE2, -_size, _col);

        GL.End();
        GL.PopMatrix();
    }

    public static void Draw_RECT(float _x, float _y, float _w, float _h, Color _col)
    {

        GL.Begin(GL.LINE_STRIP);

        Add_VERT(_x, _y, _col);
        Add_VERT(_x + _w, _y, _col);
        Add_VERT(_x + _w, _y + _h, _col);
        Add_VERT(_x, _y + _h, _col);
        Add_VERT(_x, _y, _col);

        GL.End();
    }
    public static void Draw_RECT_FILL(float _x, float _y, float _w, float _h, Color _col)
    {
        GL.Begin(GL.QUADS);

        Add_VERT(_x, _y, _col);
        Add_VERT(_x + _w, _y, _col);
        Add_VERT(_x + _w, _y + _h, _col);
        Add_VERT(_x, _y + _h, _col);

        GL.End();
    }
    public static void Draw_QUAD(Vert _a, Vert _b, Vert _c, Vert _d)
    {
        GL.Begin(GL.QUADS);

        Add_VERT(_a);
        Add_VERT(_b);
        Add_VERT(_c);
        Add_VERT(_d);

        GL.End();
    }

    public static void Draw_NGON_LINE
    (int _sides, float _x, float _y, float _size, Color _col, float _rotation = 0)
    {
        _sides += 1;
        GL.PushMatrix();

        GL.Begin(GL.LINE_STRIP);
        TransformMatrix(_x, _y, _rotation);
        float _DIV = (Mathf.PI * 2) / _sides;
        Vector2 _START = PolarCoord(0, _size);

        Add_VERT_1to1(_START.x, _START.y, _col);
        for (int i = 1; i < _sides; i++)
        {
            Vector2 _POLAR = PolarCoord(i * _DIV, _size);
            Add_VERT_1to1(_POLAR.x, _POLAR.y, _col);
        }
        Add_VERT_1to1(_START.x, _START.y, _col);
        GL.End();

        GL.PopMatrix();
    }
    public static void Draw_NGON(int _sides, float _x, float _y, float _size, float _thickness, Color _col, float _rotation = 0)
    {
        Draw_ARC_FILL(_sides + 1, _x, _y, 0f, 1f, _size, _size + _thickness, _col, _rotation);
    }
    public static void Draw_NGON_FILL(int _sides, float _x, float _y, float _size, Color _col, float _rotation = 0)
    {
        Draw_ARC_FILL(_sides + 1, _x, _y, 0f, 1f, 0f, _size, _col, _rotation);
    }
    public static void Draw_ELLIPSE_LINE(int _segments, float _x, float _y, float _w, float _h, Color _col)
    {


        GL.Begin(GL.LINE_STRIP);
        float _DIV = (Mathf.PI * 2) / _segments;
        Vector2 _START = PolarCoord2(0, _w, _h);

        Add_VERT(_START.x + _x, _START.y + _y, _col);
        for (int i = 1; i < _segments; i++)
        {
            Vector2 _POLAR = PolarCoord2(i * _DIV, _w, _h);
            Add_VERT(_POLAR.x + _x, _POLAR.y + _y, _col);
        }
        Add_VERT(_START.x + _x, _START.y + _y, _col);
        GL.End();
    }
    public static void Draw_ELLIPSE_DOTTED(int _segments, float _x, float _y, float _w, float _h, Color _col)
    {
        float _DIV = (Mathf.PI * 2) / _segments;
        for (int i = 0; i < _segments; i++)
        {
            Vector2 _POLAR = PolarCoord2(i * _DIV, _w, _h);
            DRAW_POINT(_x + _POLAR.x, _y + _POLAR.y, _col);
        }
    }

    public static void Draw_ELLIPSE_FILL(int _segments, float _x, float _y, float _w, float _h, Color _col)
    {

        float _DIV = (Mathf.PI * 2) / _segments;


        for (int i = 0; i < _segments; i++)
        {
            Vector2 _V1 = PolarCoord2(i * _DIV, _w, _h);
            Vector2 _V2 = PolarCoord2((i + 1) * _DIV, _w, _h);
            GL.Begin(GL.TRIANGLES);
            Add_VERT(_V1.x + _x, _V1.y + _y, _col);
            Add_VERT(_V2.x + _x, _V2.y + _y, _col);
            Add_VERT(_x, _y, _col);
            GL.End();
        }
    }
    public static void Draw_CIRCLE_LINE(int _segments, float _x, float _y, float _radius, Color _col)
    {


        GL.Begin(GL.LINE_STRIP);
        float _DIV = (Mathf.PI * 2) / _segments;
        Vector2 _START = PolarCoord_1to1(0, _radius);

        Add_VERT(_START.x + _x, _START.y + _y, _col);
        for (int i = 1; i < _segments; i++)
        {
            Vector2 _POLAR = PolarCoord_1to1(i * _DIV, _radius);
            Add_VERT(_POLAR.x + _x, _POLAR.y + _y, _col);
        }
        Add_VERT(_START.x + _x, _START.y + _y, _col);
        GL.End();
    }
    public static void Draw_CIRCLE_DOTTED(int _segments, float _x, float _y, float _radius, Color _col)
    {
        float _DIV = (Mathf.PI * 2) / _segments;
        for (int i = 0; i < _segments; i++)
        {
            Vector2 _POLAR = PolarCoord_1to1(i * _DIV, _radius);
            DRAW_POINT(_x + _POLAR.x, _y + _POLAR.y, _col);
        }
    }

    public static void Draw_CIRCLE_FILL(int _segments, float _x, float _y, float _radius, Color _col)
    {

        float _DIV = (Mathf.PI * 2) / _segments;

        for (int i = 0; i < _segments; i++)
        {
            Vector2 _V1 = PolarCoord_1to1(i * _DIV, _radius);
            Vector2 _V2 = PolarCoord_1to1((i + 1) * _DIV, _radius);
            GL.Begin(GL.TRIANGLES);
            Add_VERT(_V1.x + _x, _V1.y + _y, _col);
            Add_VERT(_V2.x + _x, _V2.y + _y, _col);
            Add_VERT(_x, _y, _col);
            GL.End();
        }
    }


    public static void Draw_ARC(int _segments, float _x, float _y, float _start, float _end, float _radius_start, float _radius_end, Color _col, float _rotation = 0)
    {
        float _THETA_START = _start * PI2;
        float _THETA_END = _end * PI2;
        float _THETA_RANGE = _THETA_END - _THETA_START;
        float _THETA_DIV = _THETA_RANGE / (_segments - 1);

        GL.PushMatrix();
        TransformMatrix(_x, _y, _rotation);
        GL.Begin(GL.LINE_STRIP);

        // BOTTOM arc
        Vector2 _START_VEC = PolarCoord(_THETA_START, _radius_start);
        Add_VERT_1to1(_START_VEC.x, _START_VEC.y, _col);
        for (int i = 1; i < _segments; i++)
        {
            Vector2 _V = PolarCoord(_THETA_START + (i * _THETA_DIV), _radius_start);
            Add_VERT_1to1(_V.x, _V.y, _col);
        }
        // TOP arc
        for (int i = 0; i < _segments; i++)
        {
            Vector2 _V = PolarCoord(_THETA_END - (i * _THETA_DIV), _radius_end);
            Add_VERT_1to1(_V.x, _V.y, _col);
        }

        // end cap
        Add_VERT_1to1(_START_VEC.x, _START_VEC.y, _col);
        GL.End();

        GL.PopMatrix();
    }

    public static void Draw_ARC_FILL(int _segments, float _x, float _y, float _start, float _end, float _radius_start, float _radius_end, Color _col, float _rotation = 0)
    {
        float _THETA_START = _start * PI2;
        float _THETA_END = _end * PI2;
        float _THETA_RANGE = _THETA_END - _THETA_START;
        float _THETA_DIV = _THETA_RANGE / (_segments - 1);

        int _TOTAL_SEGMENTS = (_segments * 2) - 1;

        Vector2[] _VECS = new Vector2[_TOTAL_SEGMENTS + 1];

        // create vectors
        for (int i = 0; i < _segments; i++)
        {
            Vector2 _BTM = PolarCoord(_THETA_START + (i * _THETA_DIV), _radius_start);
            Vector2 _TOP = PolarCoord(_THETA_END - (i * _THETA_DIV), _radius_end);

            _VECS[i] = _BTM;
            _VECS[i + _segments] = _TOP;


        }
        GL.PushMatrix();
        TransformMatrix(_x, _y, _rotation);
        for (int i = 0; i < _TOTAL_SEGMENTS; i++)
        {
            Vector2 _VA, _VB, _VC;

            _VA = _VECS[_TOTAL_SEGMENTS - i];
            _VB = _VECS[i];
            _VC = _VECS[i + 1];

            GL.Begin(GL.TRIANGLES);
            Add_VERT_1to1(_VA.x, _VA.y, _col);
            Add_VERT_1to1(_VB.x, _VB.y, _col);
            Add_VERT_1to1(_VC.x, _VC.y, _col);
            GL.End();
        }
        GL.PopMatrix();
    }

    public static void Draw_CROSS(float _x, float _y, float _thickness, float _size, Color _col, float _rotation = 0)
    {
        GL.PushMatrix();
        _size *= 0.5f;
        _thickness *= 0.5f;

        TransformMatrix(_x, _y, _rotation);
        GL.Begin(GL.QUADS);
        // X rect
        Add_VERT_1to1(-_size, -_thickness, _col);
        Add_VERT_1to1(_size, -_thickness, _col);
        Add_VERT_1to1(_size, _thickness, _col);
        Add_VERT_1to1(-_size, _thickness, _col);

        // Y rect
        Add_VERT_1to1(-_thickness, -_size, _col);
        Add_VERT_1to1(_thickness, -_size, _col);
        Add_VERT_1to1(_thickness, _size, _col);
        Add_VERT_1to1(-_thickness, _size, _col);

        GL.End();
        GL.PopMatrix();
    }
    public static void Draw_CHEVRON(float _x, float _y, float _thickness, float _size, Color _col, float _rotation = 0)
    {
        GL.PushMatrix();
        _size *= 0.5f;
        _thickness *= 0.5f;

        TransformMatrix(_x, _y, _rotation);
        GL.Begin(GL.QUADS);

        // LEFT arm
        Add_VERT_1to1(0f, 0f, _col);
        Add_VERT_1to1(-_size, _size, _col);
        Add_VERT_1to1(-(_size - _thickness * 0.5f), _size + (_thickness * 0.5f), _col);
        Add_VERT_1to1(0f, _thickness, _col);

        // RIGHT arm
        Add_VERT_1to1(0f, 0f, _col);
        Add_VERT_1to1(_size, _size, _col);
        Add_VERT_1to1((_size - _thickness * 0.5f), _size + (_thickness * 0.5f), _col);
        Add_VERT_1to1(0f, _thickness, _col);

        GL.End();
        GL.PopMatrix();
    }

    public static void Draw_CHEVRON_FRAME(float _x, float _y, float _w, float _h, float _thickness, float _size, Color _col)
    {
        GL.PushMatrix();
        float _rot = -0.125f;

        // TOP LEFT
        Draw_CHEVRON(_x, _y, _thickness, _size, _col, _rot);

        // TOP RIGHT
        _rot += 0.25f;
        Draw_CHEVRON(_x + _w, _y, _thickness, _size, _col, _rot);

        // BTM RIGHT
        _rot += 0.25f;
        Draw_CHEVRON(_x + _w, _y + _h, _thickness, _size, _col, _rot);

        // BTM LEFT
        _rot += 0.25f;
        Draw_CHEVRON(_x, _y + _h, _thickness, _size, _col, _rot);
        GL.PopMatrix();
    }

    public static void Draw_AXIS(float _x, float _y, float _size, float _width_MAJOR, float _width_MINOR, int _divisions, int _subDiv, Color _col_MAJOR, Color _col_MINOR, float _rotation = 0, float _offset = 0, bool _includeStem = false, float _stemThickness = 0.01f)
    {
        GL.PushMatrix();
        float _DIV = _size / _divisions;

        TransformMatrix(_x, _y, _rotation);

        if (_includeStem)
        {
            Draw_RECT_FILL(0, 0, _size, _stemThickness, _col_MAJOR);
        }

        // draw bookends first
        Draw_LINE(_x, _y, _x + _width_MAJOR, _y, _col_MAJOR, _col_MAJOR);
        Draw_LINE(_x, _y + _size, _x + _width_MAJOR, _y + _size, _col_MAJOR, _col_MAJOR);

        for (int i = 0; i < _divisions; i++)
        {
            float _DIST = Mathf.Clamp((i * _DIV + _offset) % _size, _y, _y + _size);
            if (i % _subDiv == 0)
            {
                //MAJOR MARK
                Draw_LINE(0, _DIST, _width_MAJOR, _DIST, _col_MAJOR, _col_MAJOR);
            }
            else
            {
                //    // MINOR MARK
                Draw_LINE(0, _DIST, _width_MINOR, _DIST, _col_MINOR, _col_MAJOR);
            }
        }
        GL.PopMatrix();
    }
    public static void Draw_AXIS(float _startX, float _startY, float _endX, float _endY, float _width_MAJOR, float _width_MINOR, int _divisions, int _subDiv, Color _col_MAJOR, Color _col_MINOR, float _offset = 0, bool _includeStem = false, float _stemThickness = 0.01f)
    {
        Vector2 _VEC_START = new Vector2(_startX, _startY);
        Vector2 _VEC_END = new Vector2(_endX, _endY);
        float _SIZE = Vector2.Distance(_VEC_START, _VEC_END);
        float _ANGLE = Angle(_VEC_END - _VEC_START) / -360;

        Draw_AXIS(_startX, _startY, _SIZE, _width_MAJOR, _width_MINOR, _divisions, _subDiv, _col_MAJOR, _col_MINOR, _ANGLE, _offset, _includeStem, _stemThickness);
    }

    public static void Draw_GRID_LINE(float _x, float _y, float _w, float _h, int _divsX, int _divsY, Color _col, float _offset_x = 0f, float _offset_y = 0f)
    {
        float _DIV_X = _w / _divsX;
        float _DIV_Y = _h / _divsY;

        float _LEFT = _x;
        float _RIGHT = _x + _w;
        float _BTM = _y;
        float _TOP = _y + _h;

        float _OFFSET_X = (_offset_x * _w) % _DIV_X;
        float _OFFSET_Y = (_offset_y * _h) % _DIV_Y;

        // draw frame first
        Draw_RECT(_x, _y, _w, _h, _col);

        for (int x = 0; x < _divsX; x++)
        {
            GL.Begin(GL.LINES);
            Add_VERT(Mathf.Clamp(_LEFT + (x * _DIV_X) + _OFFSET_X, _LEFT, _RIGHT), _BTM, _col);
            Add_VERT(Mathf.Clamp(_LEFT + (x * _DIV_X) + _OFFSET_X, _LEFT, _RIGHT), _TOP, _col);
            GL.End();
            for (int y = 0; y < _divsY; y++)
            {
                GL.Begin(GL.LINES);
                Add_VERT(_LEFT, Mathf.Clamp(_BTM + (y * _DIV_Y) + _OFFSET_Y, _BTM, _TOP), _col);
                Add_VERT(_RIGHT, Mathf.Clamp(_BTM + (y * _DIV_Y) + _OFFSET_Y, _BTM, _TOP), _col);
                GL.End();
            }
        }
    }
    public static void Draw_GRID_BOX(float _x, float _y, float _w, float _h, int _divsX, int _divsY, Color _col, float _boxRatio = 0.5f)
    {
        float _DIV_X = _w / _divsX;
        float _DIV_Y = _h / _divsY;
        float _BOX_X = _DIV_X * _boxRatio;
        float _BOX_Y = _DIV_Y * _boxRatio;

        for (int x = 0; x <= _divsX; x++)
        {
            for (int y = 0; y <= _divsY; y++)
            {
                Draw_RECT(_x + (x * _DIV_X), _y + (y * _DIV_Y), _BOX_X, _BOX_Y, _col);
            }
        }
    }
    public static void Draw_GRID_BOX_FILL(float _x, float _y, float _w, float _h, int _divsX, int _divsY, Color _col, float _boxRatio = 0.5f)
    {
        float _DIV_X = _w / _divsX;
        float _DIV_Y = _h / _divsY;
        float _BOX_X = _DIV_X * _boxRatio;
        float _BOX_Y = _DIV_Y * _boxRatio;

        for (int x = 0; x <= _divsX; x++)
        {
            for (int y = 0; y <= _divsY; y++)
            {
                Draw_RECT_FILL(_x + (x * _DIV_X), _y + (y * _DIV_Y), _BOX_X, _BOX_Y, _col);
            }
        }
    }
    public static void Draw_ZOOM_GRID(float _x, float _y, float _w, float _h, Color _col, int _divsX = 10, int _divsY = 10, float _originX = 0.5f, float _originY = 0.5f, float _maxLength = 0.01f)
    {
        float _DIV_X = _w / _divsX;
        float _DIV_Y = _h / _divsY;
        for (int x = 0; x <= _divsX; x++)
        {
            float _CUR_X = _x + (x * _DIV_X);
            float _DIST_X = (_originX - _CUR_X) * _maxLength;
            for (int y = 0; y <= _divsY; y++)
            {
                float _CUR_Y = _y + (y * _DIV_Y);
                float _DIST_Y = (_originY - _CUR_Y) * _maxLength;

                Draw_LINE(_CUR_X, _CUR_Y, _CUR_X + _DIST_X, _CUR_Y + _DIST_Y, _col);
            }
        }
    }
    public static void Draw_ZOOM_GRID(float _x, float _y, float _w, float _h, Color _col_MIN, Color _color_MAX, int _divsX = 10, int _divsY = 10, float _originX = 0.5f, float _originY = 0.5f, float _maxLength = 0.01f)
    {
        float _DIV_X = _w / _divsX;
        float _DIV_Y = _h / _divsY;
        Vector2 _VEC_ORIGIN = new Vector2(_originX, _originY);
        for (int x = 0; x <= _divsX; x++)
        {
            float _CUR_X = _x + (x * _DIV_X);
            float _DIST_X = (_originX - _CUR_X) * _maxLength;
            for (int y = 0; y <= _divsY; y++)
            {
                float _CUR_Y = _y + (y * _DIV_Y);
                float _DIST_Y = (_originY - _CUR_Y) * _maxLength;
                float _DIST = Vector2.Distance(_VEC_ORIGIN, new Vector2(_CUR_X, _CUR_Y)) / _w;
                Color _COL = Color.Lerp(_col_MIN, _color_MAX, _DIST);

                Draw_LINE(_CUR_X, _CUR_Y, _CUR_X + _DIST_X, _CUR_Y + _DIST_Y, _COL);
            }
        }
    }
    public static void Draw_GRID_DOT(float _x, float _y, float _w, float _h, int _divsX, int _divsY, Color _col)
    {
        float _DIV_X = _w / _divsX;
        float _DIV_Y = _h / _divsY;

        for (int x = 0; x <= _divsX; x++)
        {
            for (int y = 0; y <= _divsY; y++)
            {
                DRAW_POINT(_x + (x * _DIV_X), _y + (y * _DIV_Y), _col);
            }
        }
    }
    public static void Draw_GRID_NGON(float _x, float _y, float _w, float _h, int _divsX, int _divsY, int _sides, float _ngonSize, Color _col)
    {
        float _DIV_X = _w / _divsX;
        float _DIV_Y = _h / _divsY;

        for (int x = 0; x <= _divsX; x++)
        {
            for (int y = 0; y <= _divsY; y++)
            {
                Draw_NGON_FILL(_sides, _x + (x * _DIV_X), _y + (y * _DIV_Y), _ngonSize, _col);
            }
        }
    }

    public static void Draw_MATRIX_RECT(float _x, float _y, float _w, float _h, int _cellsX, int _cellsY, Color _col, BitArray _cells, float _rotation = 1)
    {
        //GL.PushMatrix();
        //GL.LoadPixelMatrix();
        //TransformMatrix(_x, _y, _rotation);

        float _DIV_X = _w / _cellsX;

        float _DIV_Y = _h / _cellsY;
        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells.Get(i))
            {
                Draw_RECT_FILL(
                    _x + (i % _cellsX) * _DIV_X,
                    _y + Mathf.FloorToInt(i / _cellsX) * _DIV_Y,
                    _DIV_X,
                    _DIV_Y, _col
                );
            }
        }

        //GL.PopMatrix();
    }

    public static void Draw_MATRIX_NGON(float _x, float _y, float _w, float _h, int _sides, float _ngonScaleFactor, int _cellsX, int _cellsY, Color _col, BitArray _cells, float _rotation = 0)
    {
        GL.PushMatrix();
        TransformMatrix(_x, _y, _rotation);

        float _DIV_X = _w / _cellsX;
        float _DIV_Y = _h / _cellsY;
        float _NGON_SIZE = _DIV_X * _ngonScaleFactor;

        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells.Get(i))
            {
                Draw_NGON_FILL(
                    _sides,
                    (i % _cellsX) * _DIV_X,
                    Mathf.FloorToInt(i / _cellsX) * _DIV_Y,
                    _NGON_SIZE,
                    _col);
            }
        }

        GL.PopMatrix();
    }



    public static void Draw_ARC_CELLS(float _x, float _y, float _radius_START, float _thickness, BitArray _cells, Color _col, float _angle_START = 0, float _angle_END = 1, float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO, int _segmentSides = HUD.DEFAULT_ARC_SIDES, float _rotation = 0)
    {
        int _TOTAL_CELLS = _cells.Length;
        float _ANGLE_RANGE = _angle_END - _angle_START;
        float _ANGLE_DRAW_AREA = _ANGLE_RANGE * _gutterRatio;
        float _DIV_ANGLE = _ANGLE_DRAW_AREA / _TOTAL_CELLS;
        float _GUTTER_ANGLE = (_ANGLE_RANGE - _ANGLE_DRAW_AREA) / (_TOTAL_CELLS - 1);

        for (int i = 0; i < _TOTAL_CELLS; i++)
        {
            if (_cells.Get(i))
            {
                float _START_ANGLE = _angle_START + ((i * _DIV_ANGLE) + (i * _GUTTER_ANGLE));
                GL_DRAW.Draw_ARC_FILL(_segmentSides, _x, _y, _START_ANGLE, _START_ANGLE + _DIV_ANGLE, _radius_START, _radius_START + _thickness, _col, _rotation);
            }
        }
    }
    public static void Draw_ARC_CELLS(float _x, float _y, float _radius_START, float _thickness, Color _col, float _angle_START = 0, float _angle_END = 1, float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO, int _segmentSides = HUD.DEFAULT_ARC_SIDES, float _rotation = 0, params int[] _cells)
    {
        BitArray _CELLS = new BitArray(_cells.Length);
        for (int i = 0; i < _cells.Length; i++)
        {
            _CELLS.Set(i, (_cells[i] == 1));
        }
        Draw_ARC_CELLS(_x, _y, _radius_START, _thickness, _CELLS, _col, _angle_START, _angle_END, _gutterRatio, _segmentSides, _rotation);
    }
    public static void Draw_RECT_CELLS(float _x, float _y, float _w, float _h, BitArray _cells, Color _col, float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO, float _rotation = 0)
    {
        int _TOTAL_CELLS = _cells.Length;
        float _ACTIVE_AREA = _w * _gutterRatio;
        float _CELL_SIZE = _ACTIVE_AREA / _TOTAL_CELLS;
        float _GUTTER_SIZE = (_w - _ACTIVE_AREA) / (_TOTAL_CELLS - 1);

        for (int i = 0; i < _TOTAL_CELLS; i++)
        {
            if (_cells.Get(i))
            {
                GL_DRAW.Draw_RECT_FILL(_x + (i * _CELL_SIZE) + (i * _GUTTER_SIZE), _y, _CELL_SIZE, _h, _col);
            }
        }
    }
    public static void Draw_RECT_CELLS(float _x, float _y, float _w, float _h, Color _col, float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO, float _rotation = 0, params int[] _cells)
    {
        BitArray _CELLS = new BitArray(_cells.Length);
        for (int i = 0; i < _cells.Length; i++)
        {
            _CELLS.Set(i, (_cells[i] == 1));
        }
        Draw_RECT_CELLS(_x, _y, _w, _h, _CELLS, _col, _gutterRatio, _rotation);
    }
    public static void Draw_MATRIX_RADIAL(float _x, float _y, float _radius_START, float _radius_END, int _cells_angle, int _cells_radius, Color _col, BitArray _cells, int _sides = HUD.DEFAULT_ARC_SIDES, float _angle_START = 0, float _angle_END = 1, float _gutterRatio_ANGLE = HUD.DEFAULT_GUTTER_RATIO, float _gutterRatio_RADIUS = HUD.DEFAULT_GUTTER_RATIO, float _rotation = 0)
    {
        float _RADIUS_RANGE = _radius_END - _radius_START;
        float _RADIUS_DRAW_AREA = _RADIUS_RANGE * _gutterRatio_RADIUS;
        float _GUTTER_RADIUS = (_RADIUS_RANGE - _RADIUS_DRAW_AREA) / (_cells_radius - 1);
        float _DIV_RADIUS = _RADIUS_DRAW_AREA / _cells_radius;
        for (int i = 0; i < _cells_radius; i++)
        {
            BitArray _RING_CELLS = new BitArray(_cells_angle);
            int _startIndex = i * _cells_angle;
            for (int cellIndex = 0; cellIndex < _cells_angle; cellIndex++)
            {
                _RING_CELLS.Set(cellIndex, _cells[_startIndex + cellIndex]);
            }
            float _RAD_START = (i * _DIV_RADIUS) + (i * _GUTTER_RADIUS);
            Draw_ARC_CELLS(_x, _y, _RAD_START, _DIV_RADIUS, _RING_CELLS, _col, _angle_START, _angle_END, _gutterRatio_ANGLE, _sides, _rotation);
        }

    }
    public static void Draw_MATRIX_RADIAL(float _x, float _y, float _radius_START, float _radius_END, int _cells_angle, int _cells_radius, Color _col, float _angle_START = 0, float _angle_END = 1, float _rotation = 0, int _sides = HUD.DEFAULT_ARC_SIDES, float _gutterRatio = HUD.DEFAULT_GUTTER_RATIO, params int[] _cells)
    {
        BitArray _CELLS = new BitArray(_cells.Length);
        for (int i = 0; i < _cells.Length; i++)
        {
            _CELLS.Set(i, (_cells[i] == 1));
        }

        //Draw_MATRIX_RADIAL(_x, _y, _radius_START, _radius_END, _cells_angle, _cells_radius, _gutterRatio, _col, _CELLS, _sides, _angle_START, _angle_END, _rotation);
    }

}
