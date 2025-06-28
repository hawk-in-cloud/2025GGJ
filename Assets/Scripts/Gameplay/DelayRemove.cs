using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Framework; // ���� DOTween �����ռ�

public class DelayRemove : MonoBehaviour
{
    // ����������ʧ��ʱ��
    public float fadeDuration = 1.0f;
    public float floatDuration = 1.5f;
    public Vector3 floatAmount = new Vector3(0, 1, 0); // �����ľ��루�����ƶ���

    private void Awake()
    {
        // ������ӳ�ʼλ�ø���
        FloatEffect();
    }

    // ����Ч��
    void FloatEffect()
    {
        // ʹ����λ�ø���
        transform.DOMove(transform.position + floatAmount, floatDuration).SetEase(Ease.OutQuad);

        // �����彥����ʧ
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>(); // ȷ���������� CanvasGroup ���

        canvasGroup.DOFade(0, fadeDuration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            Invoke("DelayToRemove", floatDuration + fadeDuration);
        });
    }

    // �ӳ�������Ϸ����
    void DelayToRemove()
    {
        PoolManager.Instance.PushObj(gameObject);
    }
}