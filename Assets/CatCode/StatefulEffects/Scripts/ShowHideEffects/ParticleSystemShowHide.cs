using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class ParticleSystemShowHide : MonoShowHide
    {
        private Action _onStopped;

        [SerializeField] private ParticleSystemStopHandler[] _particleSystems;

        private void OnEnable()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].StopCallback += OnParticleSystemStopCallback;
        }

        private void OnDisable()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].StopCallback -= OnParticleSystemStopCallback;
        }
            
        protected override void OnShow(Action onCompleted)
        {
            PlayParticles();
            onCompleted?.Invoke();
        }

        protected override void OnHide(Action onCompleted)
        {
            _onStopped = onCompleted;
            StopParticles();
        }

        protected override void OnSetShown()
            => PlayPrewarmParticles();

        protected override void OnSetHidden()
            => StopAndClearParticles();

        protected override void OnStop()
        {
            _onStopped = null;
        }
       
        private void OnParticleSystemStopCallback(ParticleSystemStopHandler handler)
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                if (_particleSystems[i].ParticleSystem.IsAlive())
                    return;
            _onStopped?.Invoke();
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
            _particleSystems = GetComponentsInChildren<ParticleSystemStopHandler>();

            for (int i = 0; i < _particleSystems.Length; i++)
            {
                var main = _particleSystems[i].ParticleSystem.main;
                main.playOnAwake = false;
            }
        }
#endif
    }
}