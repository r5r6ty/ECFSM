using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECFSM
{
    public class Event : MonoBehaviour
    {
        private Com com;
        private void Awake()
        {
            com = GetComponentInParent<Com>();
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (com.data.states.TryGetValue(com.stateno, out State state))
                {
                    state?.onAnimationEvent?.Invoke(com.variables);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (com.data.states.TryGetValue(com.stateno, out State state))
                {
                    state?.onAnimationEvent?.Invoke(com.variables);
                }
            }
        }
    }
}
