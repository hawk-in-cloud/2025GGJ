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
        {
            if(!isActive)Debug.Log("isActive");
            return;
        }
           
        // ���Э���������У���������ֹɳ���ɣ�
        if (attackCoroutine != null)
            return;

        // ���Ź�������
        _anim.Play("Attack", 0, 0.0f);

        // ִ����Ծ + ��Ƥ��Ч��
        attackCoroutine = StartCoroutine(PlayJumpAndStretchEffect());

    }

    private IEnumerator PlayJumpAndStretchEffect()
    {
        // ��ʼ�����ź���Ծλ��
        Vector3 originalScale = transform.localScale;
        Vector3 originalPos = transform.localPosition;

        // ��Ƥ������Ч��
        // ������ʹ��DOLocalScale�������ű仯�����Ҵ��е���Ч��
        transform.DOLocalMoveY(originalPos.y + 2f, 0.3f).SetEase(Ease.OutQuad); // ������Ծ
        transform.DOScale(originalScale * 1.3f, 0.15f).SetEase(Ease.InOutElastic); // ����Ч��

        // ����Ч����Ϻ�ָ�ԭ״
        yield return new WaitForSeconds(0.3f);

        

        // ����ص�ԭʼ����
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InOutElastic);

        // ��Ծ��0.15s��׼�����ǰ0.05s��������
        // ��ǰ���������߼������ǰһ˲�䣩
        yield return new WaitForSeconds(0.15f); // ������ػ��� 0.05s ����
        Attack();

        // ��Ծ����
        transform.DOLocalMoveY(originalPos.y, 0.2f).SetEase(Ease.InQuad); // �ص�ԭʼλ��

        attackCoroutine = null;
    }
}
