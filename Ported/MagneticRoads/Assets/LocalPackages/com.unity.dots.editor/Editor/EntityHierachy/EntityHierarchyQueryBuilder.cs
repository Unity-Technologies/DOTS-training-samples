using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Unity.Entities.Editor
{
    class EntityHierarchyQueryBuilder
    {
        static readonly Regex k_Regex = new Regex(@"\b(?<token>[cC]:)\s*(?<componentType>(\S)*)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
        readonly StringBuilder m_UnmatchedInputBuilder;

        public EntityHierarchyQueryBuilder()
        {
            m_UnmatchedInputBuilder = new StringBuilder();
        }

        public Result BuildQuery(string input)
        {
            if (string.IsNullOrEmpty(input))
                return Result.ValidBecauseEmpty;

            var matches = k_Regex.Matches(input);
            if (matches.Count == 0)
                return Result.Valid(null, input);

            using (var componentTypes = PooledHashSet<ComponentType>.Make())
            {
                m_UnmatchedInputBuilder.Clear();
                var pos = 0;
                for (var i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var matchGroup = match.Groups["componentType"];

                    var length = match.Index - pos;
                    if (length > 0)
                        m_UnmatchedInputBuilder.Append(input.Substring(pos, length));

                    pos = match.Index + match.Length;

                    if (matchGroup.Value.Length == 0)
                        continue;

                    var results = ComponentTypeCache.GetExactMatchingTypes(matchGroup.Value);
                    var resultFound = false;
                    foreach (var result in results)
                    {
                        resultFound = true;
                        componentTypes.Set.Add(result);
                    }

                    if (!resultFound)
                        return Result.Invalid(matchGroup.Value);
                }

                if (input.Length - pos > 0)
                    m_UnmatchedInputBuilder.Append(input.Substring(pos));

                return Result.Valid(new EntityQueryDesc { Any = componentTypes.Set.ToArray(), Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled }, m_UnmatchedInputBuilder.ToString());
            }
        }

        public struct Result
        {
            public bool IsValid;
            public EntityQueryDesc QueryDesc;
            public string ErrorComponentType;
            public string Filter;

            public static readonly Result ValidBecauseEmpty = new Result { IsValid = true, QueryDesc = null, Filter = string.Empty, ErrorComponentType = string.Empty };

            public static Result Invalid(string errorComponentType)
                => new Result { IsValid = false, QueryDesc = null, Filter = string.Empty, ErrorComponentType = errorComponentType };

            public static Result Valid(EntityQueryDesc queryDesc, string filter)
                => new Result { IsValid = true, QueryDesc = queryDesc, Filter = filter, ErrorComponentType = string.Empty };
        }
    }
}
