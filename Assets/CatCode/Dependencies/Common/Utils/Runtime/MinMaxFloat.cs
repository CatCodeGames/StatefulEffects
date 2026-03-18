using System;

namespace CatCode.Common
{
    [Serializable]
    public struct MinMaxFloat
    {
        public float min;
        public float max;

        public MinMaxFloat(float min = 0f, float max = 1f)
        {
            this.min = min;
            this.max = max;
        }

        public static MinMaxFloat ZeroToOne => new(0f, 1f);
    }
}