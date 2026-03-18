using System;

namespace CatCode.Common
{
    public interface IReadOnlyEventRequest
    {
        bool IsRequested { get; }
        event Action Requested;
    }
}