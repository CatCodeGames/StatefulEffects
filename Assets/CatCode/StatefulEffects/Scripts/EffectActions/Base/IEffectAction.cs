using System;

namespace CatCode.EffectActions
{
    public interface IEffectAction
    { 
        EffectActionState State { get; }
        event Action<EffectActionState> StateChanged;
        void Play();
        void Stop();
    }
}