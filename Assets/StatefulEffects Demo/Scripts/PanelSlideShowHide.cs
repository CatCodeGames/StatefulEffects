#nullable enable

using System;
using LitMotion;
using UnityEngine;

namespace CatCode.StatefulEffects.Demo
{
    public sealed class PanelSlideShowHide : CallbackShowHide
    {
        [SerializeField] private CanvasGroup _panel = null!;
        [SerializeField] private RectTransform _button = null!;
        [SerializeField] private RectTransform _text = null!;
        [SerializeField, Min(0f)] private float _slidePixels = 130f;
        [SerializeField, Range(0f, 1f)] private float _hiddenScale = 0.55f;
        [SerializeField, Min(0.01f)] private float _transitionSeconds = 0.65f;

        private Vector2 _buttonShown;
        private Vector2 _textShown;
        private MotionHandle _motion;
        private float _progress;

        protected override void OnInitialize()
        {
            _buttonShown = _button.anchoredPosition;
            _textShown = _text.anchoredPosition;
        }

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            if (from == ShowHideState.None)
                Apply(0f);

            AnimateTo(1f, onCompleted);
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            if (from == ShowHideState.None)
                Apply(1f);

            AnimateTo(0f, onCompleted);
        }

        protected override void OnSetShown(ShowHideState from) => SnapTo(1f);
        protected override void OnSetHidden(ShowHideState from) => SnapTo(0f);
        protected override void OnStop(ShowHideState from) => _motion.TryCancel();

        private void AnimateTo(float target, Action onCompleted)
        {
            _motion.TryCancel();
            _motion = LMotion.Create(_progress, target, _transitionSeconds)
                .WithOnComplete(onCompleted)
                .Bind(Apply);
        }

        private void SnapTo(float progress)
        {
            _motion.TryCancel();
            Apply(progress);
        }

        private void Apply(float progress)
        {
            _progress = progress;
            _panel.alpha = Mathf.InverseLerp(0f, 0.4f, progress);

            float travel = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.15f, 1f, progress));
            var offset = Vector2.right * (_slidePixels * (1f - travel));
            var scale = Vector3.one * Mathf.Lerp(_hiddenScale, 1f, travel);
            _button.anchoredPosition = _buttonShown - offset;
            _button.localScale = scale;
            _text.anchoredPosition = _textShown + offset;
            _text.localScale = scale;
        }
    }
}
