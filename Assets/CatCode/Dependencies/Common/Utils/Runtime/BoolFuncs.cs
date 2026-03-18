namespace CatCode.Common
{
    public static class BoolFuncs
    {
        public static bool And(bool[] values)
        {
            for (int i = 0; i < values.Length; i++)
                if (!values[i])
                    return false;
            return true;
        }

        public static bool Or(bool[] values)
        {
            for (int i = 0; i < values.Length; i++)
                if (values[i])
                    return true;
            return false;
        }

        public static bool Nand(bool[] values) => !And(values);
        public static bool Nor(bool[] values) => !Or(values);

        public static bool AllTrue(bool[] values) => And(values);
        public static bool AnyTrue(bool[] values) => Or(values);

        public static bool AllFalse(bool[] values) => !Or(values);
        public static bool AnyFalse(bool[] values) => !And(values);
    }
}
