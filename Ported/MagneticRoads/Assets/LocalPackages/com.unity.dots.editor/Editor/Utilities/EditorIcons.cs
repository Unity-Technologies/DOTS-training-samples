using UnityEditor;
using UnityEngine;
using Bridge = Unity.Editor.Bridge;

namespace Unity.Entities.Editor
{
    static class EditorIcons
    {
        const string k_IconsLightDirectory = Constants.EditorDefaultResourcesPath + "icons/light";
        const string k_IconsDarkDirectory = Constants.EditorDefaultResourcesPath + "icons/dark";

        public static Texture2D RuntimeComponent { get; private set; }
        public static Texture2D Remove { get; private set; }
        public static Texture2D RoundedCorners { get; private set; }
        public static Texture2D Entity { get; private set; }
        public static Texture2D EntityGroup { get; private set; }
        public static Texture2D Filter { get; private set; }
        public static Texture2D Convert { get; private set; }
        public static Texture2D System { get; private set; }

        static EditorIcons()
        {
            LoadIcons();
        }

        static void LoadIcons()
        {
            RuntimeComponent = LoadIcon("RuntimeComponent/" + nameof(RuntimeComponent));
            Remove = LoadIcon("Remove/" + nameof(Remove));
            RoundedCorners = LoadIcon("RoundedCorners/" + nameof(RoundedCorners));
            Entity = LoadIcon("Entity/" + nameof(Entity));
            EntityGroup = LoadIcon("EntityGroup/" + nameof(EntityGroup));
            Filter = LoadIcon("Filter/" + nameof(Filter));
            Convert = LoadIcon("Convert/" + nameof(Convert));
            System = LoadIcon("System/" + nameof(System));
        }

        /// <summary>
        /// Workaround for `EditorGUIUtility.LoadIcon` not working with packages. This can be removed once it does
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static Texture2D LoadIcon(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var iconsDirectory = k_IconsLightDirectory;
            if (EditorGUIUtility.isProSkin)
            {
                iconsDirectory = k_IconsDarkDirectory;
            }

            // Try to use high DPI if possible
            if (Bridge.GUIUtility.pixelsPerPoint > 1.0)
            {
                var texture = LoadIconTexture($"{iconsDirectory}/{name}@2x.png");

                if (null != texture)
                {
                    return texture;
                }
            }

            // Fallback to low DPI if we couldn't find the high res or we are on a low res screen
            return LoadIconTexture($"{iconsDirectory}/{name}.png");
        }

        static Texture2D LoadIconTexture(string path)
        {
            var texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

            if (texture != null &&
                !Mathf.Approximately(texture.GetPixelsPerPoint(), (float)Bridge.GUIUtility.pixelsPerPoint) &&
                !Mathf.Approximately((float)Bridge.GUIUtility.pixelsPerPoint % 1f, 0.0f))
            {
                texture.filterMode = FilterMode.Bilinear;
            }

            return texture;
        }
    }
}
