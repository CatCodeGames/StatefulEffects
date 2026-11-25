#nullable enable

using System;
using System.Collections;
using UnityEngine;

namespace CatCode.StatefulEffects.Demo
{
    public sealed class SphereAndLightShowHide : CallbackShowHide
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private MeshRenderer _sphere = null!;
        [SerializeField] private Light _pointLight = null!;
        [SerializeField] private Color _hiddenColor = new(0.25f, 0.3f, 0.3f);
        [SerializeField] private Color _shownColor = new(0.6f, 0.9f, 0.80f);
        [SerializeField, ColorUsage(false, true)] private Color _shownEmission = new(0.6f, 2f, 1f);
        [SerializeField, Min(0f)] private float _shownIntensity = 1f;
        [SerializeField, Min(0f)] private float _transitionSeconds = 0.5f;

        private Material _material = null!;
        private Coroutine? _transition;
        private float _brightness;

        protected override void OnInitialize()
        {
            _material = _sphere.material;
            _material.EnableKeyword("_EMISSION");
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_material != null)
                Destroy(_material);
        }

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            if (from == ShowHideState.None)
                SetBrightness(0f);

            TransitionTo(1f, onCompleted);
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            if (from == ShowHideState.None)
                SetBrightness(1f);

            TransitionTo(0f, onCompleted);
        }

        protected override void OnSetShown(ShowHideState from) => SnapTo(1f);
        protected override void OnSetHidden(ShowHideState from) => SnapTo(0f);
        protected override void OnStop(ShowHideState from) => StopTransition();

        private void TransitionTo(float target, Action onCompleted)
        {
            StopTransition();

            if (Mathf.Approximately(_brightness, target) || _transitionSeconds <= 0f)
            {
                SetBrightness(target);
                onCompleted();
                return;
            }

            _transition = StartCoroutine(FadeRoutine(_brightness, target, onCompleted));
        }

        private IEnumerator FadeRoutine(float start, float target, Action onCompleted)
        {
            float elapsed = 0f;
            while (elapsed < _transitionSeconds)
            {
                elapsed += Time.deltaTime;
                SetBrightness(Mathf.SmoothStep(start, target, elapsed / _transitionSeconds));
                yield return null;
            }

            _transition = null;
            SetBrightness(target);
            onCompleted();
        }

        private void SnapTo(float brightness)
        {
            StopTransition();
            SetBrightness(brightness);
        }

        private void StopTransition()
        {
            if (_transition == null)
                return;

            StopCoroutine(_transition);
            _transition = null;
        }

        private void SetBrightness(float brightness)
        {
            _brightness = brightness;
            _material.SetColor(BaseColorId, Color.LerpUnclamped(_hiddenColor, _shownColor, brightness));
            _material.SetColor(EmissionColorId, _shownEmission * brightness);
            _pointLight.intensity = _shownIntensity * brightness;
        }
    }
}