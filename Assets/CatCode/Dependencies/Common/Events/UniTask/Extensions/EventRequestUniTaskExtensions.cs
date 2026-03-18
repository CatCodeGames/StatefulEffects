using CatCode.Common;
using CatCode.Promises;
using Cysharp.Threading.Tasks;

namespace System.Threading
{
    public static class EventRequestUniTaskExtensions
    {
        public static UniTask WaitAsync(this IReadOnlyEventRequest eventRequest, CancellationToken cancellationToken)
        {
            var taskSource = EventRequestPromise.Create(eventRequest, cancellationToken, out var token);
            return new UniTask(taskSource, token);
        }
    }
}