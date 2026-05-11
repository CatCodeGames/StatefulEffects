using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{

    public sealed class GameObjectShowHide : MonoShowHideTemplate
    {
        [SerializeField] private GameObject _gameObject;
        protected override void OnShow(Action onCompleted)
        {
            _gameObject.SetActive(true);
            onCompleted();
        }
        protected override void OnSetShown()
        {
            _gameObject.SetActive(true);
        }

        protected override void OnHide(Action onCompleted)
        {
            _gameObject.SetActive(false);
            onCompleted();
        }

        protected override void OnSetHidden()
        {
            _gameObject.SetActive(false);
        }

        protected override void OnStop() { }
    }
}