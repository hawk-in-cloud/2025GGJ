using DG.Tweening;
using Framework;
using System.Collections;
using UnityEngine;

public class Fur_Shafa : Fur_Base
{
    private void Start()
    {
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, Beat_Tan);
    }

    public void Beat_Tan(E_RatatanType type)
    {
        if (!isActive || type != E_RatatanType.Tan)
            return;

        // 播放攻击动画
        _anim.Play("Attack", 0, 0.0f);

        // 执行跳跃 + 橡皮管效果
        StartCoroutine(PlayJumpAndStretchEffect());
    }

    private IEnumerator PlayJumpAndStretchEffect()
    {
        // 初始的缩放和跳跃位置
        Vector3 originalScale = transform.localScale;
        Vector3 originalPos = transform.localPosition;

        // 橡皮管伸缩效果
        // 动画将使用DOLocalScale进行缩放变化，并且带有弹性效果
        transform.DOLocalMoveY(originalPos.y + 2f, 0.3f).SetEase(Ease.OutQuad); // 上升跳跃
        transform.DOScale(originalScale * 1.3f, 0.15f).SetEase(Ease.InOutElastic); // 伸缩效果

        // 伸缩效果完毕后恢复原状
        yield return new WaitForSeconds(0.3f);

        // 物体回到原始缩放
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutElastic);

        //
        Attack();
        // 跳跃下落
        transform.DOLocalMoveY(originalPos.y, 0.2f).SetEase(Ease.InQuad); // 回到原始位置
    }
}
