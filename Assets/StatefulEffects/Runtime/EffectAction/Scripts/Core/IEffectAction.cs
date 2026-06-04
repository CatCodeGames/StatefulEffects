using CatCode.Events;

namespace CatCode.StatefulEffects
{

    public interface IEffectAction
    {
        bool IsRunning { get; }
        IReadOnlyEventValue<EffectActionState> State { get; }
        void Play();
        void Prepare();
        void Stop();
    }
}
