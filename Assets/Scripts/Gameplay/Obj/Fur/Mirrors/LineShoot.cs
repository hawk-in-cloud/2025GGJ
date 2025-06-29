using Gameplay.BaseItem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineShoot : SingletonMono<LineShoot>
{
    public Transform firePoint;         // 射线发射点
    public LineRenderer lineRenderer;   // 你的LineRenderer
    public LayerMask enemyLayer;        // 敌人的 Layer
    public float activeDuration = 20f; // 功能持续时间（秒）
    public float _timer = 0f;//计时器
    public bool _isActive = false;//开始计时
    public Transform Stellaris;//群星transform
    public GameObject BlueFlash;//蓝色闪光特效

    private void Start()
    {
        BlueFlash = transform.Find("StellarisBlue").gameObject;
    }
    void Update()
    {
        if (_isActive)
        {
            Debug.Log("Follow");
            LineShootFunc();
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _isActive = false;
                Debug.Log("功能关闭");
                BlueFlash.SetActive(false);
            }
        }
    }

    // 外部或其它逻辑调用此函数来启用功能
    public void StartLineShoot()//触发计时器
    {
        _isActive = true;
        _timer = activeDuration;
        Debug.Log("功能启用");
    }


    public void LineShootFunc()//射出射线
    {
        Vector2 origin = firePoint.position;// 起始点
        //Vector2 direction = firePoint.right;  // 或其他方向，如 (enemy.position - origin).normalized
        Vector2 direction = FindDirectionNearestEnemy(origin);
        // 发出射线
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 100f, enemyLayer);

        // 设置particle的起点和终点
        Stellaris = transform.Find("StellarisYellow");
        float Ztransform = AngleZCalculate(direction);//计算角度
        Stellaris.rotation = Quaternion.Euler(Stellaris.rotation.eulerAngles.x, Stellaris.rotation.eulerAngles.y, Ztransform); // 设置群星位置

        if (hit.collider != null)
        {
            // 命中敌人：设置LineRenderer终点为 hit.point
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
            Vector2 BlueFlashPos = new Vector2(hit.point.x, hit.point.y);
            BlueFlash.transform.position = BlueFlashPos;//设置蓝色闪光特效位置
            BlueFlash.SetActive(true);//激活蓝色闪光特效
        }
        else
        {
            BlueFlash.SetActive(false);//关闭蓝色闪光特效
            // 未命中：延伸100个单位
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * 100f);
        }
    }

    public float AngleZCalculate(Vector2 Direction)
    {
        float angle = 0.0f;
        angle =  Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg - 5.0f;
        if(transform.parent.localScale.x > 0)//如果是左边的镜子
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

        if (allEnemies.Length <= 0)//没找到
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
