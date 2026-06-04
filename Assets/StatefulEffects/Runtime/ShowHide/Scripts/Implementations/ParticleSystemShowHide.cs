using System;
using UnityEngine;


namespace CatCode.StatefulEffects
{
    public sealed class ParticleSystemShowHide : CallbackMonoShowHide
    {
        private Action _onCompleted;

        [SerializeField] private ParticleSystemStopListener[] _particleSystems;

        protected override void OnInitialize()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].Stopped.Raised += OnParticleSystemStopCallback;
        }

        protected override void OnDestroy()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].Stopped.Raised -= OnParticleSystemStopCallback;
        }

        protected override void OnShow(Action onCompleted)
        {
            PlayParticles();
            onCompleted?.Invoke();
        }

        protected override void OnSetShown()
           => PlayPrewarmParticles();

        protected override void OnHide(Action onCompleted)
        {
            _onCompleted = onCompleted;
            StopParticles();
        }

        protected override void OnSetHidden()
            => StopAndClearParticles();

        protected override void OnStop()
            => _onCompleted = null;


        private void OnParticleSystemStopCallback()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                if (_particleSystems[i].IsAlive)
                    return;
            _onCompleted?.Invoke();
        }

        private void PlayParticles()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].ParticleSystem.Play();
        }

        private void PlayPrewarmParticles()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                var particleSystem = _particleSystems[i].ParticleSystem;
                particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                var mainModule = particleSystem.main;
                var prewarm = mainModule.prewarm;
                mainModule.prewarm = true;
                particleSystem.Play();
                mainModule.prewarm = prewarm;
            }
        }

        private void StopParticles()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].ParticleSystem.Stop();
        }

        private void StopAndClearParticles()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].ParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }


#if UNITY_EDITOR
        private void Reset()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystemStopListener>();

            for (int i = 0; i < _particleSystems.Length; i++)
            {
                var main = _particleSystems[i].ParticleSystem.main;
                main.playOnAwake = false;
            }
        }
#endif
    }
}