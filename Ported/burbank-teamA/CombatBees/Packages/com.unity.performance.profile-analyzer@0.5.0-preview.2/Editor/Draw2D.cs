using UnityEngine;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class Draw2D
    {
        public enum Origin
        {
            TopLeft,
            BottomLeft
        };

        Origin m_Origin = Origin.TopLeft;
        GUIStyle m_GLStyle;
        string m_ShaderName;
        Material m_Material;
        Rect m_Rect;
        Vector4 m_ClipRect;
        bool m_ClipRectEnabled = false;

        public Draw2D(string shaderName)
        {
            m_ShaderName = shaderName;
            CheckAndSetupMaterial();
        }

        public bool CheckAndSetupMaterial()
        {
            if (m_Material == null)
            {
                m_Material = new Material(Shader.Find(m_ShaderName));
                m_Material.EnableKeyword("UNITY_UI_CLIP_RECT");
            }

            if (m_Material == null)
                return false;

            return true;
        }

        public bool IsMaterialValid()
        {
            if (m_Material == null)
                return false;

            return true;
        }

        public void OnGUI()
        {
            if (m_GLStyle == null)
            {
                m_GLStyle = new GUIStyle(GUI.skin.box);
                m_GLStyle.padding = new RectOffset(0, 0, 0, 0);
                m_GLStyle.margin = new RectOffset(0, 0, 0, 0);
            }
        }

        public void SetClipRect(Rect clipRect)
        {
            m_ClipRect = new Vector4(clipRect.x, clipRect.y, clipRect.x + clipRect.width, clipRect.y + clipRect.height);
            m_ClipRectEnabled = true;

            CheckAndSetupMaterial();
            m_Material.SetFloat("_UseClipRect", m_ClipRectEnabled ? 1f : 0f);
            m_Material.SetVector("_ClipRect", m_ClipRect);
        }

        public void ClearClipRect()
        {
            m_ClipRectEnabled = false;

            CheckAndSetupMaterial();
            m_Material.SetFloat("_UseClipRect", m_ClipRectEnabled ? 1f : 0f);
            m_Material.SetVector("_ClipRect", m_ClipRect);
        }

        public bool DrawStart(Rect r, Origin origin = Origin.TopLeft)
        {
            if (Event.current.type != EventType.Repaint)
                return false;

            if (!CheckAndSetupMaterial())
                return false;

            CheckAndSetupMaterial();
            m_Material.SetPass(0);

            m_Rect = r;
            m_Origin = origin;
            return true;
        }

        public bool DrawStart(float w, float h, Origin origin = Origin.TopLeft, GUIStyle style = null)
        {
            Rect r = GUILayoutUtility.GetRect(w, h, style == null ? m_GLStyle : style, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            return DrawStart(r, origin);
        }

        public void DrawEnd()
        {
        }

        public void Translate(ref float x, ref float y)
        {
            // Translation done CPU side so we have world space coords in the shader for clipping.
            if (m_Origin == Origin.BottomLeft)
            {
                x = m_Rect.xMin + x;
                y = m_Rect.yMax - y;
            }
            else
            {
                x = m_Rect.xMin + x;
                y = m_Rect.yMin + y;
            }
        }

        public void DrawFilledBox(float x, float y, float w, float h, Color col)
        {
            float x2 = x + w;
            float y2 = y + h;

            Translate(ref x, ref y);
            Translate(ref x2, ref y2);

            if (m_Origin == Origin.BottomLeft)
            {
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(col);
                GL.Vertex3(x, y, 0);
                GL.Vertex3(x, y2, 0);
                GL.Vertex3(x2, y, 0);
                GL.Vertex3(x2, y2, 0);
                GL.End();
            }
            else
            {
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(col);
                GL.Vertex3(x, y, 0);
                GL.Vertex3(x2, y, 0);
                GL.Vertex3(x, y2, 0);
                GL.Vertex3(x2, y2, 0);
                GL.End();
            }
        }

        public void DrawFilledBox(float x, float y, float w, float h, float r, float g, float b)
        {
            DrawFilledBox(x, y, w, h, new Color(r, g, b));
        }

        public void DrawLine(float x, float y, float x2, float y2, Color col)
        {
            Translate(ref x, ref y);
            Translate(ref x2, ref y2);

            GL.Begin(GL.LINES);
            GL.Color(col);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(x2, y2, 0);
            GL.End();
        }

        public void DrawLine(float x, float y, float x2, float y2, float r, float g, float b)
        {
            DrawLine(x, y, x2, y2, new Color(r, g, b));
        }

        public void DrawBox(float x, float y, float w, float h, Color col)
        {
            float x2 = x + w;
            float y2 = y + h;

            Translate(ref x, ref y);
            Translate(ref x2, ref y2);

            GL.Begin(GL.LINE_STRIP);
            GL.Color(col);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(x2, y, 0);
            GL.Vertex3(x2, y2, 0);
            GL.Vertex3(x, y2, 0);
            GL.Vertex3(x, y, 0);
            GL.End();
        }

        public void DrawBox(float x, float y, float w, float h, float r, float g, float b)
        {
            DrawBox(x, y, w, h, new Color(r, g, b));
        }
    }
}