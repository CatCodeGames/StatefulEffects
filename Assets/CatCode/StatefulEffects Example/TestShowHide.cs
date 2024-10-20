using CatCode.StatefulEffects;
using System;
using System.Collections;
using UnityEngine;

public sealed class TestShowHide : MonoShowHide
{
    private Coroutine _coroutine;

    [SerializeField] private float _currentValue = 1f;
    [Space]
    [SerializeField] private float _minValue = 0f;
    [SerializeField] private float _maxValue = 1f;
    [SerializeField] private float _duration = 1f;

    protected override void OnShow(Action onCompleted)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ScaleRoutine(_maxValue, onCompleted));
    }

    protected override void OnHide(Action onCompleted)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ScaleRoutine(_minValue, onCompleted));
    }

    protected override void OnSetHidden()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _currentValue = _minValue;
        transform.localScale = _currentValue * Vector3.one;
    }

    protected override void OnSetShown()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _currentValue = _maxValue;
        transform.localScale = _maxValue * Vector3.one;
    }

    protected override void OnStop()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private IEnumerator ScaleRoutine(float target, Action callback)
    {
        var duration = _duration * Mathf.Abs((target - _currentValue) / (_maxValue - _minValue));
        var elapsedTime = 0f;
        var start = _currentValue;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _currentValue = Mathf.Lerp(start, target, elapsedTime / duration);
            transform.localScale = _currentValue * Vector3.one;
            yield return null;
        }

        transform.localScale = target * Vector3.one;
        callback?.Invoke();
    }
}
