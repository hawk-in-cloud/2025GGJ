using UnityEngine;
using Framework;
public class PlayerController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float maxSpeed = 5f;         // ����ƶ��ٶ�
    public float acceleration = 10f;    // ��������
    public float deceleration = 15f;    // ��������

    [Header("��ǰ״̬")]
    [SerializeField] private Vector2 currentVelocity; // ��ǰ�ٶ�

    Rigidbody2D rb;
    float moveX => Input.GetAxisRaw("Horizontal");
    float moveY => Input.GetAxisRaw("Vertical");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, (type) =>
        {
            Debug.Log($"�ж��ɹ�{type}");
        });
    }

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

    public void Movement()
    {
        // ��ȡ���뷽��
        Vector2 inputDirection = new Vector2(moveX, moveY).normalized;

        // ����Ŀ���ٶ�
        Vector2 targetVelocity = inputDirection * maxSpeed;

        // ƽ������/����
        if (inputDirection != Vector2.zero)
        {
            // ������ʱ����
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.deltaTime);
        }
        else
        {
            // ������ʱ����
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.deltaTime);
        }

        // Ӧ���ƶ�
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}