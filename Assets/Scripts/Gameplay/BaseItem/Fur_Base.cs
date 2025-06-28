using DG.Tweening;
using Framework;
using Gameplay;
using Gameplay.BaseItem;
using UnityEngine;
using System.Collections;
public class Fur_Base : MonoBehaviour
{
    [Header("是不是激活")]
    public bool isActive = false;
    
    [Header("缩放")]
    public float scaleFactor = 1.2f;
    public float scaleSpeed = 5f;
    
    [Header("物体属性")]
    //生命值
    public int health;
    //攻击力
    public int attack;
    private float _tempAttackTime;
    public bool CanAttack = true;//判断是否能攻击
    public float AttackInterval = 0.3f;//判断是否能攻击
    // 攻击判定子物体（带 Collider2D + AttackCollider 脚本）
    public GameObject AttackColliderObj;
    public Collider2D AttackCollider2D;


    Vector3 originalScale; // 原始大小
    
    // 引用组件
    protected Animator _anim;
    Material _mat;

    private void Awake()
    {
        _mat = GetComponent<SpriteRenderer>().material;
        if(_mat != null)
            Debug.Log(_mat.name.ToString());

        _anim = GetComponent<Animator>();
        CanAttack = true;
        AttackColliderObj = transform.Find("AttackCollider").gameObject;
        AttackCollider2D = AttackColliderObj.GetComponent<Collider2D>();
        originalScale = transform.localScale;
        isActive = false;

        _mat.SetColor("_Color", Color.gray);

        EventManager.Instance.EventTrigger(E_EventType.E_GameItem_Generate);
    }

    protected virtual void Start()
    {
        
    }

    private void Update()
    {
        OnHover();

        if (!isActive)
            return;
    }
    
    
    Tween shakeTween;

    // 处理鼠标悬停时的逻辑
    public void OnHover()
    {
        // 获取鼠标在世界空间的位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 检查鼠标是否在物体上
        if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().bounds.Contains(mousePosition) && LevelManager.Instance.isLevelUp)
        {
            // 鼠标悬停时，放大物体
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleFactor, Time.unscaledDeltaTime * scaleSpeed);

            // 添加摇晃效果（使用DOTween，避免TimeScale的影响）
            if (shakeTween == null || !shakeTween.IsPlaying())
            {
                shakeTween = transform.DOLocalRotate(new Vector3(0, 0, 10f), 0.2f, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true) // 让Tween在不受TimeScale影响的情况下更新
                    .OnKill(() => transform.localRotation = Quaternion.Euler(0, 0, 0)); // 动画结束后重置Z轴角度
            }

            if (Input.GetMouseButtonDown(0))
            {
                this.isActive = true;
                _mat.SetColor("_Color", Color.white);
                LevelManager.Instance.ExitLevelUpMode();
                _anim.Play("Idle");
            }
        }
        else
        {
            // 鼠标离开时，恢复到原始大小
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * scaleSpeed);

            // 停止摇晃动画并重置Z轴旋转
            if (shakeTween != null && shakeTween.IsPlaying())
            {
                shakeTween.Kill(); // 结束动画
                transform.localRotation = Quaternion.Euler(0, 0, 0); // 重置Z轴旋转
            }
        }
    }


    /// <summary>
    /// 攻击函数
    /// </summary>

    // 当物品被激活时的逻辑处理
    public void Attack()
    {
        if (!CanAttack)
        {
            return;
        }
            
        StartCoroutine(DoAttack());
    }

    // 执行攻击的协程
    IEnumerator DoAttack()
    {
        CanAttack = false;

        // 激活攻击判定区域
        AttackColliderObj.SetActive(true);

        // 设置攻击力给子物体脚本
        Fur_AttackObj attackCollider = AttackColliderObj.GetComponent<Fur_AttackObj>();
        if (attackCollider != null)
        {
            attackCollider.SetAttackDamage(attack);
        }

        // 持续短时间
        yield return new WaitForSeconds(0.2f);

        AttackColliderObj.SetActive(false);

        // 等待冷却
        yield return new WaitForSeconds(AttackInterval);
        CanAttack = true;
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
            _anim.Play("Injured",0,0);
            
            //失活碰撞体
            monster.target = MonsterMgr.Instance.ReturnNearestProps(monster);
            Die();
        }
    }
    
    // 处理物品死亡时的逻辑
    public void Die()
    {
        //触发死亡事件
        EventManager.Instance.EventTrigger(E_EventType.E_GameItem_Dead);
        
        PoolManager.Instance.PushObj(gameObject);
    }
}
