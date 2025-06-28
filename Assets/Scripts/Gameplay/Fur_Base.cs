using DG.Tweening; // 引入DOTween命名空间
using UnityEngine;

public class Fur_Base : MonoBehaviour
{
    [Header("家具状态")]
    public bool isActive = false;
    [Header("缩放参数")]
    public float scaleFactor = 1.2f;
    public float scaleSpeed = 5f; // 放大缩小的速度

    protected Vector3 originalScale; // 原始大小
    protected SpriteRenderer sp;
    protected Animator animator;
    protected Tween shakeTween; // 用于保存摇晃的Tween

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale; // 记录物体的初始大小
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
        // 获取鼠标在世界空间的位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 检查鼠标是否在物体上
        if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().bounds.Contains(mousePosition) && LevelManager.Instance.isLevelUp)
        {
            // 鼠标悬停时，放大物体
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleFactor, Time.unscaledDeltaTime * scaleSpeed);

            // 添加摇晃效果（使用DOTween，避免TimeScale的影响）
            if (shakeTween == null || !shakeTween.IsPlaying())
            {
                shakeTween = transform.DOLocalRotate(new Vector3(0, 0, 10f), 0.2f, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetUpdate(true) // 让Tween在不受TimeScale影响的情况下更新
                    .OnKill(() => transform.localRotation = Quaternion.Euler(0, 0, 0)); // 动画结束后重置Z轴角度
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
            // 鼠标离开时，恢复到原始大小
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * scaleSpeed);

            // 停止摇晃动画并重置Z轴旋转
            if (shakeTween != null && shakeTween.IsPlaying())
            {
                shakeTween.Kill(); // 结束动画
                transform.localRotation = Quaternion.Euler(0, 0, 0); // 重置Z轴旋转
            }
        }
    }
}
