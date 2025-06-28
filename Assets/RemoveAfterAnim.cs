using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
public class RemoveAfterAnim : StateMachineBehaviour
{
    // OnStateExit 在动画状态退出时被调用
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PoolManager.Instance.PushObj(animator.gameObject);
    }
}
