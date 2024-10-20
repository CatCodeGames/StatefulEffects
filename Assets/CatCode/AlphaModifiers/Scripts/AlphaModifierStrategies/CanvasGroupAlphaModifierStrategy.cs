using UnityEngine;

namespace CatCode.AlphaModifiers
{
    public sealed class CanvasGroupAlphaModifierStrategy : IAlphaModifierStrategy
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _alpha;

        public CanvasGroupAlphaModifierStrategy(CanvasGroup canvasGroup)
        {
            _canvasGroup = canvasGroup;
            _alpha = _canvasGroup.alpha;
        }

        public void SetAlpha(float value)
        {
            var alpha = Mathf.Clamp01(_alpha * value);
            _canvasGroup.alpha = alpha;
        }
    }
}