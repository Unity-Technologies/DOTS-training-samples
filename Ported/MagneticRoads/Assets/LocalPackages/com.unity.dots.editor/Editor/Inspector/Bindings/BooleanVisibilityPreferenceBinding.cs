namespace Unity.Entities.Editor
{
    class BooleanVisibilityPreferenceBinding : InspectorBinding<bool>
    {
        protected override void OnUpdate(bool visible)
        {
            if (visible)
                Target.Show();
            else
                Target.Hide();
        }
    }
}
