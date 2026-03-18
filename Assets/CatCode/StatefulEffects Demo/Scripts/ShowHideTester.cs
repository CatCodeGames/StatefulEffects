using CatCode.StatefulEffects;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShowHideTester : MonoBehaviour
{
    [SerializeField] private MonoShowHide[] _effects;
    [Space]
    [SerializeField] private Button _showButton;
    [SerializeField] private Button _hideButton;

    private void Start()
    {
        for (int i = 0; i < _effects.Length; i++)
            _effects[i].SetHidden();
    }

    private void OnEnable()
    {
        _showButton.onClick.AddListener(ShowEffect);
        _hideButton.onClick.AddListener(HideEffects);
    }

    private void OnDisable()
    {
        _showButton.onClick.RemoveListener(ShowEffect);
        _hideButton.onClick.RemoveListener(HideEffects);
    }

    private void ShowEffect()
    {
        for (int i = 0; i < _effects.Length; i++)
            _effects[i].Show();
    }

    private void HideEffects()
    {
        for (int i = 0; i < _effects.Length; i++)
            _effects[i].Hide();
    }
}