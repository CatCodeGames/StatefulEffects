using CatCode.Events;
using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class GameObjectShowHide : MonoShowHide
    {
        [SerializeField] private GameObject _gameObject;

        private IShowHide _showHide;

        [SerializeField] private ShowHideState _initialState;
        [SerializeField] private bool _ignoreState;

        private void Awake()
        {
            _showHide = new ShowHideCallbackStateMachine(
                _initialState,
                _ignoreState,
                OnShow,
                OnHide,
                OnSetShown,
                OnSetHidden,
                null);
        }

        private void OnShow(Action onCompleted)
        {
            _gameObject.SetActive(true);
            onCompleted();
        }
        private void OnSetShown() => _gameObject.SetActive(true);

        private void OnHide(Action onCompleted)
        {
            _gameObject.SetActive(false);
            onCompleted();
        }

        private void OnSetHidden() => _gameObject.SetActive(false);


        #region IShowHide 

        public override IReadOnlyEventValue<ShowHideState> State => _showHide.State;

        public override void Show() => _showHide.Show();
        public override void SetShown() => _showHide.SetShown();
        public override void Hide() => _showHide.Hide();
        public override void SetHidden() => _showHide.SetHidden();
        public override void Stop() => _showHide.Stop();

        #endregion
    }
}