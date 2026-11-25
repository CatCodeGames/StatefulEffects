#nullable enable

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CatCode.StatefulEffects.Demo
{
    public sealed class TechnicalDemo : CallbackShowHide
    {
        public enum Mode { InSequence, AllTogether }

        [SerializeField] private MonoShowHide[] _effects = Array.Empty<MonoShowHide>();

        private CancellationTokenSource? _cts;

        public Mode LaunchMode { get; set; }

        protected override void OnDestroy() 
        {
            base.OnDestroy();
            Cancel();
        }

        protected override void OnShow(ShowHideState from, Action onCompleted) =>
            RunAsync(show: true, onCompleted).Forget();

        protected override void OnHide(ShowHideState from, Action onCompleted) =>
            RunAsync(show: false, onCompleted).Forget();

        protected override void OnSetShown(ShowHideState from)
        {
            Cancel();
            foreach (var effect in _effects)
                effect.SetShown();
        }

        protected override void OnSetHidden(ShowHideState from)
        {
            Cancel();
            foreach (var effect in _effects)
                effect.SetHidden();
        }

        protected override void OnStop(ShowHideState from)
        {
            Cancel();
            foreach (var effect in _effects)
                effect.Stop();
        }

        private async UniTaskVoid RunAsync(bool show, Action onCompleted)
        {
            Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                if (LaunchMode == Mode.InSequence)
                {
                    foreach (var effect in _effects)
                        await (show ? effect.ShowAsync(token) : effect.HideAsync(token));
                }
                else
                {
                    var tasks = new UniTask<ShowHideResult>[_effects.Length];
                    for (int i = 0; i < _effects.Length; i++)
                        tasks[i] = show ? _effects[i].ShowAsync(token) : _effects[i].HideAsync(token);

                    await UniTask.WhenAll(tasks);
                }

                onCompleted();
            }
            catch (OperationCanceledException) { }
        }

        private void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }
    }
}