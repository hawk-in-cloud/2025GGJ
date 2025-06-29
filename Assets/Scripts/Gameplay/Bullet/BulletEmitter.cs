using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEmitter : MonoBehaviour
{
    public GameObject bulletPrefab;          // 子弹预制体
    public float bulletSpeed = 5f;           // 子弹速度
    public float fireInterval = 0.2f;        // 发射间隔
    public int bulletsPerWave = 8;           // 每轮发射子弹数（圆形弹幕）

    public float spreadAngle = 360f;         // 扇形角度（360表示圆形）
    public bool autoFire = true;             // 是否自动发射

    public bool ifFire = true;//是否可以射击

    void Start()
    {
        ifFire = true;
        if (autoFire)
        {
            StartCoroutine(FireLoop());
        }
    }

    public void TriggerFire()
    {
        StartCoroutine(FireLoop());
    }

    IEnumerator FireLoop()
    {
        while (true)
        {
            EmitBullets();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    void EmitBullets()
    {
        float angleStep = spreadAngle / bulletsPerWave;
        float angle = -spreadAngle / 2f;

        for (int i = 0; i < bulletsPerWave; i++)
        {
            float rad = Mathf.Deg2Rad * (angle + transform.eulerAngles.z);
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

            angle += angleStep;
        }
    }
    public void FireBulletWave()
    {
        Debug.Log("1bo");
        if (!ifFire)
            return;
        ifFire = false;
        float angleStep = spreadAngle / bulletsPerWave;
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < bulletsPerWave; i++)
        {
            float angle = startAngle + i * angleStep + transform.eulerAngles.z;
            float rad = angle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = dir * bulletSpeed;
        }
        // 发射完毕后，设置ifFire为true，允许下一次射击
        ifFire = true;
    }
}
