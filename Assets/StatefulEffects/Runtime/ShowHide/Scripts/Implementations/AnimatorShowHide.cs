using CatCode.Events;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class AnimatorShowHide : MonoShowHide
    {
        private StateChangeBehaviour _stateBehaviour;

        [SerializeField] private AnimatorShowHideConfig _config;
        [SerializeField] private Animator _animator;

        private EventValue<ShowHideState> _state;
        private bool _isRunning;

        public override bool IsRunning => _isRunning;
        public override IReadOnlyEventValue<ShowHideState> State => _state;

        public override bool Initialized { get; protected set; }

        private void Awake()
            => Initialize();

        public override void Initialize()
        {
            if (Initialized)
                return;

            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            var state = _config.GetStateByHash(stateInfo.shortNameHash);

            _state = new EventValue<ShowHideState>(state);

            _stateBehaviour = _animator.GetBehaviour<StateChangeBehaviour>();
            _stateBehaviour.StateEnter += OnStateEnter;

            Initialized = true;
        }

        public override void Show() => SetTrigger(ShowHideState.Showing);
        public override void SetShown() => SetTrigger(ShowHideState.Shown);
        public override void Hide() => SetTrigger(ShowHideState.Hiding);
        public override void SetHidden() => SetTrigger(ShowHideState.Hidden);
        public override void Stop()
        {
            ResetTriggers();
            _isRunning = false;
        }


        private void SetTrigger(ShowHideState state)
        {
            ResetTriggers();
            _isRunning = true;
            _animator.SetTrigger(_config.GetTriggerForState(state));
        }

        private void ResetTriggers()
        {
            var triggers = _config.TriggerHashes;
            for (int i = 0; i < triggers.Length; i++)
                _animator.ResetTrigger(triggers[i]);
        }

        private void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            var state = _config.GetStateByHash(animatorStateInfo.shortNameHash);
            _state.Value = state;
        }

#if UNITY_EDITOR        
        private void Reset()
        {
            _animator = GetComponent<Animator>();
        }
#endif
    }
}