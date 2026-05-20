using CatCode.Events;
using CatCode.StatefulEffects;
using DG.Tweening;
using System;
using UnityEngine;

public sealed class DemoShowHide : MonoShowHide
{
    private Tween _tween;
    private IShowHide _showHide;

    [SerializeField] private ShowHideState _initialState;
    [SerializeField] private bool _ignoreState;

    [SerializeField] private Transform _target;
    [SerializeField] private ParticleSystem _hideParticles;

    private void Awake()
    {
        _showHide = new ShowHideCallbackStateMachine(
            _initialState,
            _ignoreState,
            OnShow,
            OnHide,
            OnSetShown,
            OnSetHidden,
            OnStop);
    }

    private void OnShow(Action onCompleted)
    {
        _tween.Kill();
        _target.localScale = new Vector3(1f, 0f, 1f);
        _tween = DOTween.Sequence()
            .Append(_target.DOScale(new Vector3(0.5f, 1.5f, 0.5f), 0.2f).SetEase(Ease.OutQuad))
            .Append(_target.DOScale(new Vector3(1.5f, 0.5f, 1.5f), 0.2f).SetEase(Ease.InOutQuad))
            .Append(_target.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad))
            .OnComplete(() => onCompleted());
    }

    private void OnSetShown()
    {
        _tween.Kill();
        _target.localScale = Vector3.one;
    }

    private void OnHide(Action onCompleted)
    {
        _tween.Kill();
        _tween = DOTween.Sequence()
            .Append(_target.DOScale(new Vector3(0.5f, 1.5f, 0.5f), 0.2f).SetEase(Ease.OutQuad))
            .Append(_target.DOScale(new Vector3(1.5f, 0.0f, 1.5f), 0.2f).SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                _target.localScale = Vector3.zero;
                _hideParticles.Play();
                onCompleted();
            });
    }

    private void OnSetHidden()
    {
        _tween.Kill();
        _target.localScale = Vector3.zero;
    }

    private void OnStop()
    {
        _tween.Kill();
    }

    #region IShowHide 

    public override IReadOnlyEventValue<ShowHideState> State => _showHide.State;

    public override void Show() => _showHide.Show();
    public override void SetShown() => _showHide.SetShown();
    public override void Hide() => _showHide.Hide();
    public override void SetHidden() => _showHide.SetHidden();
    public override void Stop() => _showHide.Stop();

    #endregion
}
