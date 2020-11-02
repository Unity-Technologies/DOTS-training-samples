using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Unity.Entities.Editor
{
    class ComponentTypeAutoComplete : AutoComplete.IAutoCompleteBehavior
    {
        static ComponentTypeAutoComplete s_Instance;
        static readonly Regex k_Regex = new Regex(@"\b([cC]:)(?<componentType>(\S)*)$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

        public static ComponentTypeAutoComplete Instance => s_Instance ?? (s_Instance = new ComponentTypeAutoComplete());

        static ComponentTypeAutoComplete()
            => ComponentTypesTrie.Initialize();

        ComponentTypeAutoComplete() { }

        public bool ShouldStartAutoCompletion(string input, int caretPosition)
        {
            return GetToken(input, caretPosition).Length >= 1;
        }

        public string GetToken(string input, int caretPosition)
        {
            var match = k_Regex.Match(input, 0, caretPosition);
            if (!match.Success)
                return string.Empty;

            var type = match.Groups["componentType"];
            return type.Value;
        }

        public IEnumerable<string> GetCompletionItems(string input, int caretPosition)
        {
            var token = GetToken(input, caretPosition);
            return ComponentTypesTrie.SearchType(token);
        }

        public (string newInput, int caretPosition) OnCompletion(string completedToken, string input, int caretPosition)
        {
            var match = k_Regex.Match(input, 0, caretPosition);
            var componentType = match.Groups["componentType"];

            var indexOfNextSpace = input.IndexOf(' ', componentType.Index);
            var final = string.Concat(input.Substring(0, componentType.Index), completedToken);
            if (indexOfNextSpace != -1)
                final += input.Substring(indexOfNextSpace);

            return (final, componentType.Index + completedToken.Length);
        }
    }
}
