using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    [RequireComponent(typeof(Animator))]
    public sealed class AnimatorShowHide : MonoShowHide
    {
        [SerializeField, HideInInspector] private Animator _animator;
        [SerializeField] private AnimatorShowHideConfig _config;

        private StateChangeBehaviour _stateBehaviour;
        private ShowHideState _state;

        public override ShowHideState State => _state;
        public override event Action<ShowHideState> OnStateChanged;

        private void OnEnable()
        {
            _stateBehaviour = _animator.GetBehaviour<StateChangeBehaviour>();
            _stateBehaviour.StateEnter += OnStateEnter;
        }

        private void OnDisable()
        {
            _stateBehaviour.StateEnter -= OnStateEnter;
            _stateBehaviour = null;
            Stop();
        }

        public override void Show()
        {
            switch (_state)
            {
                case ShowHideState.None:
                    ResetTriggers();
                    _animator.SetTrigger(_config.SetHiddenTriggerHash);
                    _animator.SetTrigger(_config.ShowTriggerHash);
                    break;

                case ShowHideState.Hiding:
                case ShowHideState.Hidden:
                    SetTrigger(_config.ShowTriggerHash);
                    break;

                case ShowHideState.Showing:
                case ShowHideState.Shown:
                    return;
            }

            SetState(ShowHideState.Showing);
        }

        public override void Hide()
        {
            switch (_state)
            {
                case ShowHideState.None:
                    ResetTriggers();
                    _animator.SetTrigger(_config.SetShownTriggerHash);
                    _animator.SetTrigger(_config.HideTriggerHash);
                    break;

                case ShowHideState.Showing:
                case ShowHideState.Shown:
                    SetTrigger(_config.HideTriggerHash);
                    break;

                case ShowHideState.Hiding:
                case ShowHideState.Hidden:
                    return;

            }

            SetState(ShowHideState.Hiding);
        }

        public override void SetShown()
        {
            if (_state == ShowHideState.Shown)
                return;

            SetTrigger(_config.SetShownTriggerHash);
            SetState(ShowHideState.Shown);
        }

        public override void SetHidden()
        {
            if (_state == ShowHideState.Hidden)
                return;

            SetTrigger(_config.SetHiddenTriggerHash);
            SetState(ShowHideState.Hidden);
        }

        public override void Stop()
        {
            ResetTriggers();
            SetState(ShowHideState.None);
        }


        private void SetTrigger(int trigger)
        {
            ResetTriggers();
            _animator.SetTrigger(trigger);
        }

        private void ResetTriggers()
        {
            _animator.ResetTrigger(_config.ShowTriggerHash);
            _animator.ResetTrigger(_config.HideTriggerHash);
            _animator.ResetTrigger(_config.SetShownTriggerHash);
            _animator.ResetTrigger(_config.SetHiddenTriggerHash);
        }

        private void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layer)
        {
            switch (_state)
            {
                case ShowHideState.None:
                case ShowHideState.Hidden:
                case ShowHideState.Shown:
                    return;

                case ShowHideState.Showing when stateInfo.shortNameHash == _config.ShownStateHash:
                    SetState(ShowHideState.Shown);
                    return;

                case ShowHideState.Hiding when stateInfo.shortNameHash == _config.HiddenStateHash:
                    SetState(ShowHideState.Hidden);
                    return;
            }
        }

        private void SetState(ShowHideState state)
        {
            if (_state == state)
                return;

            _state = state;
            OnStateChanged?.Invoke(_state);
        }


#if UNITY_EDITOR
        private void Reset()
            => _animator = GetComponent<Animator>();

        private void OnValidate()
            => _animator = GetComponent<Animator>();
#endif
    }
}
