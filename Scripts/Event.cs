using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECFSM
{
    // 动画事件转发：把动画事件转给当前状态的 onAnimationEvent
    // （方法名 OnFootstep/OnLand 被动画剪辑引用，别改名）
    public class Event : MonoBehaviour
    {
        private Com com;
        private void Awake()
        {
            com = GetComponentInParent<Com>();
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            Relay(animationEvent);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            Relay(animationEvent);
        }

        private void Relay(AnimationEvent animationEvent)
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
