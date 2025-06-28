using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineShoot : MonoBehaviour
{
    public Transform firePoint;         // ���߷����
    public LineRenderer lineRenderer;   // ���LineRenderer
    public LayerMask enemyLayer;        // ���˵� Layer

    void Update()
    {
        Vector2 origin = firePoint.position;
        Vector2 direction = firePoint.right;  // ������������ (enemy.position - origin).normalized

        // ��������
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, 100f, enemyLayer);

        if (hit.collider != null)
        {
            // ���е��ˣ�����LineRenderer�յ�Ϊ hit.point
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // δ���У�����100����λ
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin + direction * 100f);
        }
    }
}
