using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    [RequireComponent(typeof(ParticleSystem)), DisallowMultipleComponent]
    public sealed class ParticleSystemStopListener : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        public ParticleSystem ParticleSystem => _particleSystem;

        public event Action Stopped;
        public bool IsAlive => _particleSystem.IsAlive(false);

        private void Reset()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            var mainModule = _particleSystem.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            Stopped?.Invoke();
        }
    }
}