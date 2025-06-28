using System.Collections.Generic;
using Framework;
using UnityEngine;

namespace Gameplay.BaseItem
{
    public abstract class BaseProps : MonoBehaviour
    {
        [Header("道具属性")]
        //生命值 
        private int _health;
        //攻击力
        private int _attack;
        
        [Header("组件获取")]
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            //注册道具生成事件
            EventManager.Instance.EventTrigger(E_EventType.E_GameItem_Generate);
            Init();
        }

        //初始化方法
        protected abstract void Init();
        
        //--这个函数是用音符按下后调用的
        /// <summary>
        /// 道具物体攻击怪物的函数
        /// </summary>
        private void AttackMonsters()
        {
            
            //生成预设体来攻击怪物
            
            /*foreach (var monster in monsters)
            {
                //调用monster的受伤函数
                monster.Injured(_attack);
            }*/
        }

        /// <summary>
        /// 道具物体受到伤害的函数
        /// </summary>
        /// <param name="damage">伤害值</param>
        public void Injured(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                //播放死亡动画
                _anim.SetBool("Dead", true);
                return;
            }
            //播放受伤动画
            _anim.SetTrigger("Injured");
        }

        //游戏物体的死亡函数
        public void Die()
        {
            //移除事件监听
            EventManager.Instance.RemoveEventListener(E_EventType.E_GameItem_Dead, () =>
            {
                EventManager.Instance.EventTrigger(E_EventType.E_GameItem_Dead);
            });
        }
    }
}