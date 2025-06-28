using Gameplay;
using Gameplay.BaseItem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fur_AttackObj : MonoBehaviour
{
    public int attackDamage = 10; // �����˺�

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // ���ι����ѻ��еĵ���

    private void OnEnable()
    {
        // ÿ������ʱ��գ�ȷ����һ�ֹ������������е���
        hitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // �� Tag �ж�
        {
            // ����Ѿ����й����Ͳ����ظ���������ע�ʹ˶α�ʾͬһ�����˿ɶ�α���
            if (hitEnemies.Contains(other.gameObject)) return;

            hitEnemies.Add(other.gameObject);

            // ʾ�������õ��˽ű������˺���
            BaseMonster enemy = other.GetComponent<BaseMonster>();
            if (enemy != null)
            {
                enemy.Injured(attackDamage);
                Fur_Base fur_Base = transform.parent.GetComponent<Fur_Base>();
                MonsterMgr.Instance.MonsterRePell(transform.parent, fur_Base);
                Debug.Log($"���� {other.name}����� {attackDamage} ���˺�");
            }
        }
    }
}
