#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatCode.StatefulEffects.Demo
{
    public sealed class TechnicalDemoControls : MonoBehaviour
    {
        [SerializeField] private TechnicalDemo _demo = null!;
        [Space]
        [SerializeField] private Toggle _sequence = null!;
        [SerializeField] private Toggle _together = null!;
        [Space]
        [SerializeField] private Button _show = null!;
        [SerializeField] private Button _hide = null!;
        [SerializeField] private Button _setShown = null!;
        [SerializeField] private Button _setHidden = null!;
        [Space]
        [SerializeField] private TMP_Text _status = null!;

        private void Awake()
        {
            _show.onClick.AddListener(_demo.Show);
            _hide.onClick.AddListener(_demo.Hide);
            _setShown.onClick.AddListener(_demo.SetShown);
            _setHidden.onClick.AddListener(_demo.SetHidden);
            _sequence.onValueChanged.AddListener(selected => SelectMode(selected, TechnicalDemo.Mode.InSequence));
            _together.onValueChanged.AddListener(selected => SelectMode(selected, TechnicalDemo.Mode.AllTogether));
        }

        private void OnEnable()
        {
            _demo.OnStateChanged += Refresh;
            Refresh(_demo.State);
        }

        private void Start() => _demo.SetHidden();

        private void OnDisable()
        {
            if (_demo != null)
                _demo.OnStateChanged -= Refresh;
        }

        private void SelectMode(bool selected, TechnicalDemo.Mode mode)
        {
            if (selected)
                _demo.LaunchMode = mode;

            Refresh(_demo.State);
        }

        private void Refresh(ShowHideState state)
        {
            bool running = state == ShowHideState.Showing || state == ShowHideState.Hiding;
            _sequence.interactable = _together.interactable = !running;
            _sequence.SetIsOnWithoutNotify(_demo.LaunchMode == TechnicalDemo.Mode.InSequence);
            _together.SetIsOnWithoutNotify(_demo.LaunchMode == TechnicalDemo.Mode.AllTogether);

            _show.interactable = state == ShowHideState.Hidden || state == ShowHideState.None;
            _hide.interactable = state == ShowHideState.Shown || state == ShowHideState.None;
            _status.text = state.ToString();
        }
    }
}
