using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class AutoComplete : IDisposable
    {
        public const string AutoCompleteContainerUssClass = "unity-autocomplete__container";

        const int k_CompletionListItemHeight = 18;
        const int k_CompletionListMaxHeight = 150;

        static readonly UITemplate k_Container = new UITemplate("Common/autocomplete");
        static readonly ProfilerMarker k_FetchCompletionResultsMarker = new ProfilerMarker($"{nameof(AutoComplete)}.FetchCompletionResults");

        readonly VisualElement m_Anchor;
        readonly IAutoCompleteBehavior m_Behavior;
        readonly TextField m_TextField;
        readonly VisualElement m_TextInput;
        readonly VisualElement m_CompletionContainer;
        readonly ListView m_CompletionListView;
        readonly List<string> m_CompletionItems = new List<string>();

        VisualElement m_Root;
        bool m_IgnoreNextValueChanged;

        /// <summary>
        /// Enable auto completion on the given <see cref="VisualElement"/> using the given behavior.
        /// The given <see cref="VisualElement"/> is expected to be or contain a <see cref="TextField"/>.
        /// </summary>
        /// <param name="element">The <see cref="VisualElement"/> to attach the completion box to. This is expected to be or contain a <see cref="TextField"/></param>
        /// <param name="behavior">The instance of <see cref="AutoComplete.IAutoCompleteBehavior"/> to use to drive the completion behavior</param>
        /// <returns>The <see cref="AutoComplete"/> instance created, typically used to disable the completion behavior when needed or to operate on it by code</returns>
        public AutoComplete(VisualElement element, IAutoCompleteBehavior behavior)
        {
            m_Anchor = element;
            m_TextField = element as TextField ?? element.Q<TextField>();
            if (m_TextField == null)
                throw new ArgumentNullException(nameof(element), $"{nameof(element)} needs to be or contain a {nameof(TextField)}");
            m_TextInput = m_TextField.Q(TextField.textInputUssName);
            m_Behavior = behavior;

            m_CompletionContainer = new VisualElement();
            m_CompletionContainer.AddToClassList(AutoCompleteContainerUssClass);
            m_CompletionContainer.Hide();
            m_CompletionContainer.style.position = Position.Absolute;
            m_CompletionListView = new ListView(m_CompletionItems, k_CompletionListItemHeight, () => new Label(), (l, i) => ((Label)l).text = m_CompletionItems[i])
            {
                style = { flexGrow = 1 }
            };

            m_CompletionContainer.Add(m_CompletionListView);

            if (element.panel == null)
                element.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            else
                OnAttachToPanel(null);

            m_TextInput.RegisterCallback<KeyDownEvent>(OnInputKeyDown);
            m_TextInput.RegisterCallback<KeyUpEvent>(OnInputKeyUp);
            m_TextField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            m_CompletionContainer.RegisterCallback<FocusOutEvent>(OnFocusOut);
            m_TextField.RegisterValueChangedCallback(OnValueChanged);
            m_CompletionListView.RegisterCallback<PointerUpEvent>(OnListViewPointerUp);
        }

        int CaretPosition => Math.Max(m_TextField.cursorIndex, m_TextField.selectIndex);
        bool IsCompletionContainerVisible => m_CompletionContainer.style.display == DisplayStyle.Flex;

        void OnAttachToPanel(AttachToPanelEvent evt)
        {
            var root = m_Anchor;
            while (root.parent != m_Anchor.panel.visualTree)
            {
                root = root.parent;
                if (root == null)
                    throw new InvalidOperationException("Cannot find the root visual element from " + m_Anchor);
            }

            m_Root = root;
            k_Container.AddStyles(m_Root);
            m_Root.Add(m_CompletionContainer);
            m_Root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        void OnGeometryChanged(GeometryChangedEvent evt) => AdjustCompletionContainerSizeAndPosition();

        void OnInputKeyDown(KeyDownEvent evt)
        {
            if (!IsCompletionContainerVisible)
            {
                if (evt.keyCode == KeyCode.Space && (evt.modifiers == EventModifiers.Command || evt.modifiers == EventModifiers.Control))
                    BeginCompletion();

                return;
            }

            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    CancelEvent(evt);
                    m_CompletionContainer.Hide();
                    break;
                case KeyCode.DownArrow:
                    CancelEvent(evt);
                    m_CompletionListView.selectedIndex = (m_CompletionListView.selectedIndex + 1) % m_CompletionItems.Count;
                    m_CompletionListView.ScrollToItem(m_CompletionListView.selectedIndex);
                    break;
                case KeyCode.UpArrow:
                    CancelEvent(evt);
                    m_CompletionListView.selectedIndex = m_CompletionListView.selectedIndex - 1 < 0 ? m_CompletionItems.Count - 1 : m_CompletionListView.selectedIndex - 1;
                    m_CompletionListView.ScrollToItem(m_CompletionListView.selectedIndex);
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    CancelEvent(evt);
                    SubmitCompletion(m_CompletionListView.selectedIndex);
                    break;
            }
        }

        static void CancelEvent(EventBase evt)
        {
            evt.PreventDefault();
            evt.StopImmediatePropagation();
        }

        void OnInputKeyUp(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.LeftArrow || evt.keyCode == KeyCode.RightArrow)
                BeginCompletion();
        }

        void OnFocusOut(FocusOutEvent evt)
        {
            if (!IsCompletionContainerVisible
                || !(evt.relatedTarget is VisualElement ve)
                || !m_CompletionContainer.Contains(ve) && !m_TextField.Contains(ve))
            {
                m_CompletionContainer.Hide();
            }
        }

        void OnListViewPointerUp(PointerUpEvent evt) => SubmitCompletion(m_CompletionListView.selectedIndex);

        void OnValueChanged(ChangeEvent<string> evt)
        {
            if (m_IgnoreNextValueChanged)
            {
                m_IgnoreNextValueChanged = false;
                return;
            }

            BeginCompletion();
        }

        void BeginCompletion()
        {
            if (!m_Behavior.ShouldStartAutoCompletion(m_TextField.value, CaretPosition))
            {
                m_CompletionContainer.Hide();
                return;
            }

            using (k_FetchCompletionResultsMarker.Auto())
            {
                m_CompletionItems.Clear();
                m_CompletionItems.AddRange(m_Behavior.GetCompletionItems(m_TextField.value, CaretPosition));
            }

            if (m_CompletionItems.Count == 0 || m_CompletionItems.Count == 1 && m_CompletionItems[0] == m_Behavior.GetToken(m_TextField.value, CaretPosition))
            {
                m_CompletionContainer.Hide();
                return;
            }

            m_CompletionListView.Refresh();
            m_CompletionListView.selectedIndex = 0;
            AdjustCompletionContainerSizeAndPosition();
            m_CompletionContainer.BringToFront();
            m_CompletionContainer.Show();
        }

        void SubmitCompletion(int selectedIndex)
        {
            m_IgnoreNextValueChanged = true;
            var (newInput, newCaretPosition) = m_Behavior.OnCompletion(m_CompletionItems[selectedIndex], m_TextField.value, CaretPosition);
            m_TextField.value = newInput;
            m_TextField.schedule.Execute(() =>
            {
                m_TextField.Focus();
                m_TextInput.Focus();
                m_TextField.SelectRange(newCaretPosition, newCaretPosition);
            }).StartingIn(0);
            m_CompletionContainer.Hide();
        }

        void AdjustCompletionContainerSizeAndPosition()
        {
            var worldBound = m_Anchor.worldBound;
            m_CompletionContainer.style.top = worldBound.y;
            m_CompletionContainer.style.left = worldBound.x;
            m_CompletionContainer.style.width = worldBound.width;

            var maxHeightInCurrentContainer = m_Root.worldBound.height - worldBound.y - m_CompletionContainer.resolvedStyle.marginBottom;

            m_CompletionContainer.style.height = Math.Min(maxHeightInCurrentContainer, k_CompletionListMaxHeight);
        }

        // Useful when needing to close the completion container by code without clearing the textfield text
        internal void Clear() => m_CompletionContainer.Hide();

        public void Dispose()
        {
            m_Root.Remove(m_CompletionListView);
            k_Container.RemoveStyles(m_Root);

            m_TextInput.UnregisterCallback<KeyDownEvent>(OnInputKeyDown);
            m_TextInput.UnregisterCallback<KeyUpEvent>(OnInputKeyUp);
            m_TextField.UnregisterCallback<FocusOutEvent>(OnFocusOut);
            m_CompletionContainer.UnregisterCallback<FocusOutEvent>(OnFocusOut);
            m_TextField.UnregisterValueChangedCallback(OnValueChanged);
            m_CompletionListView.UnregisterCallback<PointerUpEvent>(OnListViewPointerUp);
            m_Root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        /// <summary>
        /// Interface driving the auto complete behavior. Typically implemented by a singleton.
        /// This allows you to determine when a input should open the completion box, which data to put in it and how the completed string will fit in the source textfield text once completed.
        /// </summary>
        public interface IAutoCompleteBehavior
        {
            /// <summary>
            /// Determines if the autocomplete box should be visible based on the current input string and with the current caret position.
            /// </summary>
            /// <param name="input">The full value of the underlying textfield text</param>
            /// <param name="caretPosition">The current position of the caret in the <paramref name="input"/></param>
            /// <returns>True to show the completion box</returns>
            bool ShouldStartAutoCompletion(string input, int caretPosition);

            /// <summary>
            /// Extracts the token (or substring) that is currently being completed.
            /// <example>If the <paramref name="input"/> is "hello world 42" and <paramref name="caretPosition"/> is 11 (just after "world") the token will most likely be "world"</example>
            /// </summary>
            /// <param name="input">The full value of the underlying textfield text</param>
            /// <param name="caretPosition">The current position of the caret in the <paramref name="input"/></param>
            /// <returns>The token extracted from the input</returns>
            string GetToken(string input, int caretPosition);

            /// <summary>
            /// Retrieve items to include in the completion box based on <paramref name="input"/> string and <paramref name="caretPosition"/>
            /// </summary>
            /// <param name="input">The full value of the underlying textfield text</param>
            /// <param name="caretPosition">The current position of the caret in the <paramref name="input"/></param>
            /// <returns>The collection of string that can complete the current token being completed</returns>
            IEnumerable<string> GetCompletionItems(string input, int caretPosition);

            /// <summary>
            /// Called when a completion is done. Based on the completed token, the full input and caret position returns the new full input string and new caret position.
            /// </summary>
            /// <param name="completedToken">the selected item from the completion items that will complete the current token being completed</param>
            /// <param name="input">The full value of the underlying textfield text</param>
            /// <param name="caretPosition">The current position of the caret in the <paramref name="input"/></param>
            /// <returns>The new full input and the caret position in this new input string</returns>
            (string newInput, int caretPosition) OnCompletion(string completedToken, string input, int caretPosition);
        }
    }

    static class AutoCompleteExtension
    {
        /// <summary>
        /// Enable auto completion on the given <see cref="VisualElement"/> using the given behavior.
        /// The given <see cref="VisualElement"/> is expected to be or contain a <see cref="TextField"/>.
        /// </summary>
        /// <param name="this">The <see cref="VisualElement"/> to attach the completion box to. This is expected to be or contain a <see cref="TextField"/></param>
        /// <param name="behavior">The instance of <see cref="AutoComplete.IAutoCompleteBehavior"/> to use to drive the completion behavior</param>
        /// <returns>The <see cref="AutoComplete"/> instance created, typically used to disable the completion behavior when needed or to operate on it by code</returns>
        public static AutoComplete EnableAutoComplete(this VisualElement @this, AutoComplete.IAutoCompleteBehavior behavior)
            => new AutoComplete(@this, behavior);
    }
}
