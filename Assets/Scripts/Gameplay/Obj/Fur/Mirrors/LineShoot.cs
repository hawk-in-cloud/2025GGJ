using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineShoot : MonoBehaviour
{
    public Transform firePoint;         // ���߷����
    public LineRenderer lineRenderer;   // ���LineRenderer
    public Material LineMaterial;//���߲���
    public LayerMask enemyLayer;        // ���˵� Layer
    public float activeDuration = 20f; // ���ܳ���ʱ�䣨�룩
    public float _timer = 0f;//��ʱ��
    public bool _isActive = false;//��ʼ��ʱ
    public Transform Stellaris;//Ⱥ��transform
    public GameObject BlueFlash;//��ɫ������Ч

    private void Start()
    {
        BlueFlash = transform.Find("StellarisBlue").gameObject;
        LineMaterial = lineRenderer.material;
    }
    void Update()
    {
        if (_isActive)
        {
            Debug.Log("Follow");
            LineShootFunc();
            _timer -= Time.deltaTime;
            if (_timer <= 2f)
            {
                LineMaterial.SetFloat("_FadeAmount", _timer * (-0.25f) + 0.4f);
                Debug.Log("���ܹر�");
                BlueFlash.SetActive(false);
            }
            if (_timer <= 0f)
            {
                _isActive = false;
                Debug.Log("���ܹر�");
                BlueFlash.SetActive(false);
                LineMaterial.SetFloat("_FadeAmount", -0.1f);
                transform.gameObject.SetActive(false);
            }
        }
    }

    // �ⲿ�������߼����ô˺��������ù���
    public void StartLineShoot()//������ʱ��
    {
        _isActive = true;
        _timer = activeDuration;
        Debug.Log("��������");
    }


    public void LineShootFunc()//�������
    {
        Vector2 origin = firePoint.position;// ��ʼ��
        //Vector2 direction = firePoint.right;  // ������������ (enemy.position - origin).normalized
        Vector2 direction = FindDirectionNearestEnemy(origin);
        // ��������
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 100f, enemyLayer);

        // ����particle�������յ�
        Stellaris = transform.Find("StellarisYellow");
        float Ztransform = AngleZCalculate(direction);//����Ƕ�
        Stellaris.rotation = Quaternion.Euler(Stellaris.rotation.eulerAngles.x, Stellaris.rotation.eulerAngles.y, Ztransform); // ����Ⱥ��λ��

        if (hit.collider != null)
        {
            Debug.Log("������");
            // ���е��ˣ�����LineRenderer�յ�Ϊ hit.point
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
            Vector3 BlueFlashPos = new Vector3(hit.point.x, hit.point.y, 8.0f);
            BlueFlash.transform.position = BlueFlashPos;//������ɫ������Чλ��
            BlueFlash.SetActive(true);//������ɫ������Ч
        }
        else
        {
            Debug.Log("δ����");
            BlueFlash.SetActive(false);//�ر���ɫ������Ч
            // δ���У�����100����λ
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * 8f);
        }
    }

    public float AngleZCalculate(Vector2 Direction)
    {
        float angle = 0.0f;
        angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg - 5.0f;
        if (transform.parent.localScale.x > 0)//�������ߵľ���
        {
            angle += 180.0f;
        }
        return angle;
    }
    public Vector2 FindDirectionNearestEnemy(Vector2 myPosition)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        GameObject closest = null;

        if (allEnemies.Length <= 0)//û�ҵ�
        {
            return firePoint.right;
        }
        foreach (GameObject enemy in allEnemies)
        {
            float dist = Vector2.Distance(myPosition, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        Vector2 EnemyPos = new Vector2(closest.transform.position.x, closest.transform.position.y);

        Vector2 Direction = (EnemyPos - myPosition).normalized;
        return Direction;
    }
}
