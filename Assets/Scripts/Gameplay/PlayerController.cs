using UnityEngine;
using Framework;
public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float maxSpeed = 5f;         // 最大移动速度
    public float acceleration = 10f;    // 加速力度
    public float deceleration = 15f;    // 减速力度

    [Header("当前状态")]
    [SerializeField] private Vector2 currentVelocity; // 当前速度

    Rigidbody2D rb;
    float moveX => Input.GetAxisRaw("Horizontal");
    float moveY => Input.GetAxisRaw("Vertical");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, (type) =>
        {
            Debug.Log($"判定成功{type}");
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
        // 获取输入方向
        Vector2 inputDirection = new Vector2(moveX, moveY).normalized;

        // 计算目标速度
        Vector2 targetVelocity = inputDirection * maxSpeed;

        // 平滑加速/减速
        if (inputDirection != Vector2.zero)
        {
            // 有输入时加速
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.deltaTime);
        }
        else
        {
            // 无输入时减速
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.deltaTime);
        }

        // 应用移动
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}