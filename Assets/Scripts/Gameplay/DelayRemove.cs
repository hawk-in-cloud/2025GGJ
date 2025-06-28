using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Framework; // 引入 DOTween 命名空间

public class DelayRemove : MonoBehaviour
{
    // 用来控制消失的时间
    public float fadeDuration = 1.0f;
    public float floatDuration = 1.5f;
    public Vector3 floatAmount = new Vector3(0, 1, 0); // 浮动的距离（向上移动）

    private void Awake()
    {
        // 让字体从初始位置浮动
        FloatEffect();
    }

    // 浮动效果
    void FloatEffect()
    {
        // 使字体位置浮动
        transform.DOMove(transform.position + floatAmount, floatDuration).SetEase(Ease.OutQuad);

        // 让字体渐变消失
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>(); // 确保物体上有 CanvasGroup 组件

        canvasGroup.DOFade(0, fadeDuration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Invoke("DelayToRemove", floatDuration + fadeDuration);
        });
    }

    // 延迟销毁游戏对象
    void DelayToRemove()
    {
        PoolManager.Instance.PushObj(gameObject);
    }
}