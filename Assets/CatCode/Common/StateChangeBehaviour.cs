using System;
using UnityEngine;

namespace CatCode
{
    [SharedBetweenAnimators]
    public class StateChangeBehaviour : StateMachineBehaviour
    {
        public event Action<Animator, AnimatorStateInfo, int> StateExit;
        public event Action<Animator, AnimatorStateInfo, int> StateEnter;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
            => StateEnter.Invoke(animator, animatorStateInfo, layerIndex);

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
            => StateExit.Invoke(animator, animatorStateInfo, layerIndex);
    }            
}