using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void OnStateEnter();
    void OnStateLogicUpdate();
    void OnStateFixedUpdate();
    void OnStateExit();
}
