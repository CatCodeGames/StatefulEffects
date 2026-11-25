using TMPro;
using UnityEngine;

namespace CatCode.StatefulEffects.Demo
{
    public sealed class EffectLabel : MonoBehaviour
    {
        [SerializeField] private MonoShowHide _effect;
        [SerializeField] private TMP_Text _state;

        private void OnEnable()
        {
            if (_effect == null)
                return;

            _effect.OnStateChanged += UpdateState;
            UpdateState(_effect.State);
        }

        private void OnDisable()
        {
            if (_effect != null)
                _effect.OnStateChanged -= UpdateState;
        }

        private void UpdateState(ShowHideState state) => _state.text = state.ToString();
    }
}
