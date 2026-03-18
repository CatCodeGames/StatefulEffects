using System;

namespace CatCode.Common
{
    public interface IReadOnlyEventValue<T>
    {
        T Value { get; }
        event Action<T> Changed;
        bool Equals(T value);
    }

    public static class EventValueExtensions
    {
        public static void Add<T>(this IReadOnlyEventValue<T> eventValue, Action<T> action, bool invoke = false)
        {
            eventValue.Changed += action;
            if (invoke)
                action(eventValue.Value);
        }

        public static void Remove<T>(this IReadOnlyEventValue<T> eventValue, Action<T> action)
        {
            eventValue.Changed -= action;
        }
    }
}
