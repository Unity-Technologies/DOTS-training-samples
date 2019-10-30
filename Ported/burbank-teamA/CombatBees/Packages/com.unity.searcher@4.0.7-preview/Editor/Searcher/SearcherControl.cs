using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.Searcher
{
    class SearcherControl : VisualElement
    {
        // Window constants.
        const string k_WindowTitleLabel = "windowTitleLabel";
        const string k_WindowDetailsPanel = "windowDetailsVisualContainer";
        const string k_WindowResultsScrollViewName = "windowResultsScrollView";
        const string k_WindowSearchTextFieldName = "searchBox";
        const string k_WindowAutoCompleteLabelName = "autoCompleteLabel";
        const string k_WindowSearchIconName = "searchIcon";
        const string k_WindowResizerName = "windowResizer";
        const int k_TabCharacter = 9;

        Label m_AutoCompleteLabel;
        IEnumerable<SearcherItem> m_Results;
        List<SearcherItem> m_VisibleResults;
        HashSet<SearcherItem> m_ExpandedResults;
        Searcher m_Searcher;
        string m_SuggestedTerm;
        string m_Text = string.Empty;
        Action<SearcherItem> m_SelectionCallback;
        Action<Searcher.AnalyticsEvent> m_AnalyticsDataCallback;
        ListView m_ListView;
        TextField m_SearchTextField;
        VisualElement m_SearchTextInput;
        VisualElement m_DetailsPanel;

        internal Label TitleLabel { get; }
        internal VisualElement Resizer { get; }

        public SearcherControl()
        {
            // Load window template.
            var windowUxmlTemplate = Resources.Load<VisualTreeAsset>("SearcherWindow");

            // Clone Window Template.
            var windowRootVisualElement = windowUxmlTemplate.CloneTree();
            windowRootVisualElement.AddToClassList("content");

            windowRootVisualElement.StretchToParentSize();

            // Add Window VisualElement to window's RootVisualContainer
            Add(windowRootVisualElement);

            m_VisibleResults = new List<SearcherItem>();
            m_ExpandedResults = new HashSet<SearcherItem>();

            m_ListView = this.Q<ListView>(k_WindowResultsScrollViewName);

            if (m_ListView != null)
            {
                m_ListView.bindItem = Bind;
                m_ListView.RegisterCallback<KeyDownEvent>(OnResultsScrollViewKeyDown);
                m_ListView.onItemChosen += obj => m_SelectionCallback((SearcherItem)obj);
                m_ListView.onSelectionChanged += selectedItems => m_Searcher.Adapter.OnSelectionChanged(selectedItems.OfType<SearcherItem>());
                m_ListView.focusable = true;
                m_ListView.tabIndex = 1;
            }

            m_DetailsPanel = this.Q(k_WindowDetailsPanel);

            TitleLabel = this.Q<Label>(k_WindowTitleLabel);

            m_SearchTextField = this.Q<TextField>(k_WindowSearchTextFieldName);
            if (m_SearchTextField != null)
            {
                m_SearchTextField.focusable = true;
                m_SearchTextField.RegisterCallback<InputEvent>(OnSearchTextFieldTextChanged);

                m_SearchTextInput = m_SearchTextField.Q(TextInputBaseField<string>.textInputUssName);
                m_SearchTextInput.RegisterCallback<KeyDownEvent>(OnSearchTextFieldKeyDown);
            }

            m_AutoCompleteLabel = this.Q<Label>(k_WindowAutoCompleteLabelName);

            Resizer = this.Q(k_WindowResizerName);

            RegisterCallback<AttachToPanelEvent>(OnEnterPanel);
            RegisterCallback<DetachFromPanelEvent>(OnLeavePanel);

            // TODO: HACK - ListView's scroll view steals focus using the scheduler.
            EditorApplication.update += HackDueToListViewScrollViewStealingFocus;

            style.flexGrow = 1;
        }

        void HackDueToListViewScrollViewStealingFocus()
        {
            m_SearchTextInput?.Focus();
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= HackDueToListViewScrollViewStealingFocus;
        }

        void OnEnterPanel(AttachToPanelEvent e)
        {
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        void OnLeavePanel(DetachFromPanelEvent e)
        {
            UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }

        void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode == KeyCode.Escape)
            {
                CancelSearch();
            }
        }

        void CancelSearch()
        {
            OnSearchTextFieldTextChanged(InputEvent.GetPooled(m_Text, string.Empty));
            m_SelectionCallback(null);
            m_AnalyticsDataCallback?.Invoke(new Searcher.AnalyticsEvent(Searcher.AnalyticsEvent.EventType.Cancelled, m_SearchTextField.value));
        }

        public void Setup(Searcher searcher, Action<SearcherItem> selectionCallback, Action<Searcher.AnalyticsEvent> analyticsDataCallback)
        {
            m_Searcher = searcher;
            m_SelectionCallback = selectionCallback;
            m_AnalyticsDataCallback = analyticsDataCallback;

            if (m_Searcher.Adapter.HasDetailsPanel)
            {
                m_Searcher.Adapter.InitDetailsPanel(m_DetailsPanel);
                m_DetailsPanel.RemoveFromClassList("hidden");
            }
            else
            {
                m_DetailsPanel.AddToClassList("hidden");
            }

            TitleLabel.text = m_Searcher.Adapter.Title;
            if (string.IsNullOrEmpty(TitleLabel.text))
            {
                TitleLabel.parent.style.visibility = Visibility.Hidden;
                TitleLabel.parent.style.position = Position.Absolute;
            }

            Refresh();
        }

        void Refresh()
        {
            var query = m_Text;
            m_Results = m_Searcher.Search(query);
            GenerateVisibleResults();

            // The first item in the results is always the highest scored item.
            // We want to scroll to and select this item.
            var visibleIndex = -1;
            m_SuggestedTerm = string.Empty;

            var results = m_Results.ToList();
            if (results.Any())
            {
                var scrollToItem = results.First();
                visibleIndex = m_VisibleResults.IndexOf(scrollToItem);

                var cursorIndex = m_SearchTextField.cursorIndex;

                if (query.Length > 0)
                {
                    var strings = scrollToItem.Name.Split(' ');
                    var wordStartIndex = cursorIndex == 0 ? 0 : query.LastIndexOf(' ', cursorIndex - 1) + 1;
                    var word = query.Substring(wordStartIndex, cursorIndex - wordStartIndex);

                    if (word.Length > 0)
                        foreach (var t in strings)
                        {
                            if (t.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                            {
                                m_SuggestedTerm = t;
                                break;
                            }
                        }
                }
            }

            m_ListView.itemsSource = m_VisibleResults;
            m_ListView.makeItem = m_Searcher.Adapter.MakeItem;
            m_ListView.Refresh();

            SetSelectedElementInResultsList(visibleIndex);
        }

        void GenerateVisibleResults()
        {
            if (string.IsNullOrEmpty(m_Text))
            {
                m_ExpandedResults.Clear();
                RemoveChildrenFromResults();
                return;
            }

            RegenerateVisibleResults();
            ExpandAllParents();
        }

        void ExpandAllParents()
        {
            m_ExpandedResults.Clear();
            foreach (var item in m_VisibleResults)
                if (item.HasChildren)
                    m_ExpandedResults.Add(item);
        }

        void RemoveChildrenFromResults()
        {
            m_VisibleResults.Clear();
            var parents = new HashSet<SearcherItem>();

            foreach (var item in m_Results.Where(i => !parents.Contains(i)))
            {
                var currentParent = item;

                while (true)
                {
                    if (currentParent.Parent == null)
                    {
                        if (parents.Contains(currentParent))
                            break;

                        parents.Add(currentParent);
                        m_VisibleResults.Add(currentParent);
                        break;
                    }

                    currentParent = currentParent.Parent;
                }
            }

            if (m_Searcher.SortComparison != null)
                m_VisibleResults.Sort(m_Searcher.SortComparison);
        }

        void RegenerateVisibleResults()
        {
            var idSet = new HashSet<SearcherItem>();
            m_VisibleResults.Clear();

            foreach (var item in m_Results.Where(item => !idSet.Contains(item)))
            {
                idSet.Add(item);
                m_VisibleResults.Add(item);

                var currentParent = item.Parent;
                while (currentParent != null)
                {
                    if (!idSet.Contains(currentParent))
                    {
                        idSet.Add(currentParent);
                        m_VisibleResults.Add(currentParent);
                    }

                    currentParent = currentParent.Parent;
                }

                AddResultChildren(item, idSet);
            }

            var comparison = m_Searcher.SortComparison ?? ((i1, i2) =>
            {
                var result = i1.Database.Id - i2.Database.Id;
                return result != 0 ? result : i1.Id - i2.Id;
            });
            m_VisibleResults.Sort(comparison);
        }

        void AddResultChildren(SearcherItem item, ISet<SearcherItem> idSet)
        {
            if (!item.HasChildren)
                return;

            foreach (var child in item.Children)
            {
                if (!idSet.Contains(child))
                {
                    idSet.Add(child);
                    m_VisibleResults.Add(child);
                }

                AddResultChildren(child, idSet);
            }
        }

        bool HasChildResult(SearcherItem item)
        {
            if (m_Results.Contains(item))
                return true;

            foreach (var child in item.Children)
            {
                if (HasChildResult(child))
                    return true;
            }

            return false;
        }

        ItemExpanderState GetExpanderState(int index)
        {
            var item = m_VisibleResults[index];

            foreach (var child in item.Children)
            {
                if (!m_VisibleResults.Contains(child) && !HasChildResult(child))
                    continue;

                return m_ExpandedResults.Contains(item) ? ItemExpanderState.Expanded : ItemExpanderState.Collapsed;
            }

            return ItemExpanderState.Hidden;
        }

        void Bind(VisualElement target, int index)
        {
            var item = m_VisibleResults[index];
            var expanderState = GetExpanderState(index);
            var expander = m_Searcher.Adapter.Bind(target, item, expanderState, m_Text);
            expander.RegisterCallback<MouseDownEvent>(ExpandOrCollapse);
        }

        static void GetItemsToHide(SearcherItem parent, ref HashSet<SearcherItem> itemsToHide)
        {
            if (!parent.HasChildren)
            {
                itemsToHide.Add(parent);
                return;
            }

            foreach (var child in parent.Children)
            {
                itemsToHide.Add(child);
                GetItemsToHide(child, ref itemsToHide);
            }
        }

        void HideUnexpandedItems()
        {
            // Hide unexpanded children.
            var itemsToHide = new HashSet<SearcherItem>();
            foreach (var item in m_VisibleResults)
            {
                if (m_ExpandedResults.Contains(item))
                    continue;

                if (!item.HasChildren)
                    continue;

                if (itemsToHide.Contains(item))
                    continue;

                // We need to hide its children.
                GetItemsToHide(item, ref itemsToHide);
            }

            foreach (var item in itemsToHide)
                m_VisibleResults.Remove(item);
        }

        // ReSharper disable once UnusedMember.Local
        void RefreshListViewOn()
        {
            // TODO: Call ListView.Refresh() when it is fixed.
            // Need this workaround until then.
            // See: https://fogbugz.unity3d.com/f/cases/1027728/
            // And: https://gitlab.internal.unity3d.com/upm-packages/editor/com.unity.searcher/issues/9

            var scrollView = m_ListView.Q<ScrollView>();

            var scroller = scrollView?.Q<Scroller>("VerticalScroller");
            if (scroller == null)
                return;

            var oldValue = scroller.value;
            scroller.value = oldValue + 1.0f;
            scroller.value = oldValue - 1.0f;
            scroller.value = oldValue;
        }

        void Expand(SearcherItem item)
        {
            m_ExpandedResults.Add(item);

            RegenerateVisibleResults();
            HideUnexpandedItems();

            m_ListView.Refresh();
        }

        void Collapse(SearcherItem item)
        {
            // if it's already collapsed or not collapsed
            if (!m_ExpandedResults.Remove(item))
            {
                // this case applies for a left arrow key press
                if (item.Parent != null)
                    SetSelectedElementInResultsList(m_VisibleResults.IndexOf(item.Parent));

                // even if it's a root item and has no parents, do nothing more
                return;
            }

            RegenerateVisibleResults();
            HideUnexpandedItems();

            // TODO: understand what happened
            m_ListView.Refresh();

            // RefreshListViewOn();
        }

        void ExpandOrCollapse(MouseDownEvent evt)
        {
            if (!(evt.target is VisualElement expanderLabel))
                return;

            VisualElement itemElement = expanderLabel.GetFirstAncestorOfType<TemplateContainer>();

            if (!(itemElement?.userData is SearcherItem item)
                || !item.HasChildren
                || !expanderLabel.ClassListContains("Expanded") && !expanderLabel.ClassListContains("Collapsed"))
                return;

            if (!m_ExpandedResults.Contains(item))
                Expand(item);
            else
                Collapse(item);

            evt.StopImmediatePropagation();
        }

        void OnSearchTextFieldTextChanged(InputEvent inputEvent)
        {
            var text = inputEvent.newData;

            if (string.Equals(text, m_Text))
                return;

            // This is necessary due to OnTextChanged(...) being called after user inputs that have no impact on the text.
            // Ex: Moving the caret.
            m_Text = text;

            // If backspace is pressed and no text remain, clear the suggestion label.
            if (string.IsNullOrEmpty(text))
            {
                this.Q(k_WindowSearchIconName).RemoveFromClassList("Active");

                // Display the unfiltered results list.
                Refresh();

                m_AutoCompleteLabel.text = String.Empty;
                m_SuggestedTerm = String.Empty;

                SetSelectedElementInResultsList(0);

                return;
            }

            if (!this.Q(k_WindowSearchIconName).ClassListContains("Active"))
                this.Q(k_WindowSearchIconName).AddToClassList("Active");

            Refresh();

            // Calculate the start and end indexes of the word being modified (if any).
            var cursorIndex = m_SearchTextField.cursorIndex;

            // search toward the beginning of the string starting at the character before the cursor
            // +1 because we want the char after a space, or 0 if the search fails
            var wordStartIndex = cursorIndex == 0 ? 0 : (text.LastIndexOf(' ', cursorIndex - 1) + 1);

            // search toward the end of the string from the cursor index
            var wordEndIndex = text.IndexOf(' ', cursorIndex);
            if (wordEndIndex == -1) // no space found, assume end of string
                wordEndIndex = text.Length;

            // Clear the suggestion term if the caret is not within a word (both start and end indexes are equal, ex: (space)caret(space))
            // or the user didn't append characters to a word at the end of the query.
            if (wordStartIndex == wordEndIndex || wordEndIndex < text.Length)
            {
                m_AutoCompleteLabel.text = string.Empty;
                m_SuggestedTerm = string.Empty;
                return;
            }

            var word = text.Substring(wordStartIndex, wordEndIndex - wordStartIndex);

            if (!string.IsNullOrEmpty(m_SuggestedTerm))
            {
                var wordSuggestion =
                    word + m_SuggestedTerm.Substring(word.Length, m_SuggestedTerm.Length - word.Length);
                text = text.Remove(wordStartIndex, word.Length);
                text = text.Insert(wordStartIndex, wordSuggestion);
                m_AutoCompleteLabel.text = text;
            }
            else
            {
                m_AutoCompleteLabel.text = String.Empty;
            }
        }

        void OnResultsScrollViewKeyDown(KeyDownEvent keyDownEvent)
        {
            switch (keyDownEvent.keyCode)
            {
                case KeyCode.UpArrow:
                case KeyCode.DownArrow:
                case KeyCode.Home:
                case KeyCode.End:
                case KeyCode.PageUp:
                case KeyCode.PageDown:
                    return;
                default:
                    SetSelectedElementInResultsList(keyDownEvent);
                    break;
            }
        }

        void OnSearchTextFieldKeyDown(KeyDownEvent keyDownEvent)
        {
            // First, check if we cancelled the search.
            if (keyDownEvent.keyCode == KeyCode.Escape)
            {
                CancelSearch();
                return;
            }

            // For some reason the KeyDown event is raised twice when entering a character.
            // As such, we ignore one of the duplicate event.
            // This workaround was recommended by the Editor team. The cause of the issue relates to how IMGUI works
            // and a fix was not in the works at the moment of this writing.
            if (keyDownEvent.character == k_TabCharacter)
            {
                // Prevent switching focus to another visual element.
                keyDownEvent.PreventDefault();

                return;
            }

            // If Tab is pressed, complete the query with the suggested term.
            if (keyDownEvent.keyCode == KeyCode.Tab)
            {
                // Used to prevent the TAB input from executing it's default behavior. We're hijacking it for auto-completion.
                keyDownEvent.PreventDefault();

                if (!string.IsNullOrEmpty(m_SuggestedTerm))
                {
                    SelectAndReplaceCurrentWord();
                    m_AutoCompleteLabel.text = string.Empty;

                    // TODO: Revisit, we shouldn't need to do this here.
                    m_Text = m_SearchTextField.text;

                    Refresh();

                    m_SuggestedTerm = string.Empty;
                }
            }
            else
            {
                SetSelectedElementInResultsList(keyDownEvent);
            }
        }

        void SelectAndReplaceCurrentWord()
        {
            var s = m_SearchTextField.value;
            var lastWordIndex = s.LastIndexOf(' ');
            lastWordIndex++;

            var newText = s.Substring(0, lastWordIndex) + m_SuggestedTerm;

            // Wait for SelectRange api to reach trunk
//#if UNITY_2018_3_OR_NEWER
//            m_SearchTextField.value = newText;
//            m_SearchTextField.SelectRange(m_SearchTextField.value.Length, m_SearchTextField.value.Length);
//#else
            // HACK - relies on the textfield moving the caret when being assigned a value and skipping
            // all low surrogate characters
            var magicMoveCursorToEndString = new string('\uDC00', newText.Length);
            m_SearchTextField.value = magicMoveCursorToEndString;
            m_SearchTextField.value = newText;

//#endif
        }

        void SetSelectedElementInResultsList(KeyDownEvent keyDownEvent)
        {
            if (m_ListView.childCount == 0)
                return;

            int index;
            switch (keyDownEvent.keyCode)
            {
                case KeyCode.Escape:
                    m_SelectionCallback(null);
                    m_AnalyticsDataCallback?.Invoke(new Searcher.AnalyticsEvent(Searcher.AnalyticsEvent.EventType.Cancelled, m_SearchTextField.value));
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (m_ListView.selectedIndex != -1)
                    {
                        m_SelectionCallback((SearcherItem)m_ListView.selectedItem);
                        m_AnalyticsDataCallback?.Invoke(new Searcher.AnalyticsEvent(Searcher.AnalyticsEvent.EventType.Picked, m_SearchTextField.value));
                    }
                    else
                    {
                        m_SelectionCallback(null);
                        m_AnalyticsDataCallback?.Invoke(new Searcher.AnalyticsEvent(Searcher.AnalyticsEvent.EventType.Cancelled, m_SearchTextField.value));
                    }
                    break;
                case KeyCode.LeftArrow:
                    index = m_ListView.selectedIndex;
                    if (index >= 0 && index < m_ListView.itemsSource.Count)
                        Collapse(m_ListView.selectedItem as SearcherItem);
                    break;
                case KeyCode.RightArrow:
                    index = m_ListView.selectedIndex;
                    if (index >= 0 && index < m_ListView.itemsSource.Count)
                        Expand(m_ListView.selectedItem as SearcherItem);
                    break;
                case KeyCode.UpArrow:
                case KeyCode.DownArrow:
                case KeyCode.PageUp:
                case KeyCode.PageDown:
                    index = m_ListView.selectedIndex;
                    if (index >= 0 && index < m_ListView.itemsSource.Count)
                        m_ListView.OnKeyDown(keyDownEvent);
                    break;
            }
        }

        void SetSelectedElementInResultsList(int selectedIndex)
        {
            var newIndex = selectedIndex >= 0 && selectedIndex < m_VisibleResults.Count ? selectedIndex : -1;
            if (newIndex < 0)
                return;

            m_ListView.selectedIndex = newIndex;
            m_ListView.ScrollToItem(m_ListView.selectedIndex);
        }
    }
}
