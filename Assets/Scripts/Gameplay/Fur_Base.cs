using DG.Tweening; // ����DOTween�����ռ�
using UnityEngine;

public class Fur_Base : MonoBehaviour
{
    [Header("�Ҿ�״̬")]
    public bool isActive = false;
    [Header("���Ų���")]
    public float scaleFactor = 1.2f;
    public float scaleSpeed = 5f; // �Ŵ���С���ٶ�

    protected Vector3 originalScale; // ԭʼ��С
    protected SpriteRenderer sp;
    protected Animator animator;
    protected Tween shakeTween; // ���ڱ���ҡ�ε�Tween

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale; // ��¼����ĳ�ʼ��С
        isActive = false;
        sp.color = Color.gray;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        OnHover();

        if (!isActive)
            return;
        ActiveLogic();
    }

    public void ActiveLogic()
    {

    }

    public void OnHover()
    {
        // ��ȡ���������ռ��λ��
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �������Ƿ���������
        if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().bounds.Contains(mousePosition) && LevelManager.Instance.isLevelUp)
        {
            // �����ͣʱ���Ŵ�����
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleFactor, Time.unscaledDeltaTime * scaleSpeed);

            // ���ҡ��Ч����ʹ��DOTween������TimeScale��Ӱ�죩
            if (shakeTween == null || !shakeTween.IsPlaying())
            {
                shakeTween = transform.DOLocalRotate(new Vector3(0, 0, 10f), 0.2f, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true) // ��Tween�ڲ���TimeScaleӰ�������¸���
                    .OnKill(() => transform.localRotation = Quaternion.Euler(0, 0, 0)); // ��������������Z��Ƕ�
            }

            if (Input.GetMouseButtonDown(0))
            {
                this.isActive = true;
                sp.color = Color.white;
                LevelManager.Instance.ExitLevelUpMode();
                animator.Play("Idle");
            }
        }
        else
        {
            // ����뿪ʱ���ָ���ԭʼ��С
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * scaleSpeed);

            // ֹͣҡ�ζ���������Z����ת
            if (shakeTween != null && shakeTween.IsPlaying())
            {
                shakeTween.Kill(); // ��������
                transform.localRotation = Quaternion.Euler(0, 0, 0); // ����Z����ת
            }
        }
    }
}
