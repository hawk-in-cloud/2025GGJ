using DG.Tweening;
using Framework;
using System.Collections;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float maxSpeed = 5f;         // 最大移动速度
    public float acceleration = 10f;    // 加速力度
    public float deceleration = 15f;    // 减速力度
    [Header("当前状态")]
    [SerializeField] private Vector2 currentVelocity; // 当前速度

    [Header("冲击波参数")]
    public float shockWaveRadius_Ra;
    public float shockWaveRadius_Ta;
    public float shockWaveRadius_Tan;

    Rigidbody2D rb;
    float moveX => Input.GetAxisRaw("Horizontal");
    float moveY => Input.GetAxisRaw("Vertical");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, (type) =>
        {
            // 相机震动
            Camera.main.DOShakePosition(0.1f, 0.2f, 10, 90, true);
            // 攻击逻辑
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
        // 获得经验
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EventManager.Instance.EventTrigger<float>(E_EventType.E_Exp_GetExp, 35f);
        }
        // 增加乐谱
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
            Debug.Log($"进行{rest}s休息...");
            yield return new WaitForSeconds(2f);
        }
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