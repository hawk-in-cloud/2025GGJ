using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
public class RemoveAfterAnim : StateMachineBehaviour
{
    // OnStateExit �ڶ���״̬�˳�ʱ������
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PoolManager.Instance.PushObj(animator.gameObject);
    }
}
