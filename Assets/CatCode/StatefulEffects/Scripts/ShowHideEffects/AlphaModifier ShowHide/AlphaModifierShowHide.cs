using CatCode.AlphaModifiers;
using DG.Tweening;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public class AlphaModifierShowHide : MonoShowHide
    {
        [Serializable]
        public struct Settings
        {
            public float alpha;
            public float duration;
        }

        private Tween _tween;
        [SerializeField] private MonoAlphaModifier _alphaModifier;
        [SerializeField] private Settings _showSettings;
        [SerializeField] private Settings _hideSettings;


        protected override void OnShow(Action onCompleted)
            => ChangeAlpha(_showSettings, onCompleted);

        protected override void OnSetShown()
            => SetAlpha(_showSettings);

        protected override void OnHide(Action onCompleted) 
            => ChangeAlpha(_hideSettings, onCompleted);

        protected override void OnSetHidden()
            => SetAlpha(_hideSettings);

        protected override void OnStop()
            => _tween.Kill();


        private void ChangeAlpha(Settings settings, Action callback)
        {
            _tween.Kill();
            var diff = _showSettings.alpha - _hideSettings.alpha;
            var duration = Mathf.Abs((settings.alpha - _alphaModifier.Alpha) / diff) * settings.duration;
            _tween = DOTween
                .To(() => _alphaModifier.Alpha, value => _alphaModifier.Alpha = value, settings.alpha, duration)
                .OnComplete(() => callback());
        }
        private void SetAlpha(Settings settings)
        {
            _tween.Kill();
            _alphaModifier.Alpha = settings.alpha;
        }
    }
}