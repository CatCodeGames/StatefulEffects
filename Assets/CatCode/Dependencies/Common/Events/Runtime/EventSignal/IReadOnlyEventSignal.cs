using System;

namespace CatCode.Common
{
    public interface IReadOnlyEventSignal
    {
        event Action Raised;
    }
}