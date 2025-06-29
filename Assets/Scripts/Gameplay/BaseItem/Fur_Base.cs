using System;
using Framework;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.BaseItem;
using Unity.VisualScripting;
using UnityEngine;
public class Fur_Base : MonoBehaviour
{
    [Header("是不是激活")]
    public bool isActive = false;
    
    [Header("缩放")]
    public float scaleFactor = 1.2f;
    public float scaleSpeed = 5f;
    
    [Header("物体属性")]
    //生命值
    public int health = 100;
    //攻击力
    public int attack;
    //攻击间隔
    public float attackInterval = 1f;
    public bool canAttack = true;
    private float _tempAttackTime;
    
    Vector3 _originalScale; // 原始大小
    
    // 引用组件
    private Animator _anim;
    private SpriteRenderer sp;
    private BoxCollider2D _col;
    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _col = GetComponent<BoxCollider2D>();
        _originalScale = transform.localScale;
        isActive = false;
        sp.color = Color.gray;
        
        EventManager.Instance.EventTrigger(E_EventType.E_GameItem_Generate);
    }

    protected virtual void Start()
    {
        if (isActive)
        {
            //启用碰撞体
            _col.enabled = true;
        }
        else
        {
            _col.enabled = false;
        }
    }

    private void Update()
    {
        OnHover();

        if (!isActive)
            return;
        ActiveLogic();
    }
    
    // 当物品被激活时的逻辑处理
    public void ActiveLogic()
    {
        
    }
    
    // 处理鼠标悬停时的逻辑
    public void OnHover()
    {
        // 
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (LevelManager.Instance.isLevelUp)
            Debug.Log($"{GetComponent<Collider2D>() != null}" +
                $"{GetComponent<Collider2D>().bounds.Contains(mousePosition)}");

        // 
        if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().bounds.Contains(mousePosition) && LevelManager.Instance.isLevelUp)
        {
            // 
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale * scaleFactor, Time.unscaledDeltaTime * scaleSpeed);
            if (Input.GetMouseButtonDown(0))
            {
                this.isActive = true;
                sp.color = Color.white;
                EventManager.Instance.EventTrigger(E_EventType.E_Exp_EndLevelUp);
            }
        }
        else
        {
            // 
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, Time.unscaledDeltaTime * scaleSpeed);
        }
    }


    /// <summary>
    /// 攻击函数
    /// </summary>
    public void Attack()
    {
       
    }
    
    /// <summary>
    /// 受伤函数
    /// </summary>
    /// <param name="damage"></param>
    public void Injured(BaseMonster monster,int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            _anim.SetBool("Dead", true);
          
            //失活碰撞体
            _col.enabled = false;
            
            var deadTime = MonsterMgr.Instance.GetAnimLength(_anim,"Dead");
            Invoke("Die", deadTime);
            monster.target = MonsterMgr.Instance.ReturnNearestProps(monster);
            return;
        }
        _anim.SetTrigger("Injured");
    }
    
    // 处理物品死亡时的逻辑
    public void Die()
    {
        //触发死亡事件
        EventManager.Instance.EventTrigger(E_EventType.E_GameItem_Dead);
        
        PoolManager.Instance.PushObj(gameObject);
    }
}
