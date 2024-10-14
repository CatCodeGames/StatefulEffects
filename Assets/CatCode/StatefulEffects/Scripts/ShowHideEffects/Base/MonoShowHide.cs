using System;
using UnityEngine;

namespace CatCode.ShowHideEffects
{
    public abstract class MonoShowHide : MonoBehaviour, IShowHide
    {
        [SerializeField] private StateTransitionController<ShowHideState> _stateTransitionController = new();

        public ShowHideState State => _stateTransitionController.State;

        public event Action<IShowHide, ShowHideState> StateChanged;

        public void Show()
        {
            _stateTransitionController.TransitionToState(
                c => OnShow(c),
                ShowHideState.Show,
                ShowHideState.Shown);
        }

        public void SetShown()
        {
            OnSetShown();
            _stateTransitionController.SetState(ShowHideState.Shown);
        }

        public void Hide()
        {
            _stateTransitionController.TransitionToState(
                c => OnHide(c),
                ShowHideState.Hide,
                ShowHideState.Hidden);
        }

        public void SetHidden()
        {
            OnSetHidden();
            _stateTransitionController.SetState(ShowHideState.Hidden);
        }

        public void Stop()
        {
            OnStop();
            _stateTransitionController.Stop();
        }


        protected abstract void OnShow(Action onCompleted);
        protected abstract void OnHide(Action onCompleted);
        protected abstract void OnSetShown();
        protected abstract void OnSetHidden();
        protected abstract void OnStop();
    }
}