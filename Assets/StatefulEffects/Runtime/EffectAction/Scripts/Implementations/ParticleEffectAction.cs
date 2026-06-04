using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class ParticleEffectAction : CallbackEffectAction
    {
        private Action _onCompleted;

        [SerializeField] private ParticleSystemStopListener[] _listeners;

        protected override void OnInitialize()
        {
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].Stopped.Raised += OnParticleSystemStopCallback;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].Stopped.Raised += OnParticleSystemStopCallback;
        }

        protected override void OnPlay(Action onCompleted)
        {
            _onCompleted = onCompleted;
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].ParticleSystem.Play();
        }

        protected override void OnPrepare()
        {
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].ParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        protected override void OnStop()
        {
            _onCompleted = null;
        }

        private void OnParticleSystemStopCallback()
        {
            for (int i = 0; i < _listeners.Length; i++)
                if (_listeners[i].IsAlive)
                    return;
            _onCompleted?.Invoke();
        }
    }
}
