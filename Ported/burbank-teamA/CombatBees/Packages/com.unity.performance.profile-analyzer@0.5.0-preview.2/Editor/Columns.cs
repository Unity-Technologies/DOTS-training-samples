using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    public class Columns
    {
        private int[] m_ColumnWidth = new int[4];

        public Columns(int a, int b, int c, int d)
        {
            SetColumnSizes(a, b, c, d);
        }

        public void SetColumnSizes(int a, int b, int c, int d)
        {
            m_ColumnWidth[0] = a;
            m_ColumnWidth[1] = b;
            m_ColumnWidth[2] = c;
            m_ColumnWidth[3] = d;
        }

        public int GetColumnWidth(int n)
        {
            if (n < 0 || n >= m_ColumnWidth.Length)
                return 0;
            
            return m_ColumnWidth[n];
        }

        public void Draw(int n, string col)
        {
            if (n < 0 || n >= m_ColumnWidth.Length || m_ColumnWidth[n] <= 0)
                EditorGUILayout.LabelField(col);
            
            EditorGUILayout.LabelField(col, GUILayout.Width(m_ColumnWidth[n]));
        }

        public void Draw(int n, float value)
        {
            Draw(n, string.Format("{0:f2}", value));
        }

        public void Draw2(string col1, string col2)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, col2);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw2(string label, float value)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, label);
            Draw(1, value);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw3(string col1, string col2, string col3)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, col2);
            Draw(2, col3);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw3(string col1, float value2, float value3)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, value2);
            Draw(2, value3);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw4(string col1, string col2, string col3, string col4)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, col2);
            Draw(2, col3);
            Draw(3, col4);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw4Diff(string col1, float left, float right)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, left);
            Draw(2, right);
            Draw(3, right - left);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw4(string col1, float value2, float value3, float value4)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, value2);
            Draw(2, value3);
            Draw(3, value4);
            EditorGUILayout.EndHorizontal();
        }


        // GUIContent versions
        public void Draw(int n, GUIContent col)
        {
            if (n < 0 || n >= m_ColumnWidth.Length || m_ColumnWidth[n] <= 0)
                EditorGUILayout.LabelField(col);

            EditorGUILayout.LabelField(col, GUILayout.Width(m_ColumnWidth[n]));
        }

        public void Draw2(GUIContent col1, GUIContent col2)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, col2);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw2(GUIContent label, float value)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, label);
            Draw(1, value);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw3(GUIContent col1, GUIContent col2, GUIContent col3)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, col2);
            Draw(2, col3);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw3(GUIContent col1, float value2, float value3)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, value2);
            Draw(2, value3);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw4(GUIContent col1, GUIContent col2, GUIContent col3, GUIContent col4)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, col2);
            Draw(2, col3);
            Draw(3, col4);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw4Diff(GUIContent col1, float left, float right)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, left);
            Draw(2, right);
            Draw(3, right - left);
            EditorGUILayout.EndHorizontal();
        }

        public void Draw4(GUIContent col1, float value2, float value3, float value4)
        {
            EditorGUILayout.BeginHorizontal();
            Draw(0, col1);
            Draw(1, value2);
            Draw(2, value3);
            Draw(3, value4);
            EditorGUILayout.EndHorizontal();
        }
    }
}