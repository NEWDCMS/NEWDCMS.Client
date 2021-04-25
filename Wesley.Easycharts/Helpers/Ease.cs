namespace Wesley.Easycharts
{
    internal static class Ease
    {
        public static float Out(float t) => t * t * t;


        public static float In(float t) => (--t) * t * t + 1;
    }
}
