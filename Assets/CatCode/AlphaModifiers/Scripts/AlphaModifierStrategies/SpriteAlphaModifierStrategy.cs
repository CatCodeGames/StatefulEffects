using System;
using UnityEngine;

namespace CatCode.AlphaModifiers
{
    [Serializable]
    public sealed class SpriteAlphaModifierStrategy : IAlphaModifierStrategy
    {
        [SerializeField] private float _alpha;
        [SerializeField] private SpriteRenderer _renderer;

        public SpriteAlphaModifierStrategy(SpriteRenderer renderer)
        {
            _renderer = renderer;
            _alpha = _renderer.color.a;
        }

        public void SetAlpha(float value)
        {
            var alpha = Mathf.Clamp01(_alpha * value);
            var color = _renderer.color;
            color.a = alpha;
            _renderer.color = color;
        }
    }
}