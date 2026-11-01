using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public sealed class GameObjectShowHide : CallbackShowHide
    {
        [SerializeField] private GameObject _gameObject;

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            _gameObject.SetActive(true);
            onCompleted();
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            _gameObject.SetActive(false);
            onCompleted();
        }

        protected override void OnSetShown(ShowHideState from)
            => _gameObject.SetActive(true);

        protected override void OnSetHidden(ShowHideState from)
            => _gameObject.SetActive(false);

        protected override void OnStop(ShowHideState from) { }


#if UNITY_EDITOR
        private void Reset()
        {
            if (transform.childCount > 0)
                _gameObject = transform.GetChild(0).gameObject;
        }

        private void OnValidate()
        {
            if (_gameObject != null && transform.IsChildOf(_gameObject.transform))
                Debug.LogError($"{nameof(GameObjectShowHide)}: target '{_gameObject.name}' is this object or its parent. Disabling it would disable this component.", this);
        }
#endif
    }
}
