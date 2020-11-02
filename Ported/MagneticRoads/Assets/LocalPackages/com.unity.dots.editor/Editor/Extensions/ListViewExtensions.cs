namespace Unity.Entities.Editor
{
    static class ListViewExtensions
    {
#if !UNITY_2020_1_OR_NEWER
        public static void ClearSelection(this ListView listView)
        {
            listView.selectedIndex = -1;
        }
#endif
    }
}
