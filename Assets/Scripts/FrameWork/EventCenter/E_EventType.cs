using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EventType
{
    E_Monster_Dead,
    E_Monster_Generate,
    E_GameItem_Dead,
    E_GameItem_Generate,
    E_Player_GetRewa,
    E_Test,

    // Input
    E_Input_J,
    E_Input_K,
    E_Input_L,

    // Spawn
    E_Spawn,
    E_Destroy,

    // beat
    E_Beat_Success,
    E_Beat_Failure,

    // exp
    E_Exp_GetExp,
    E_Exp_EnterLevelUp,
    E_Exp_EndLevelUp,
}