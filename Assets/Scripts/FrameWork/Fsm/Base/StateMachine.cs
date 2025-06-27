using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private IState currentState;
    public IState CurrentState
    {
        get { return currentState; }
    }
    /// <summary>
    /// 进入默认状态
    /// </summary>
    public void EnterDefalutState(IState newState)
    {
        currentState = newState;
        currentState.OnStateEnter();
    }
    /// <summary>
    /// 更新状态
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IState newState)
    {
        if (currentState == null)
            return;
        currentState.OnStateExit();
        currentState = newState;
        currentState.OnStateEnter();
    }
    /// <summary>
    ///  处理玩法GamePlay逻辑
    /// </summary>
    public void Update()
    {
        if (currentState == null)
            return;
        currentState.OnStateLogicUpdate();
    }
    /// <summary>
    /// 处理物理Physics逻辑
    /// </summary>
    public void FixedUpdate()
    {
        if (currentState == null)
            return;
        currentState.OnStateFixedUpdate();
    }
}
