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
    /// ����Ĭ��״̬
    /// </summary>
    public void EnterDefalutState(IState newState)
    {
        currentState = newState;
        currentState.OnStateEnter();
    }
    /// <summary>
    /// ����״̬
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
    ///  �����淨GamePlay�߼�
    /// </summary>
    public void Update()
    {
        if (currentState == null)
            return;
        currentState.OnStateLogicUpdate();
    }
    /// <summary>
    /// ��������Physics�߼�
    /// </summary>
    public void FixedUpdate()
    {
        if (currentState == null)
            return;
        currentState.OnStateFixedUpdate();
    }
}
