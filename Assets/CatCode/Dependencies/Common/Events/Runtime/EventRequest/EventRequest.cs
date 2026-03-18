using System;

namespace CatCode.Common
{
    public sealed class EventRequest : IEventRequest
    {
        private bool _isRequested;
        public bool IsRequested => _isRequested;

        public event Action Requested;

        public void Request()
        {
            if (_isRequested)
                return;

            _isRequested = true;
            Requested?.Invoke();
        }

        public void Reset()
        {
            _isRequested = false;
        }
    }
}