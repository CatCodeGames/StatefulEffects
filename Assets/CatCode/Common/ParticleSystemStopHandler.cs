using System;
using UnityEngine;

namespace CatCode
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemStopHandler : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public ParticleSystem ParticleSystem { get { return _particleSystem; } }
        public event Action<ParticleSystemStopHandler> StopCallback;

        private void Reset()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            var mainModule = _particleSystem.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            StopCallback?.Invoke(this);
        }
    }
}