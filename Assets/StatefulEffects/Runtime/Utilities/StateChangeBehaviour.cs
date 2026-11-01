using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    public class StateChangeBehaviour : StateMachineBehaviour
    {
        public event Action<Animator, AnimatorStateInfo, int> StateEnter;
        public event Action<Animator, AnimatorStateInfo, int> StateExit;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
            => StateEnter?.Invoke(animator, animatorStateInfo, layerIndex);

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
            => StateExit?.Invoke(animator, animatorStateInfo, layerIndex);
    }
}
