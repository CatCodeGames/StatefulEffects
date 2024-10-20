using UnityEngine;

namespace CatCode.AlphaModifiers
{
    public static class GradientUtils
    {
        public static void UpdateAlphaKeys(Gradient gradient, GradientAlphaKey[] defaultAlphaKeys, float alpha)
        {
            var alphaKeys = gradient.alphaKeys;
            var length = Mathf.Min(alphaKeys.Length, defaultAlphaKeys.Length);

            for (int i = 0; i < length; i++)
            {
                ref var alphaKey = ref alphaKeys[i];
                alphaKey.alpha = defaultAlphaKeys[i].alpha * alpha;
            }

            gradient.alphaKeys = alphaKeys;
        }
    }
}