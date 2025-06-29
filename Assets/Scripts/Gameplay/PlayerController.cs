using DG.Tweening;
using Framework;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            // 相机震动
            Camera.main.DOShakePosition(0.1f, 0.2f, 10, 90, true);
            animator.Play("Attack", 0, 0f);
            CircleShockTrigger.Instance.TriggerCircleShock(transform);
            GameObject obj = PoolManager.Instance.GetObj("VFX", "dirt");
            obj.transform.position = new Vector2(transform.position.x - 0.3f, transform.position.y - 1.5f);
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

        EventManager.Instance.AddEventListener(E_EventType.E_StartGame, () =>
        {
            AudioManager.Instance.PlayBKMusic("GameMusic");
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
        });
    }

    private Coroutine sequenceCoroutine;
    private bool isSequenceRunning = false;

    void Update()
    {
        // 增加乐谱
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            EventManager.Instance.EventTrigger(E_EventType.E_StartGame);
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
        List<float> times = new List<float>
        {
            5.95f,  6.50f,  7.43f,  8.91f,  9.47f, 10.39f, 11.87f, 12.43f, 13.36f, 14.84f,
            15.39f, 16.32f, 20.21f, 20.76f, 22.25f, 22.80f, 23.73f, 26.13f, 26.69f, 28.17f,
            28.73f, 34.10f, 34.66f, 35.58f, 36.50f, 37.06f, 38.54f, 39.10f, 40.02f, 40.58f,
            41.50f, 42.43f, 42.99f, 44.47f, 45.02f, 45.95f, 46.51f, 47.43f, 48.36f, 48.91f,
            50.39f, 50.95f, 51.87f, 52.43f, 53.36f, 54.28f, 54.84f, 56.32f, 56.87f
        };
        List<float> monsTimes = new List<float>
        {
            3.0f,80.0f,13.0f,18.0f,23.0f,27.0f,33.0f,38.0f,45.0f,50.0f
        };

        float currentTime = 0f;
        int beatIndex = 0;
        int index = 0;
        int monsWave = 0;

        while (true)
        {
            if (currentTime >= times[index] - 3.0f)
            {
                Debug.Log($"生成音符{index}");
                EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Spawn, sequence[beatIndex]);
                index++;
                beatIndex++;
                if (beatIndex >= 3)
                    beatIndex = 0;
            }

            if (currentTime >= monsTimes[monsWave])
            {
                Debug.Log($"生成怪物{monsWave}");
                MonsterMgr.Instance.GenerateMonster(5);
                monsWave++;
            }

            currentTime += 0.01f;

            if(currentTime >= times[times.Count - 1])
            {
                currentTime = 0f;
            }

            yield return new WaitForSeconds(0.01f);
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

            // === 左右翻转 ===
            if (moveX != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = moveX > 0 ? -1f : 1f;  // 向右翻转
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