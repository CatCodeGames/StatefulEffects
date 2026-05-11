using CatCode.Events;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemStopListener : MonoBehaviour
    {
        private EventSignal _stopped = new EventSignal();

        [SerializeField] private ParticleSystem _particleSystem;
        public ParticleSystem ParticleSystem => _particleSystem;

        public IReadOnlyEventSignal Stopped => _stopped;
        public bool IsAlive => _particleSystem.IsAlive();

        private void Reset()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            var mainModule = _particleSystem.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
        }

        private void OnParticleSystemStopped()
        {
            _stopped.Raise();
        }
    }
}