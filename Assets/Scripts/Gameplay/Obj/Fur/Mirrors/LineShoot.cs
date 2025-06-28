using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineShoot : MonoBehaviour
{
    public Transform firePoint;         // 射线发射点
    public LineRenderer lineRenderer;   // 你的LineRenderer
    public LayerMask enemyLayer;        // 敌人的 Layer

    void Update()
    {
        Vector2 origin = firePoint.position;
        Vector2 direction = firePoint.right;  // 或其他方向，如 (enemy.position - origin).normalized

        // 发出射线
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 100f, enemyLayer);

        if (hit.collider != null)
        {
            // 命中敌人：设置LineRenderer终点为 hit.point
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // 未命中：延伸100个单位
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * 100f);
        }
    }
}
