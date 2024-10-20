using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class ParticleEffectAction : MonoEffectAction
    {
        private Action _onCompleted;

        [SerializeField] private ParticleSystemStopHandler[] _particleSystems;

        private void Awake()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].StopCallback += OnParticleSystemStopCallback;
        }

        protected override void OnPlay(Action onCompleted)
        {
            _onCompleted = onCompleted;
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].ParticleSystem.Play();
        }

        protected override void OnStop()
        {
            _onCompleted = null;
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].ParticleSystem.Stop();
        }

        private void OnParticleSystemStopCallback(ParticleSystemStopHandler handler)
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                if (_particleSystems[i].ParticleSystem.IsAlive(false))
                    return;
            _onCompleted?.Invoke();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystemStopHandler>();
            for (int i = 0; i < _particleSystems.Length; i++)
            {
                var main = _particleSystems[i].ParticleSystem.main;
                main.loop = false;
                main.playOnAwake = false;
            }
        }
#endif
    }
}