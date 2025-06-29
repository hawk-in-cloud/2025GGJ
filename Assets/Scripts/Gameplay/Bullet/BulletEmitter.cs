using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEmitter : MonoBehaviour
{
    public GameObject bulletPrefab;          // �ӵ�Ԥ����
    public float bulletSpeed = 5f;           // �ӵ��ٶ�
    public float fireInterval = 0.2f;        // ������
    public int bulletsPerWave = 8;           // ÿ�ַ����ӵ�����Բ�ε�Ļ��

    public float spreadAngle = 360f;         // ���νǶȣ�360��ʾԲ�Σ�
    public bool autoFire = true;             // �Ƿ��Զ�����

    public bool ifFire = true;//�Ƿ�������

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
        // ������Ϻ�����ifFireΪtrue��������һ�����
        ifFire = true;
    }
}
