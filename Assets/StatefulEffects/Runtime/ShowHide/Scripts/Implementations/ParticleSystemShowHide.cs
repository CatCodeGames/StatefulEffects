using CatCode.Events;
using System;
using UnityEngine;


namespace CatCode.StatefulEffects
{

    public sealed class ParticleSystemShowHide : MonoShowHide
    {
        private Action _onStopped;
        private IShowHide _showHide;

        [SerializeField] private ShowHideState _initialState;
        [SerializeField] private bool _ignoreState;
        [SerializeField] private ParticleSystemStopListener[] _particleSystems;

        private void Awake()
        {
            _showHide = new ShowHideCallbackStateMachine(
                _initialState,
                _ignoreState,
                OnShow,
                OnHide,
                OnSetShown,
                OnSetHidden,
                OnStop);
        }

        private void OnEnable()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].Stopped.Raised += OnParticleSystemStopCallback;
        }

        private void OnDisable()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                _particleSystems[i].Stopped.Raised -= OnParticleSystemStopCallback;
        }

        private void OnShow(Action onCompleted)
        {
            PlayParticles();
            onCompleted?.Invoke();
        }

        private void OnSetShown()
           => PlayPrewarmParticles();

        private void OnHide(Action onCompleted)
        {
            _onStopped = onCompleted;
            StopParticles();
        }

        private void OnSetHidden()
            => StopAndClearParticles();

        private void OnStop()
            => _onStopped = null;


        private void OnParticleSystemStopCallback()
        {
            for (int i = 0; i < _particleSystems.Length; i++)
                if (_particleSystems[i].IsAlive)
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

        #region IShowHide 

        public override IReadOnlyEventValue<ShowHideState> State => _showHide.State;

        public override void Show() => _showHide.Show();
        public override void SetShown() => _showHide.SetShown();
        public override void Hide() => _showHide.Hide();
        public override void SetHidden() => _showHide.SetHidden();
        public override void Stop() => _showHide.Stop();

        #endregion

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