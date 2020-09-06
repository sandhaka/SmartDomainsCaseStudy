namespace Utils
{
    public static class NumberExt
    {
        public static int CastToInt(this double d)
        {
            return (int) d;
        }

        public static uint CastToUInt(this int d)
        {
            return (uint) d;
        }
    }
}