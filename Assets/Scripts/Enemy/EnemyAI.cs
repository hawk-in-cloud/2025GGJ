using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridManager;

public class EnemyAI : MonoBehaviour
{
    [Header("��������")]
    public Transform target;
    public float moveSpeed = 2f;
    public float detectionRange = 1f;
    public LayerMask obstacleMask;

    [Header("��ס����")]
    public float maxStuckTime = 1.2f;
    public float escapeAngle = 135f;
    public float escapeBoost = 1.5f;

    private Rigidbody2D rb;
    private Vector2 lastPosition;
    private float stuckTimer = 0f;

    private readonly float[] checkAngles = { 0f, 10f, -10f, 90f, -90f };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        lastPosition = rb.position;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 baseDirection = (target.position - transform.position).normalized;
        Vector2 moveDir = Vector2.zero;

        // === 1. �ж��Ƿ�ס ===
        float movedDist = Vector2.Distance(rb.position, lastPosition);
        if (movedDist < 0.03f)
            stuckTimer += Time.fixedDeltaTime;
        else
            stuckTimer = 0f;

        lastPosition = rb.position;

        if (stuckTimer >= maxStuckTime)
        {
            // === ����ִ������ ===
            float sign = Random.value > 0.5f ? 1f : -1f;
            moveDir = Quaternion.Euler(0, 0, escapeAngle * sign) * baseDirection;
            moveDir *= escapeBoost;
            stuckTimer = 0f;

            Debug.Log($"{name} ��ס�ˣ�ǿ������");
        }
        else
        {
            // === 2. ����·����� ===
            foreach (float angle in checkAngles)
            {
                Vector2 testDir = Quaternion.Euler(0, 0, angle) * baseDirection;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, testDir, detectionRange, obstacleMask);

                if (!hit.collider || hit.collider.gameObject == gameObject)
                {
                    moveDir = testDir;
                    break;
                }
            }

            if (moveDir == Vector2.zero)
            {
                // ���з��򱻵���Ҳ���Լ���Ĭ������ҿ���
                moveDir = baseDirection * 0.2f;
            }
        }

        // === �ƶ� ===
        rb.MovePosition(rb.position + moveDir.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Vector2 baseDir = (target.position - transform.position).normalized;
        foreach (float angle in checkAngles)
        {
            Vector2 testDir = Quaternion.Euler(0, 0, angle) * baseDir;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(testDir * detectionRange));
        }
    }
}
