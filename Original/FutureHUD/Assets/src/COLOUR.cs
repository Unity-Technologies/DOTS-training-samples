using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace COLOUR
{
    public struct Palette
    {
        public int totalColours;
        public List<Color> colours;
        public Palette(params Color[] _colours)
        {
            colours = _colours.ToList();
            totalColours = colours.Count;
        }
        public Color Get(int _index)
        {
            return colours[_index];
        }
        public Color Get(int _index, float _alpha)
        {
            return COL.Set_alphaStrength(colours[_index], _alpha);
        }
        public Color Get(int _index, float _strength_H = 1f, float _strength_S = 1f, float _strength_V = 1f)
        {
            float h, s, v;
            Color.RGBToHSV(Get(_index), out h, out s, out v);
            return COL.HSV(h * _strength_H, s * _strength_S, v * _strength_V);

        }
        public Color RandomColour()
        {
            return colours[Mathf.FloorToInt(Random.Range(0, totalColours))];
        }
        public Color RandomColour(float _alpha)
        {
            return COL.Set_alphaStrength(colours[Mathf.FloorToInt(Random.Range(0, totalColours))], _alpha);
        }
        public Color Lerp(int _A, int _B, float _value)
        {
            return Color.Lerp(Get(_A), Get(_B), _value);
        }
        public void Draw_Swatches(float _x, float _y, float _w, float _h)
        {
            float _XDIV = _w / totalColours;
            for (int i = 0; i < totalColours; i++)
            {
                GL_DRAW.Draw_RECT_FILL(i * _XDIV, _y, _XDIV, _h, Get(i));
            }
        }
    }
    public class COL
    {
        public static List<Palette> palettes;
        public static void INIT_PALETTES(Color[] _palette)
        {
            palettes = new List<Palette>();
            palettes.Add(new Palette(_palette));
        }
        public static Color HSV(float _hue, float _saturation, float _value, float _alpha = 1)
        {
            Color _C = Color.HSVToRGB(_hue, _value, _value);
            _C.a = _alpha;
            return _C;
        }
        public static Color Set_alphaStrength(Color _col, float _strength)
        {
            return new Color(_col.r, _col.g, _col.b, _col.a * _strength);
        }
        public static Palette Get_Palette(int _index)
        {
            return palettes[_index % palettes.Count];
        }
        public static Palette Get_RandomPalette()
        {
            return palettes[Mathf.FloorToInt(Random.Range(0, palettes.Count))];
        }
        public static Color Get_Colour(int _paletteIndex, int _colourIndex)
        {
            Palette _P = palettes[_paletteIndex % palettes.Count];
            return _P.colours[_colourIndex % _P.colours.Count];
        }
    }
}
