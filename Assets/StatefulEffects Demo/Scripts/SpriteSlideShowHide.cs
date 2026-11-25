using System;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace CatCode.StatefulEffects.Demo
{
    public sealed class FallingStarsShowHide : CallbackShowHide
    {
        private static readonly Vector3 HiddenScale = new(0.2f, 0.2f, 1f);
        private static readonly Vector3 StretchScale = new(0.6f, 1.0f, 1f);
        private static readonly Vector3 SquashScale = new(1f, .6f, 1f);
        private static readonly Vector3 ShownScale = new(0.8f, 0.8f, 1f);

        [Serializable]
        private struct StarItem
        {
            public SpriteRenderer Renderer;
            public float DropHeight;

            [NonSerialized] public Vector3 Top;
            public readonly Vector3 Bottom => Top + Vector3.down * DropHeight;
        }

        [SerializeField] private StarItem[] _items;
        private MotionHandle _motion;

        protected override void OnInitialize()
        {
            for (int i = 0; i < _items.Length; i++)
                _items[i].Top = _items[i].Renderer.transform.localPosition;
        }

        protected override void OnShow(ShowHideState from, Action onCompleted)
        {
            _motion.TryCancel();
            if (_items.Length == 0)
            {
                onCompleted?.Invoke();
                return;
            }

            var totalSeq = LSequence.Create();

            for (int i = 0; i < _items.Length; i++)
            {
                ref readonly var item = ref _items[i];
                var tr = item.Renderer.transform;
                var overshoot = item.Bottom + Vector3.down * 0.1f;
                float delay = i * 0.05f;

                var rebound = LMotion.Create(overshoot, item.Bottom, 0.20f)
                    .WithEase(Ease.OutSine)
                    .WithImmediateBind(false);

                var move = LSequence.Create()
                    .Append(LMotion.Create(item.Top, overshoot, 0.30f).WithEase(Ease.InQuad).BindToLocalPosition(tr))
                    .Append(rebound.BindToLocalPosition(tr))
                    .Run();

                var scale = LSequence.Create()
                    .Append(LMotion.Create(HiddenScale, StretchScale, 0.30f).WithEase(Ease.OutQuad).BindToLocalScale(tr))
                    .Append(LMotion.Create(StretchScale, SquashScale, 0.10f).WithEase(Ease.InOutQuad).WithImmediateBind(false).BindToLocalScale(tr))
                    .Append(LMotion.Create(SquashScale, ShownScale, 0.10f).WithEase(Ease.OutBack).WithImmediateBind(false).BindToLocalScale(tr))
                    .Run();

                var fade = LMotion.Create(0f, 1f, 0.20f)
                    .WithEase(Ease.OutQuad)
                    .BindToColorA(item.Renderer);

                var seq = LSequence.Create()
                    .Join(move)
                    .Join(scale)
                    .Join(fade);

                totalSeq.Insert(delay, seq.Run());
            }

            _motion = LSequence.Create()
                .Append(totalSeq.Run())
                .Append(LMotion.Create(0f, 0f, 0.001f).WithOnComplete(onCompleted).RunWithoutBinding())
                .Run();
        }

        protected override void OnHide(ShowHideState from, Action onCompleted)
        {
            _motion.TryCancel();
            if (_items.Length == 0)
            {
                onCompleted?.Invoke();
                return;
            }

            var totalSeq = LSequence.Create();

            for (int i = 0; i < _items.Length; i++)
            {
                ref readonly var item = ref _items[i];
                var tr = item.Renderer.transform;
                float delay = i * 0.05f;

                var move = LMotion.Create(item.Bottom, item.Top, 0.30f)
                    .WithEase(Ease.InBack)
                    .BindToLocalPosition(tr);

                var scale = LSequence.Create()
                    .Append(LMotion.Create(ShownScale, StretchScale, 0.10f).WithEase(Ease.OutQuad).BindToLocalScale(tr))
                    .Append(LMotion.Create(StretchScale, HiddenScale, 0.20f).WithEase(Ease.InQuad).WithImmediateBind(false).BindToLocalScale(tr))
                    .Run();

                var fade = LSequence.Create()
                    .AppendInterval(0.05f)
                    .Append(LMotion.Create(1f, 0f, 0.25f).WithEase(Ease.InQuad).WithImmediateBind(false).BindToColorA(item.Renderer))
                    .Run();

                var seq = LSequence.Create()
                  .Join(move)
                  .Join(scale)
                  .Join(fade);

                totalSeq.Insert(delay, seq.Run());
            }

            _motion = LSequence.Create()
                .Append(totalSeq.Run())
                .Append(LMotion.Create(0f, 0f, 0.001f).WithOnComplete(onCompleted).RunWithoutBinding())
                .Run();


        }

        protected override void OnSetShown(ShowHideState from)
        {
            _motion.TryCancel();
            for (int i = 0; i < _items.Length; i++)
                ApplyState(in _items[i], _items[i].Bottom, ShownScale, 1f);
        }

        protected override void OnSetHidden(ShowHideState from)
        {
            _motion.TryCancel();
            for (int i = 0; i < _items.Length; i++)
                ApplyState(in _items[i], _items[i].Top, HiddenScale, 0f);
        }

        protected override void OnStop(ShowHideState from) => _motion.TryCancel();

        private static void ApplyState(in StarItem item, Vector3 pos, Vector3 scale, float alpha)
        {
            var tr = item.Renderer.transform;
            tr.localPosition = pos;
            tr.localScale = scale;

            var color = item.Renderer.color;
            color.a = alpha;
            item.Renderer.color = color;
        }
    }
}