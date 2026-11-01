using CatCode.StatefulEffects;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShowHideTester : MonoBehaviour
{
    private CancellationTokenSource _cts;

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
        _showButton.onClick.AddListener(ShowEffectsAsync);
        _hideButton.onClick.AddListener(HideEffectsAsync);
    }

    private void OnDisable()
    {
        _showButton.onClick.RemoveListener(ShowEffectsAsync);
        _hideButton.onClick.RemoveListener(HideEffectsAsync);
    }

    private async void ShowEffectsAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        try
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                var result = await _effects[i].ShowAsync(_cts.Token);
                Debug.Log($"{_effects[i].name}: Show {result}");
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Show cancelled");
        }
    }

    private async void HideEffectsAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        try
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                var result = await _effects[i].HideAsync(_cts.Token);
                Debug.Log($"{_effects[i].name}: Hide {result}");
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Hide cancelled");
        }
    }
}
