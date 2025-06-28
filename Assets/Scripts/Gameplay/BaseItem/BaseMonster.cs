using Framework;
using UnityEngine;

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
        
        [Header("获得组件")]
        private Animator _anim;
        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }
        
        protected void Start()
        {
            //注册事件监听 -- 怪物的生成注册事件
            EventManager.Instance.EventTrigger(E_EventType.E_Monster_Generate);
            Init();
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

        /// <summary>
        /// 攻击函数 -- 让外部调用
        /// </summary>
        public void Attack(BaseProps prop)
        {
            //调用props的Injured函数
            prop.Injured(attack);
        }
        
        //死亡函数
        public void Die()
        {
            //触发死亡事件
            EventManager.Instance.EventTrigger(E_EventType.E_Monster_Dead);
            //进入对象池
            PoolManager.Instance.PushObj(gameObject);
        }
    }
}