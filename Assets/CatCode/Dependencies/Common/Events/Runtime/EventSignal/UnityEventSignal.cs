using System;
using UnityEngine.Events;

namespace CatCode.Common
{
    public sealed class UnityEventSignal : IReadOnlyEventSignal, IDisposable
    {
        private readonly UnityEvent _unityEvent;
        private bool _isActive;

        public UnityEventSignal(UnityEvent unityEvent)
        {
            _unityEvent = unityEvent;            
        }

        public event Action Raised;

        public void Activate()
        {
            if (_isActive)
                return;
            _isActive = true;
            _unityEvent?.AddListener(EventHandler);
        }

        public void Deactivate()
        {
            if (!_isActive)
                return;
            _isActive = false;
            _unityEvent?.RemoveListener(EventHandler);
        }

        public void Dispose()
            => Deactivate();

        private void EventHandler()
            => Raised?.Invoke();
    }
}