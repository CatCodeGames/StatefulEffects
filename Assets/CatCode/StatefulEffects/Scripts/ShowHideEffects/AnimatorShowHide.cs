using System;
using UnityEngine;

namespace CatCode.ShowHideEffects
{
    public sealed class AnimatorShowHide : MonoShowHide
    {
        private static int _showHashId = Animator.StringToHash("Show");
        private static int _shownHashId = Animator.StringToHash("Shown");
        private static int _hideHashId = Animator.StringToHash("Hide");
        private static int _hiddenHashId = Animator.StringToHash("Hidden");

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
            if (animatorStateInfo.shortNameHash == _showHashId)
            {
                _onShowCompleted?.Invoke();
                _onShowCompleted = null;
            }
            else if (animatorStateInfo.shortNameHash == _hideHashId)
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