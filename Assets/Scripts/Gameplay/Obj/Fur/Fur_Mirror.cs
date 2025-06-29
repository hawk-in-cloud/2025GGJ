using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fur_Mirror : Fur_Base
{
    public GameObject ShootLight; // ÉäÏß·¢ÉäÆ÷
    private void Start()
    {
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, Beat_Ta);
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, Beat_Tan);
        ShootLight = transform.Find("ShootLight").gameObject;
    }

    
    public void Beat_Ta(E_RatatanType type)
    {
        if (!isActive || type != E_RatatanType.Ta)
            return;

        Fur_AttackObj fur_AttackObj = transform.Find("ShootLight").Find("StellarisBlue").GetComponent<Fur_AttackObj>();
        fur_AttackObj.SetAttackDamage(attack);

        ShootLight.SetActive(true);
        LineShoot lineShoot = transform.Find("ShootLight").GetComponent<LineShoot>();
        lineShoot.StartLineShoot();
        // ²¥·Å¹¥»÷¶¯»­
        _anim.Play("Attack", 0, 0.0f);
        AudioManager.Instance.PlaySound("sfx_laser");
    }

    public void Beat_Tan(E_RatatanType type)
    {
        if (!isActive || type != E_RatatanType.Tan)
            return;

        ShootLight.SetActive(true);
        LineShoot lineShoot = transform.Find("ShootLight").GetComponent<LineShoot>();
        lineShoot.StartLineShoot();
        // ²¥·Å¹¥»÷¶¯»­
        _anim.Play("Attack", 0, 0.0f);
        AudioManager.Instance.PlaySound("sfx_laser");
    }
}
