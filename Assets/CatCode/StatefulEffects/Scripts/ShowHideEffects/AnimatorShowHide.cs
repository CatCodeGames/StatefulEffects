using System;
using UnityEngine;

namespace CatCode.ShowHideEffects
{
    public sealed class AnimatorShowHide : MonoShowHide
    {
        private static readonly int _showHashID = Animator.StringToHash("Show");
        private static readonly int _shownHashID = Animator.StringToHash("Shown");
        private static readonly int _hideHashID = Animator.StringToHash("Hide");
        private static readonly int _hiddenHashID = Animator.StringToHash("Hidden");

        public const string ShowTrigger = "show";
        public const string HideTrigger = "hide";
        public const string SetShownTrigger = "set_shown";
        public const string SetHiddenTrigger = "set_hidden";

        private Action _onShowCompleted;
        private Action _onHideCompleted;
        private StateChangeBehaviour _stateBehaviour;

        [SerializeField] private Animator _animator;

        private void Awake()
        {
            _stateBehaviour = _animator.GetBehaviour<StateChangeBehaviour>();
            _stateBehaviour.StateExit += OnStateExit;
        }

        protected override void OnShow(Action onCompleted)
        {
            _onShowCompleted = onCompleted;
            _animator.SetTrigger(ShowTrigger);
        }

        protected override void OnHide(Action onCompleted)
        {
            _onHideCompleted = onCompleted;
            _animator.SetTrigger(HideTrigger);
        }

        protected override void OnSetShown()
        {
            _animator.SetTrigger(SetShownTrigger);
        }

        protected override void OnSetHidden()
        {
            _animator.SetTrigger(SetHiddenTrigger);
        }

        protected override void OnStop()
        {
        }


        private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (animatorStateInfo.shortNameHash == _showHashID)
            {
                _onShowCompleted?.Invoke();
                _onShowCompleted = null;
            }
            else if (animatorStateInfo.shortNameHash == _hideHashID)
            {
                _onHideCompleted?.Invoke();
                _onHideCompleted = null;
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