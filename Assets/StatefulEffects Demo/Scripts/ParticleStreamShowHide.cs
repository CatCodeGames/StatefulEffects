#nullable enable

using System;
using LitMotion;
using UnityEngine;

namespace CatCode.StatefulEffects.Demo
{
    public sealed class ParticleStreamShowHide : CallbackShowHide
    {
        [SerializeField] private ParticleSystemStopListener _stopListener = null!;
        private ParticleSystem.ColorOverLifetimeModule _colorOverLifetime;
        private MotionHandle _fade;
        private Action? _completeHide;
        private float _alpha;

        protected override void OnInitialize()
        {
            _colorOverLifetime = _stopListener.ParticleSystem.colorOverLifetime;
            _stopListener.Stopped += OnParticlesStopped;
        }

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            CancelTransition();
            if (from == ShowHideState.None || from == ShowHideState.Hidden)
                ApplyAlpha(0f);

            _stopListener.ParticleSystem.Play(false);
            _fade = LMotion.Create(_alpha, 1f, 1f)
                .WithOnComplete(onCompleted)
                .Bind(ApplyAlpha);
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            CancelTransition();
            _completeHide = onCompleted;
            _stopListener.ParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            OnParticlesStopped();
        }

        protected override void OnSetShown(ShowHideState from)
        {
            CancelTransition();
            var particles = _stopListener.ParticleSystem;
            particles.Simulate(particles.main.startLifetime.constantMax, false, true);
            ApplyAlpha(1f);
            particles.Play(false);
        }

        protected override void OnSetHidden(ShowHideState from)
        {
            CancelTransition();
            _stopListener.ParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            ApplyAlpha(0f);
        }

        protected override void OnStop(ShowHideState from)
        {
            CancelTransition();
            if (_stopListener == null)
                return;

            var particles = _stopListener.ParticleSystem;
            if (particles != null)
                particles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        protected override void OnDestroy()
        {
            if (_stopListener != null)
                _stopListener.Stopped -= OnParticlesStopped;

            base.OnDestroy();
        }

        private void OnParticlesStopped()
        {
            if (_completeHide == null || _stopListener.IsAlive)
                return;

            var onCompleted = _completeHide;
            _completeHide = null;
            ApplyAlpha(0f);
            onCompleted();
        }

        private void CancelTransition()
        {
            _completeHide = null;
            _fade.TryCancel();
        }

        private void ApplyAlpha(float alpha)
        {
            _alpha = alpha;
            _colorOverLifetime.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
