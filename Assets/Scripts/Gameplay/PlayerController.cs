using DG.Tweening;
using Framework;
using System.Collections;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float maxSpeed = 5f;         // ����ƶ��ٶ�
    public float acceleration = 10f;    // ��������
    public float deceleration = 15f;    // ��������
    [Header("��ǰ״̬")]
    [SerializeField] private Vector2 currentVelocity; // ��ǰ�ٶ�

    [Header("���������")]
    public float shockWaveRadius_Ra;
    public float shockWaveRadius_Ta;
    public float shockWaveRadius_Tan;

    Rigidbody2D rb;
    Animator animator;

    float moveX => Input.GetAxisRaw("Horizontal");
    float moveY => Input.GetAxisRaw("Vertical");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        animator = GetComponent<Animator>();

        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, (type) =>
        {
            // �����
            Camera.main.DOShakePosition(0.1f, 0.2f, 10, 90, true);
            animator.Play("Attack", 0, 0f);
            CircleShockTrigger.Instance.TriggerCircleShock(transform);
            GameObject obj = PoolManager.Instance.GetObj("VFX", "dirt");
            obj.transform.position = new Vector2(transform.position.x - 0.3f, transform.position.y - 1.5f);
            // �����߼�
            switch (type)
            {
                case E_RatatanType.Ra:
                    break;
                case E_RatatanType.Ta:
                    break;
                case E_RatatanType.Tan:
                    break;
            }
        });
    }

    private Coroutine sequenceCoroutine;
    private bool isSequenceRunning = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Spawn, E_RatatanType.Ra);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Spawn, E_RatatanType.Ta);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Spawn, E_RatatanType.Tan);
        }
        // ��þ���
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EventManager.Instance.EventTrigger<float>(E_EventType.E_Exp_GetExp, 35f);
        }
        // ��������
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (isSequenceRunning)
            {
                StopCoroutine(sequenceCoroutine);
                isSequenceRunning = false;
            }
            else
            {
                sequenceCoroutine = StartCoroutine(TriggerSequence());
                isSequenceRunning = true;
            }
        }


        if (Input.GetKeyDown(KeyCode.J))
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Input_J);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Input_K);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Input_L);
        }

        Movement();
    }

    IEnumerator TriggerSequence()
    {
        E_RatatanType[] sequence = new E_RatatanType[]
        {
        E_RatatanType.Ra,
        E_RatatanType.Ta,
        E_RatatanType.Tan
        };

        while (true)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Spawn, sequence[i]);
                float random = Random.Range(0.4f, 0.6f);
                yield return new WaitForSeconds(random);
            }

            float rest = Random.Range(1.5f, 2.0f);
            Debug.Log($"����{rest}s��Ϣ...");
            yield return new WaitForSeconds(2f);
        }
    }

    public void Movement()
    {
        Vector2 inputDirection = new Vector2(moveX, moveY).normalized;
        Vector2 targetVelocity = inputDirection * maxSpeed;

        animator.SetBool("Run", targetVelocity != Vector2.zero);

        if (inputDirection != Vector2.zero)
        {
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.deltaTime);

            // === ���ҷ�ת ===
            if (moveX != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = moveX > 0 ? -1f : 1f;  // ���ҷ�ת
                transform.localScale = scale;
            }
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.deltaTime);
        }

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }


}