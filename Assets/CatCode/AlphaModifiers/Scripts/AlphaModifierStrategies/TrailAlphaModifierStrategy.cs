using UnityEngine;

namespace CatCode.AlphaModifiers
{
    public sealed class TrailAlphaModifierStrategy : IAlphaModifierStrategy
    {
        private TrailRenderer _trailRenderer;
        private GradientAlphaKey[] _alphaKeys;

        public TrailAlphaModifierStrategy(TrailRenderer trailRenderer)
        {
            _trailRenderer = trailRenderer;
            _alphaKeys = _trailRenderer.colorGradient.alphaKeys;
        }

        public void SetAlpha(float value)
        {
            GradientUtils.UpdateAlphaKeys(_trailRenderer.colorGradient, _alphaKeys, value);
        }
    }
}