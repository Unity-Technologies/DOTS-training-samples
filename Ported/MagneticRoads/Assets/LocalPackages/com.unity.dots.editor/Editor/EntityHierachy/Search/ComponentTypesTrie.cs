using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unity.Entities.Editor
{
    static class ComponentTypesTrie
    {
        static readonly Trie k_Trie = new Trie();

        static bool s_IsInitialized;
        static bool s_IsReady;

        public static void Initialize()
        {
            if (s_IsInitialized)
                return;

            s_IsInitialized = true;

            Task.Run(() =>
            {
                k_Trie.Index(TypeManager.GetAllTypes().Where(t => t.Type != null).Select(t => t.Type.Name));
                s_IsReady = true;
            });
        }

        public static IEnumerable<string> SearchType(string startWith)
            => !s_IsReady ? Array.Empty<string>() : k_Trie.Search(startWith);
    }
}
