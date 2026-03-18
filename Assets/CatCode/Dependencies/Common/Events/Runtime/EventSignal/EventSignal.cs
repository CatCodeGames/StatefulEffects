using System;

namespace CatCode.Common
{
    public sealed class EventSignal : IEventSignal
    {
        public event Action Raised;

        public void Invoke() => Raised?.Invoke();
    }
}