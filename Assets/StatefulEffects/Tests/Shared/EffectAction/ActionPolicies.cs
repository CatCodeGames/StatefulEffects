using System;

namespace CatCode.StatefulEffects.Tests
{
    public enum ActionPolicy
    {
        Default,
        OnRepeatIgnore
    }

    public static class ActionPolicies
    {
        public static EffectActionPolicy Get(ActionPolicy policy)
        {
            return policy switch
            {
                ActionPolicy.Default => EffectActionPolicy.Default,
                ActionPolicy.OnRepeatIgnore => new EffectActionPolicy { OnRepeat = InterruptResolution.Ignore },
                _ => throw new ArgumentOutOfRangeException(nameof(policy), policy, null),
            };
        }
    }
}
