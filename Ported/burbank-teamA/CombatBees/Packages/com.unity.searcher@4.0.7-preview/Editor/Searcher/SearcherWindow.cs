using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Searcher
{
    [PublicAPI]
    public class SearcherWindow : EditorWindow
    {
        [PublicAPI]
        public struct Alignment
        {
            [PublicAPI]
            public enum Horizontal { Left = 0, Center, Right }
            [PublicAPI]
            public enum Vertical { Top = 0, Center, Bottom }

            public readonly Vertical vertical;
            public readonly Horizontal horizontal;

            public Alignment(Vertical v, Horizontal h)
            {
                vertical = v;
                horizontal = h;
            }
        }

        const string k_DatabaseDirectory = "/../Library/Searcher";

        static readonly float k_SearcherDefaultWidth = 300;
        static readonly float k_DetailsDefaultWidth = 200;
        static readonly float k_DefaultHeight = 300;
        static readonly Vector2 k_MinSize = new Vector2(300, 150);

        static Vector2 s_Size = Vector2.zero;
        static IEnumerable<SearcherItem> s_Items;
        static Searcher s_Searcher;
        static Func<SearcherItem, bool> s_ItemSelectedDelegate;

        Action<Searcher.AnalyticsEvent> m_AnalyticsDataDelegate;
        SearcherControl m_SearcherControl;
        Vector2 m_OriginalMousePos;
        Rect m_OriginalWindowPos;
        Rect m_NewWindowPos;
        bool m_IsMouseDownOnResizer;
        bool m_IsMouseDownOnTitle;
        Focusable m_FocusedBefore;

        static Vector2 Size
        {
            get
            {
                if (s_Size == Vector2.zero)
                {
                    s_Size = s_Searcher != null && s_Searcher.Adapter.HasDetailsPanel
                        ? new Vector2(k_SearcherDefaultWidth + k_DetailsDefaultWidth, k_DefaultHeight)
                        : new Vector2(k_SearcherDefaultWidth, k_DefaultHeight);
                }

                return s_Size;
            }
            set => s_Size = value;
        }

        public static void Show(
            EditorWindow host,
            IList<SearcherItem> items,
            string title,
            Func<SearcherItem, bool> itemSelectedDelegate,
            Vector2 displayPosition,
            Alignment align = default)
        {
            Show(host, items, title, Application.dataPath + k_DatabaseDirectory, itemSelectedDelegate, displayPosition, align);
        }

        public static void Show(
            EditorWindow host,
            IList<SearcherItem> items,
            ISearcherAdapter adapter,
            Func<SearcherItem, bool> itemSelectedDelegate,
            Vector2 displayPosition,
            Action<Searcher.AnalyticsEvent> analyticsDataDelegate,
            Alignment align = default)
        {
            Show(host, items, adapter, Application.dataPath + k_DatabaseDirectory, itemSelectedDelegate,
                displayPosition, analyticsDataDelegate, align);
        }

        public static void Show(
            EditorWindow host,
            IList<SearcherItem> items,
            string title,
            string directoryPath,
            Func<SearcherItem, bool> itemSelectedDelegate,
            Vector2 displayPosition,
            Alignment align = default)
        {
            s_Items = items;
            var databaseDir = directoryPath;
            var database = SearcherDatabase.Create(s_Items.ToList(), databaseDir);
            s_Searcher = new Searcher(database, title);

            Show(host, s_Searcher, itemSelectedDelegate, displayPosition, null, align);
        }

        public static void Show(
            EditorWindow host,
            IEnumerable<SearcherItem> items,
            ISearcherAdapter adapter,
            string directoryPath,
            Func<SearcherItem, bool> itemSelectedDelegate,
            Vector2 displayPosition,
            Action<Searcher.AnalyticsEvent> analyticsDataDelegate,
            Alignment align = default)
        {
            s_Items = items;
            var databaseDir = directoryPath;
            var database = SearcherDatabase.Create(s_Items.ToList(), databaseDir);
            s_Searcher = new Searcher(database, adapter);

            Show(host, s_Searcher, itemSelectedDelegate, displayPosition, analyticsDataDelegate, align);
        }

        public static void Show(
            EditorWindow host,
            Searcher searcher,
            Func<SearcherItem, bool> itemSelectedDelegate,
            Vector2 displayPosition,
            Action<Searcher.AnalyticsEvent> analyticsDataDelegate,
            Alignment align = default)
        {
            s_Searcher = searcher;
            s_ItemSelectedDelegate = itemSelectedDelegate;

            var window = CreateInstance<SearcherWindow>();
            window.m_AnalyticsDataDelegate = analyticsDataDelegate;
            var position = GetPosition(host, displayPosition);
            window.position = new Rect(GetPositionWithAlignment(position + host.position.position, Size, align), Size);
            window.ShowPopup();
            window.Focus();
        }

        static Vector2 GetPositionWithAlignment(Vector2 pos, Vector2 size, Alignment align)
        {
            var x = pos.x;
            var y = pos.y;

            switch (align.horizontal)
            {
                case Alignment.Horizontal.Center:
                    x -= size.x / 2;
                    break;

                case Alignment.Horizontal.Right:
                    x -= size.x;
                    break;
            }

            switch (align.vertical)
            {
                case Alignment.Vertical.Center:
                    y -= size.y / 2;
                    break;

                case Alignment.Vertical.Bottom:
                    y -= size.y;
                    break;
            }

            return new Vector2(x, y);
        }

        static Vector2 GetPosition(EditorWindow host, Vector2 displayPosition)
        {
            var x = displayPosition.x;
            var y = displayPosition.y;

            // Searcher overlaps with the right boundary.
            if (x + Size.x >= host.position.size.x)
                x -= Size.x;

            // The displayPosition should be in window world space but the
            // EditorWindow.position is actually the rootVisualElement
            // rectangle, not including the tabs area. So we need to do a
            // small correction here.
            y -= host.rootVisualElement.resolvedStyle.top;

            // Searcher overlaps with the bottom boundary.
            if (y + Size.y >= host.position.size.y)
                y -= Size.y;

            return new Vector2(x, y);
        }

        void OnEnable()
        {
            m_SearcherControl = new SearcherControl();
            m_SearcherControl.Setup(s_Searcher, SelectionCallback, OnAnalyticsDataCallback);

            m_SearcherControl.TitleLabel.RegisterCallback<MouseDownEvent>(OnTitleMouseDown);
            m_SearcherControl.TitleLabel.RegisterCallback<MouseUpEvent>(OnTitleMouseUp);

            m_SearcherControl.Resizer.RegisterCallback<MouseDownEvent>(OnResizerMouseDown);
            m_SearcherControl.Resizer.RegisterCallback<MouseUpEvent>(OnResizerMouseUp);

            var root = rootVisualElement;
            root.style.flexGrow = 1;
            root.Add(m_SearcherControl);
        }

        void OnDisable()
        {
            m_SearcherControl.TitleLabel.UnregisterCallback<MouseDownEvent>(OnTitleMouseDown);
            m_SearcherControl.TitleLabel.UnregisterCallback<MouseUpEvent>(OnTitleMouseUp);

            m_SearcherControl.Resizer.UnregisterCallback<MouseDownEvent>(OnResizerMouseDown);
            m_SearcherControl.Resizer.UnregisterCallback<MouseUpEvent>(OnResizerMouseUp);
        }

        void OnTitleMouseDown(MouseDownEvent evt)
        {
            if (evt.button != (int)MouseButton.LeftMouse)
                return;

            m_IsMouseDownOnTitle = true;

            m_NewWindowPos = position;
            m_OriginalWindowPos = position;
            m_OriginalMousePos = evt.mousePosition;

            m_FocusedBefore = rootVisualElement.panel.focusController.focusedElement;

            m_SearcherControl.TitleLabel.RegisterCallback<MouseMoveEvent>(OnTitleMouseMove);
            m_SearcherControl.TitleLabel.RegisterCallback<KeyDownEvent>(OnSearcherKeyDown);
            m_SearcherControl.TitleLabel.CaptureMouse();
        }

        void OnTitleMouseUp(MouseUpEvent evt)
        {
            if (evt.button != (int)MouseButton.LeftMouse)
                return;

            if (!m_SearcherControl.TitleLabel.HasMouseCapture())
                return;

            FinishMove();
        }

        void FinishMove()
        {
            m_SearcherControl.TitleLabel.UnregisterCallback<MouseMoveEvent>(OnTitleMouseMove);
            m_SearcherControl.TitleLabel.UnregisterCallback<KeyDownEvent>(OnSearcherKeyDown);
            m_SearcherControl.TitleLabel.ReleaseMouse();
            m_FocusedBefore?.Focus();
            m_IsMouseDownOnTitle = false;
        }

        void OnTitleMouseMove(MouseMoveEvent evt)
        {
            var delta = evt.mousePosition - m_OriginalMousePos;

            // TODO Temporary fix for Visual Scripting 1st drop. Find why position.position is 0,0 on MacOs in MouseMoveEvent
            // Bug occurs with Unity 2019.2.0a13
#if UNITY_EDITOR_OSX
            m_NewWindowPos = new Rect(m_NewWindowPos.position + delta, position.size);
#else
            m_NewWindowPos = new Rect(position.position + delta, position.size);
#endif
            Repaint();
        }

        void OnResizerMouseDown(MouseDownEvent evt)
        {
            if (evt.button != (int)MouseButton.LeftMouse)
                return;

            m_IsMouseDownOnResizer = true;

            m_NewWindowPos = position;
            m_OriginalWindowPos = position;
            m_OriginalMousePos = evt.mousePosition;

            m_FocusedBefore = rootVisualElement.panel.focusController.focusedElement;

            m_SearcherControl.Resizer.RegisterCallback<MouseMoveEvent>(OnResizerMouseMove);
            m_SearcherControl.Resizer.RegisterCallback<KeyDownEvent>(OnSearcherKeyDown);
            m_SearcherControl.Resizer.CaptureMouse();
        }

        void OnResizerMouseUp(MouseUpEvent evt)
        {
            if (evt.button != (int)MouseButton.LeftMouse)
                return;

            if (!m_SearcherControl.Resizer.HasMouseCapture())
                return;

            FinishResize();
        }

        void FinishResize()
        {
            m_SearcherControl.Resizer.UnregisterCallback<MouseMoveEvent>(OnResizerMouseMove);
            m_SearcherControl.Resizer.UnregisterCallback<KeyDownEvent>(OnSearcherKeyDown);
            m_SearcherControl.Resizer.ReleaseMouse();
            m_FocusedBefore?.Focus();
            m_IsMouseDownOnResizer = false;
        }

        void OnResizerMouseMove(MouseMoveEvent evt)
        {
            var delta = evt.mousePosition - m_OriginalMousePos;
            Size = m_OriginalWindowPos.size + delta;
            Size = new Vector2(Math.Max(k_MinSize.x, Size.x), Math.Max(k_MinSize.y, Size.y));

            // TODO Temporary fix for Visual Scripting 1st drop. Find why position.position is 0,0 on MacOs in MouseMoveEvent
            // Bug occurs with Unity 2019.2.0a13
#if UNITY_EDITOR_OSX
            m_NewWindowPos = new Rect(m_NewWindowPos.position, Size);
#else
            m_NewWindowPos = new Rect(position.position, Size);
#endif
            Repaint();
        }

        void OnSearcherKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape)
            {
                if (m_IsMouseDownOnTitle)
                {
                    FinishMove();
                    position = m_OriginalWindowPos;
                }
                else if (m_IsMouseDownOnResizer)
                {
                    FinishResize();
                    position = m_OriginalWindowPos;
                }
            }
        }

        void OnGUI()
        {
            if ((m_IsMouseDownOnTitle || m_IsMouseDownOnResizer) && Event.current.type == EventType.Layout)
                position = m_NewWindowPos;
        }

        void SelectionCallback(SearcherItem item)
        {
            if (s_ItemSelectedDelegate == null || s_ItemSelectedDelegate(item))
                Close();
        }

        void OnAnalyticsDataCallback(Searcher.AnalyticsEvent item)
        {
            m_AnalyticsDataDelegate?.Invoke(item);
        }

        void OnLostFocus()
        {
            if (m_IsMouseDownOnTitle)
            {
                FinishMove();
            }
            else if (m_IsMouseDownOnResizer)
            {
                FinishResize();
            }

            // TODO: HACK - ListView's scroll view steals focus using the scheduler.
            EditorApplication.update += HackDueToCloseOnLostFocusCrashing;
        }

        // See: https://fogbugz.unity3d.com/f/cases/1004504/
        void HackDueToCloseOnLostFocusCrashing()
        {
            // Notify user that the searcher action was cancelled.
            s_ItemSelectedDelegate?.Invoke(null);

            Close();

            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= HackDueToCloseOnLostFocusCrashing;
        }
    }
}
