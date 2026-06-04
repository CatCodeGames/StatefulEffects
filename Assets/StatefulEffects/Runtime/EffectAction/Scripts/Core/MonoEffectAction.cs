using CatCode.Events;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public abstract class MonoEffectAction : MonoBehaviour, IEffectAction
    {
        public abstract bool Initialized { get; protected set; }
        public abstract bool IsRunning { get; }
        public abstract IReadOnlyEventValue<EffectActionState> State { get; }

        public abstract void Initialize();
        public abstract void Play();
        public abstract void Prepare();
        public abstract void Stop();

#if UNITY_EDITOR
        [ContextMenu("Play")]
        private void TestPlay() => Play();

        [ContextMenu("Prepare")]
        private void TestPrepare() => Prepare();

        [ContextMenu("Stop")]
        private void TestStop() => Stop();
#endif
    }
}
