using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class AnimatorEffectAction : MonoEffectAction
    {
        private static readonly int _idleHashID = Animator.StringToHash("Idle");
        private static readonly int _playHashID = Animator.StringToHash("Play");

        public const string PlayTrigger = "play";

        private Action _onPlayCompleted;

        private StateChangeBehaviour _stateBehaviour;
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            _stateBehaviour = _animator.GetBehaviour<StateChangeBehaviour>();
            _stateBehaviour.StateExit += OnStateExit;
        }


        protected override void OnPlay(Action callback)
        {
            _onPlayCompleted = callback;
            _animator.SetTrigger(PlayTrigger);
        }

        protected override void OnStop()
        {
        }

        private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (animatorStateInfo.shortNameHash == _playHashID)
            {
                _onPlayCompleted?.Invoke();
                _onPlayCompleted = null;
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _animator = GetComponent<Animator>();
        }
#endif
    }
}