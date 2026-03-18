using CatCode.Common;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class MonoShowHide : MonoBehaviour, IShowHide
    {
        public abstract IReadOnlyEventValue<ShowHideState> State { get; }

        public virtual void Initialize() { }

        public abstract void Show();
        public abstract void SetShown();

        public abstract void Hide();
        public abstract void SetHidden();

        public abstract void Stop();

#if UNITY_EDITOR
        [ContextMenu("Show")]
        private void TestShow() => Show();

        [ContextMenu("Hide")]
        private void TestHide() => Hide();

        [ContextMenu("SetShown")]
        private void TestSetShown() => SetShown();

        [ContextMenu("SetHidden")]
        private void TestSetHidden() => SetHidden();

        [ContextMenu("Stop")]
        private void TestStop() => Stop();
#endif
    }
}
