using DG.Tweening;
using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fur_Clock : Fur_Base
{
    public BulletEmitter bulletEmitter;
    private void Start()
    {
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, Beat_Ra);
        bulletEmitter = transform.Find("BulletEmitter").GetComponent<BulletEmitter>();
    }

    public void Beat_Ra(E_RatatanType type)
    {
        if (!isActive || type != E_RatatanType.Ra)
            return;

        // 播放攻击动画
        _anim.Play("Attack", 0, 0.0f);

        // 执行时钟缩小、跳跃和放大的效果
        StartCoroutine(PlayShrinkJumpAndExpandEffect());
    }

    private IEnumerator PlayShrinkJumpAndExpandEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 originalPos = transform.localPosition;

        // 1. 先缩小到一定程度
        transform.DOScale(originalScale * 0.5f, 0.2f).SetEase(Ease.OutBack);

        // 2. 然后向上跳并放大
        yield return new WaitForSeconds(0.2f);  // 等待缩小完成
        transform.DOLocalMoveY(originalPos.y + 2f, 0.3f).SetEase(Ease.OutQuad);  // 上升
        transform.DOScale(originalScale * 1.5f, 0.3f).SetEase(Ease.InOutElastic);  // 放大

        // 3. 跳跃并放大结束后，恢复原始位置和大小
        yield return new WaitForSeconds(0.3f);  // 等待跳跃和放大完成
        Attack();  // 执行攻击逻辑
        bulletEmitter.FireBulletWave();  // 发射一波子弹
        transform.DOLocalMoveY(originalPos.y, 0.2f).SetEase(Ease.InQuad);  // 回到原位置
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutElastic);  // 恢复原大小
    }
}
