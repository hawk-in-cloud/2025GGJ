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

        // ���Ź�������
        _anim.Play("Attack", 0, 0.0f);

        // ִ��ʱ����С����Ծ�ͷŴ��Ч��
        StartCoroutine(PlayShrinkJumpAndExpandEffect());
    }

    private IEnumerator PlayShrinkJumpAndExpandEffect()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 originalPos = transform.localPosition;

        // 1. ����С��һ���̶�
        transform.DOScale(originalScale * 0.5f, 0.2f).SetEase(Ease.OutBack);

        // 2. Ȼ�����������Ŵ�
        yield return new WaitForSeconds(0.2f);  // �ȴ���С���
        transform.DOLocalMoveY(originalPos.y + 2f, 0.3f).SetEase(Ease.OutQuad);  // ����
        transform.DOScale(originalScale * 1.5f, 0.3f).SetEase(Ease.InOutElastic);  // �Ŵ�

        // 3. ��Ծ���Ŵ�����󣬻ָ�ԭʼλ�úʹ�С
        yield return new WaitForSeconds(0.3f);  // �ȴ���Ծ�ͷŴ����
        Attack();  // ִ�й����߼�
        bulletEmitter.FireBulletWave();  // ����һ���ӵ�
        transform.DOLocalMoveY(originalPos.y, 0.2f).SetEase(Ease.InQuad);  // �ص�ԭλ��
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutElastic);  // �ָ�ԭ��С
    }
}
