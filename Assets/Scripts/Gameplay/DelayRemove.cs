using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Framework;

public class DelayRemove : MonoBehaviour
{
    private void Start()
    {
        // 设置初始缩放
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // 执行DOTween动画
        StartCoroutine(PlayDamageTextAnimation());
    }

    private IEnumerator PlayDamageTextAnimation()
    {
        // 1. 弹簧效果放大
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // 2. 等待放大动画完成
        yield return new WaitForSeconds(0.5f);

        // 3. 开始下落并淡出
        transform.DOMoveY(transform.position.y - 1.0f, 1.0f).SetEase(Ease.InQuad);  // 下落
        transform.GetComponent<Image>().DOFade(0f, 1.0f);  // 渐隐

        // 4. 等待淡出和下落完成
        yield return new WaitForSeconds(1.0f);

        // 5. 动画结束后销毁物体
        // 在回收前先取消 DOTween 动画，避免其继续执行
        DOTween.Kill(transform);

        // 回收物体（禁用物体）
        Destroy(gameObject);
    }
}
