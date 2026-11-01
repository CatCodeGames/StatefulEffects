using System;

namespace CatCode.StatefulEffects.Tests
{
    public enum Policy
    {
        Default,
        ShowOnInterruptIgnore,
        HideOnInterruptIgnore,
        ShowOnRepeatRestart,
        HideOnRepeatRestart
    }

    public static class Policies
    {
        public static ShowHidePolicy Get(Policy policy)
        {
            return policy switch
            {
                Policy.Default => ShowHidePolicy.Default,
                Policy.ShowOnInterruptIgnore => new ShowHidePolicy { Show = new TransitionPolicy { OnInterrupt = InterruptResolution.Ignore } },
                Policy.HideOnInterruptIgnore => new ShowHidePolicy { Hide = new TransitionPolicy { OnInterrupt = InterruptResolution.Ignore } },
                Policy.ShowOnRepeatRestart => new ShowHidePolicy { Show = new TransitionPolicy { OnRepeat = RepeatResolution.Restart } },
                Policy.HideOnRepeatRestart => new ShowHidePolicy { Hide = new TransitionPolicy { OnRepeat = RepeatResolution.Restart } },
                _ => throw new ArgumentOutOfRangeException(nameof(policy), policy, null),
            };
        }
    }
}
