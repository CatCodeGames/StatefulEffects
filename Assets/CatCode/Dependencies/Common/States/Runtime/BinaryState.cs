using System;

namespace CatCode.Common
{
    public sealed class BinaryState
    {
        private readonly EventValue<bool> _isActive = new(false);

        private readonly Action _onActivate;
        private readonly Action _onDeactivate;

        public BinaryState(Action onActivate, Action onDeactivate)
        {
            _onActivate = onActivate;
            _onDeactivate = onDeactivate;
        }

        public EventValue<bool> IsActive
            => _isActive;

        public void Activate()
        {
            if (_isActive.Value)
                return;
            _onActivate?.Invoke();
            _isActive.Value = true;
        }

        public void Deactivate()
        {
            if (!_isActive.Value)
                return;
            _onDeactivate?.Invoke();
            _isActive.Value = false;
        }
    }
}