using Gameplay;
using Gameplay.BaseItem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fur_AttackObj : MonoBehaviour
{
    public int attackDamage = 10; // 攻击伤害

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // 本次攻击已击中的敌人

    private void OnEnable()
    {
        // 每次启用时清空，确保新一轮攻击能重新命中敌人
        hitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 用 Tag 判断
        {
            // 如果已经打中过，就不再重复攻击（可注释此段表示同一个敌人可多次被打）
            if (hitEnemies.Contains(other.gameObject)) return;

            hitEnemies.Add(other.gameObject);

            // 示例：调用敌人脚本的受伤函数
            BaseMonster enemy = other.GetComponent<BaseMonster>();
            if (enemy != null)
            {
                enemy.Injured(attackDamage);
                Fur_Base fur_Base = transform.parent.GetComponent<Fur_Base>();
                MonsterMgr.Instance.MonsterRePell(transform.parent, fur_Base);
                Debug.Log($"击中 {other.name}，造成 {attackDamage} 点伤害");
            }
        }
    }
}
