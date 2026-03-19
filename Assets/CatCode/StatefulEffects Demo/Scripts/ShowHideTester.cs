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

        //_showButton.onClick.AddListener(ShowEffect);
        //_hideButton.onClick.AddListener(HideEffects);
    }

    private void OnDisable()
    {
        _showButton.onClick.RemoveListener(ShowEffectsAsync);
        _hideButton.onClick.RemoveListener(HideEffectsAsync);

        //_showButton.onClick.RemoveListener(ShowEffect);
        //_hideButton.onClick.RemoveListener(HideEffects);
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


    private async void HideEffectsAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        try
        {
            for (int i = 0; i < _effects.Length; i++)
                await _effects[i].HideAsync(_cts.Token);
            Debug.Log("Hidden");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Cancelled");
        }
    }

    private async void ShowEffectsAsync()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();

        try
        {
            for (int i = 0; i < _effects.Length; i++)
                await _effects[i].ShowAsync(_cts.Token);
            Debug.Log("SHOWN");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Cancelled");
        }
    }
}