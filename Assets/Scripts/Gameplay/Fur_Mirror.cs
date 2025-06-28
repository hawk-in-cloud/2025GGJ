using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fur_Mirror : Fur_Base
{
    private void Start()
    {
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, Beat_Ta);
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, Beat_Tan);
    }

    public void Beat_Ta(E_RatatanType type)
    {
        if (!isActive || type != E_RatatanType.Ta)
            return;

        // ²¥·Å¹¥»÷¶¯»­
        _anim.Play("Attack", 0, 0.0f);
    }

    public void Beat_Tan(E_RatatanType type)
    {
        if (!isActive || type != E_RatatanType.Tan)
            return;

        // ²¥·Å¹¥»÷¶¯»­
        _anim.Play("Attack", 0, 0.0f);
    }
}
