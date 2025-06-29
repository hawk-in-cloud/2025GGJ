using Gameplay.BaseItem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var monster = other.GetComponent<BaseMonster>();
            if (monster != null)
            {
                monster.Injured(damage);
                Debug.Log($"{transform.name} 击中 {other.name}，造成 {damage} 点伤害");
            }

            Destroy(gameObject); // 击中就销毁
        }
    }
}
