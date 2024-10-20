using System;
using UnityEngine;

namespace CatCode.AlphaModifiers
{
    [Serializable]
    public class ParticleAlphaModifierStrategy : IAlphaModifierStrategy
    {
        private bool _initialized;
        [SerializeField] private ParticleSystem _particleSystem;

        private ParticleSystem.ColorOverLifetimeModule _colorOverLifetimeModule;
        private GradientAlphaKey[] _alphaKeysMin;
        private GradientAlphaKey[] _alphaKeysMax;

        public ParticleAlphaModifierStrategy(ParticleSystem particleSystem)
        {
            _particleSystem = particleSystem;          
        }

        public void Initialize()
        {
            if (_initialized)
                return;
            
            _initialized = true;
            _colorOverLifetimeModule = _particleSystem.colorOverLifetime;

            if (!_colorOverLifetimeModule.enabled)
                _colorOverLifetimeModule.enabled = true;

            if (_colorOverLifetimeModule.color.mode == ParticleSystemGradientMode.TwoGradients)
                _alphaKeysMin = _colorOverLifetimeModule.color.gradientMin.alphaKeys;
            _alphaKeysMax = _colorOverLifetimeModule.color.gradientMax.alphaKeys;
        }

        public void SetAlpha(float value)
        {
            Initialize();
            var minMaxGradient = _colorOverLifetimeModule.color;
            if (minMaxGradient.mode == ParticleSystemGradientMode.TwoGradients)
                GradientUtils.UpdateAlphaKeys(minMaxGradient.gradientMin, _alphaKeysMin, value);
            GradientUtils.UpdateAlphaKeys(minMaxGradient.gradientMax, _alphaKeysMax, value);
        }

    }
}