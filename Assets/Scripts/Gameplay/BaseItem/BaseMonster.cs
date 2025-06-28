using System;
using Framework;
using Gameplay.Obj;
//using Gameplay.Obj.Fur;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.BaseItem
{
    public abstract class BaseMonster : MonoBehaviour
    {
        [Header("怪物的基本属性")]
        //怪物的生命值
        public int health;
        //怪物的攻击力
        public int attack;
        //怪物的移速
        public float speed;
        //怪物的攻击间隔
        public float attackInterval;
        
        [Header("获得组件")]
        private Animator _anim;
        private float _attackTempTime;
        
        [Header("寻路设置")]
        public Transform target;
        //移速
        private float moveSpeed;
        public float detectionRange = 1f;
        public LayerMask obstacleMask;
        
        [Header("追击停止距离")]
        public float stopDistance = 1f;

        [Header("卡住处理")]
        public float maxStuckTime = 1.2f;
        public float escapeAngle = 135f;
        public float escapeBoost = 1.5f;
        //寻路相关变量
        private Rigidbody2D rb;
        private Vector2 lastPosition;
        private float stuckTimer = 0f;
        
        //私有变量
        private bool _canAttack;
        
        private readonly float[] checkAngles = { 0f, 25f, -25f, 90f, -90f };

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }
        
        protected void Start()
        {
            //寻路目标初始化
            if (target == null)
            {
                //随机寻路目标
                target = MonsterMgr.Instance.ReturnNearestProps(this);
            }
            //设置速度
            moveSpeed = speed;
            lastPosition = rb.position;
            
            //触发事件 -- 怪物的生成注册事件
            EventManager.Instance.EventTrigger<BaseMonster>(E_EventType.E_Monster_Generate, this);
            Init();
        }

        protected virtual void Update()
        {
            if (_canAttack)
                return;
            
            _attackTempTime += Time.deltaTime;
            if (_attackTempTime >= attackInterval)
            {
                _attackTempTime = 0;
                _canAttack = true;
            }
        }

        //初始化函数
        protected abstract void Init();

        /// <summary>
        /// 收到伤害函数 -- 让外部调用
        /// </summary>
        /// <param name="damage"></param>
        public void Injured(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                //播放死亡动画
                _anim.SetBool("Dead", true);
                //计算动画播放时间
                var deadTime = MonsterMgr.Instance.GetAnimLength(_anim,"Dead");
                //延迟执行死亡函数
                Invoke("Die", deadTime);
                return;
            }
            //播放受伤动画
            _anim.SetTrigger("Injured");
        }
        
        //死亡函数
        public void Die()
        {
            //触发死亡事件
            EventManager.Instance.EventTrigger(E_EventType.E_Monster_Dead);
            //进入对象池
            PoolManager.Instance.PushObj(gameObject);
        }
        void FixedUpdate()
        {
            if (target == null) return;
            float distanceToTarget = Vector2.Distance(target.position, transform.position);
            if (distanceToTarget < stopDistance)
            {
                // 靠太近了，不移动，只可攻击
                rb.velocity = Vector2.zero;
                return;
            }


            Vector2 baseDirection = (target.position - transform.position).normalized;
            Vector2 moveDir = Vector2.zero;

            // === 1. 判断是否卡住 ===
            float movedDist = Vector2.Distance(rb.position, lastPosition);
            if (movedDist < 0.03f)
                stuckTimer += Time.fixedDeltaTime;
            else
                stuckTimer = 0f;

            lastPosition = rb.position;

            if (stuckTimer >= maxStuckTime)
            {
                // === 优先执行脱困 ===
                float sign = Random.value > 0.5f ? 1f : -1f;
                moveDir = Quaternion.Euler(0, 0, escapeAngle * sign) * baseDirection;
                moveDir *= escapeBoost;
                stuckTimer = 0f;

                Debug.Log($"{name} 卡住了，强制脱困");
            }
            else
            {
                // === 2. 正常路径检测 ===
                foreach (float angle in checkAngles)
                {
                    Vector2 testDir = Quaternion.Euler(0, 0, angle) * baseDirection;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, testDir, detectionRange, obstacleMask);

                    if (!hit.collider || hit.collider.gameObject == gameObject)
                    {
                        moveDir = testDir;
                        break;
                    }
                }

                if (moveDir == Vector2.zero)
                {
                    // 所有方向被挡，也可以加入默认向玩家靠近
                    moveDir = baseDirection * 0.2f;
                }
            }

            // === 移动 ===
            rb.MovePosition(rb.position + moveDir.normalized * moveSpeed * Time.fixedDeltaTime);
            // === 朝向翻转 ===
            if (moveDir.x != 0)
            {
                Vector3 localScale = transform.localScale;
                localScale.x = Mathf.Abs(localScale.x) * (moveDir.x > 0 ? -1 : 1);
                transform.localScale = localScale;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (target == null) return;
            Vector2 baseDir = (target.position - transform.position).normalized;
            foreach (float angle in checkAngles)
            {
                Vector2 testDir = Quaternion.Euler(0, 0, angle) * baseDir;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + (Vector3)(testDir * detectionRange));
            }
        }
        private void OnCollisionStay2D(Collision2D other)
        {
            
            if (other.gameObject.CompareTag("Player"))
            {
                if (!_canAttack) return;
                _canAttack = false;
                _attackTempTime = 0;
                //小女孩本质也是fur，和fur逻辑处理一致
                if (other.gameObject.GetComponent<Fur_Base>() != null &&
                    other.gameObject.GetComponent<Fur_Base>().isActive)
                    other.gameObject.GetComponent<Fur_Base>().Injured(this, attack);
                if (other.gameObject.GetComponent<Girl>()!= null)
                {
                    other.gameObject.GetComponent<Girl>().Injured(this);
                }
            }
        }
    }
}