using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dev
{
    public class AnimBehaviour : StateMachineBehaviour
    {
        public event Action<AnimatorStateInfo, int> EventStateExit;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EventStateExit?.Invoke(stateInfo, layerIndex);
        }
    }
}