namespace MetroECS.Trains
{
    public static class MathHelpers
    {
        public static int Loop(in int index, in int maxValue)
        {
            return index < 0 ? index + maxValue : (index >= maxValue ? index - maxValue : index);
        }
    }
}