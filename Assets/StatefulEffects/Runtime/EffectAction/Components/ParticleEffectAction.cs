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
                _listeners[i].Stopped += OnParticleSystemStopCallback;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].Stopped -= OnParticleSystemStopCallback;
        }

        protected override void OnPlay(EffectActionState from, Action onCompleted)
        {
            switch (from)
            {
                case EffectActionState.None:
                case EffectActionState.Playing:
                    StopAndClearParticles();
                    break;
                case EffectActionState.Ready:
                    break;
            }

            _onCompleted = onCompleted;
            PlayParticles();
        }

        protected override void OnPrepare(EffectActionState from)
        {
            _onCompleted = null;
            StopAndClearParticles();
        }

        protected override void OnStop(EffectActionState from)
            => _onCompleted = null;


        private void OnParticleSystemStopCallback()
        {
            for (int i = 0; i < _listeners.Length; i++)
                if (_listeners[i].IsAlive)
                    return;
            _onCompleted?.Invoke();
        }

        private void PlayParticles()
        {
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].ParticleSystem.Play();
        }

        private void StopAndClearParticles()
        {
            for (int i = 0; i < _listeners.Length; i++)
                _listeners[i].ParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }


#if UNITY_EDITOR
        private void Reset()
        {
            _listeners = GetComponentsInChildren<ParticleSystemStopListener>();

            for (int i = 0; i < _listeners.Length; i++)
            {
                var main = _listeners[i].ParticleSystem.main;
                main.playOnAwake = false;
            }
        }
#endif
    }
}
