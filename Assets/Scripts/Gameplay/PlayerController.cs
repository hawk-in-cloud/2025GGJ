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
                    int ra = Random.Range(20, 30);
                    MonsterMgr.Instance.AttackAllMonsters(ra);
                    AudioManager.Instance.PlaySound("sfx_teddy_01");
                    break;
                case E_RatatanType.Ta:
                    int ta = Random.Range(30, 40);
                    MonsterMgr.Instance.AttackAllMonsters(ta);
                    AudioManager.Instance.PlaySound("sfx_teddy_02");
                    break;
                case E_RatatanType.Tan:
                    int tan = Random.Range(50, 60);
                    MonsterMgr.Instance.AttackAllMonsters(tan);
                    AudioManager.Instance.PlaySound("sfx_teddy_03");
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
            6.1f, 6.57f, 7.3f, 7.5f, 8.4f, 8.97f, 9.53f, 10.27f, 10.43f,
    11.0f, 11.57f, 11.93f, 12.5f, 13.23f, 13.4f, 13.97f, 14.03f, 14.9f,
    15.47f, 16.2f, 16.4f, 17.47f, 20.27f, 20.83f, 21.4f, 21.93f, 22.3f,
    22.87f, 23.8f, 24.03f, 24.9f, 26.2f, 27.2f, 27.03f, 27.87f, 28.23f,
    28.8f, 34.17f, 34.43f, 34.7f, 35.27f, 35.63f, 36.0f, 36.57f, 36.73f,
    37.13f, 37.5f, 37.07f, 38.23f, 38.6f, 38.97f, 39.17f, 39.7f, 40.1f,
    40.4f, 40.63f, 41.2f, 41.57f, 41.93f, 42.6f, 42.07f, 43.03f, 43.4f,
    44.13f, 44.5f, 44.9f, 45.07f, 45.63f, 46.03f, 46.37f, 46.6f, 47.1f,
    47.47f, 47.87f, 48.4f, 48.6f, 49.0f, 49.03f, 49.53f, 50.1f, 50.47f,
    50.8f, 51.0f, 51.57f, 51.97f, 52.27f, 52.5f, 53.03f, 5.43f, 53.8f,
    54.37f, 54.53f, 54.9f, 55.27f, 55.47f, 56.0f, 56.37f, 56.73f, 56.93f,
    57.47f,999f
        };
        List<float> monsTimes = new List<float>
        {
            3.0f,5.0f,10.0f,15.0f,20.0f,25.0f,30.0f,35.0f,40.0f,45.0f,50.0f,999f
        };
        List<float> beatWhite = new List<float>
        {
            0.0f,0.185f, 0.37f, 0.556f, 0.741f, 0.926f, 1.111f, 1.296f, 1.481f, 1.667f, 1.852f, 2.037f, 2.222f, 2.407f, 2.593f, 2.778f, 2.963f, 3.148f, 3.333f, 3.519f, 3.704f, 3.889f, 4.074f, 4.259f, 4.444f, 4.63f, 4.815f, 5.0f, 5.185f, 5.556f, 5.741f, 5.926f, 6.296f, 6.667f, 6.852f, 7.037f, 7.407f, 7.778f, 7.963f, 8.148f, 8.519f, 8.704f, 9.074f, 9.259f, 9.63f, 9.815f, 10.0f, 10.556f, 10.741f, 11.111f, 11.296f, 11.667f, 12.037f, 12.222f, 12.407f, 12.778f, 12.963f, 13.519f, 13.704f, 14.259f, 14.444f, 14.63f, 15.0f, 15.185f, 15.37f, 15.741f, 15.926f, 16.296f, 16.667f, 16.852f,999f
        };

        float SpiderWeb = 2.0f;
        bool hasSpider = false;

        float currentTime = 0f;
        int beatIndex = 0;
        int index = 0;
        int monsWave = 0;

        int whiteindex = 0;

        while (true)
        {
            currentTime += 0.01f;

            Debug.Log(currentTime);
            if(currentTime >= 76f)
            {
                currentTime = 0f;
                monsWave = 0;
                index = 0;
                whiteindex = 0;
            }
            
            if (currentTime >= times[index] - 2.75f)
            {
                Debug.Log($"生成音符{index}");
                EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Spawn, sequence[beatIndex]);
                index++;
                if (index > times.Count - 1)
                {
                    index--;
                }
                beatIndex++;
                if (beatIndex >= 3)
                    beatIndex = 0;
            }

            if (currentTime >= monsTimes[monsWave])
            {
                Debug.Log($"生成怪物{monsWave}");
                MonsterMgr.Instance.GenerateMonster(5 + monsWave, 15 * monsWave);
                monsWave++;
                if (monsWave > monsTimes.Count - 1)
                {
                    monsWave--;
                }
            }

            if (currentTime >= SpiderWeb && !hasSpider)
            {
                GameObject.Find("SpiderWeb").GetComponent<Animator>().Play("Active");
                hasSpider = true;
            }

            if (currentTime >= beatWhite[whiteindex] - 2.75f)
            {
                GameObject obj = PoolManager.Instance.GetObj("Beat", "white");
                obj.transform.SetParent(GameObject.Find("Canvas").transform);
                obj.transform.position = GameObject.Find("SpawnPoint").transform.position;
                whiteindex++;
                if (whiteindex > beatWhite.Count - 1)
                {
                    whiteindex--;
                }
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