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
        // ���ó�ʼ����
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // ִ��DOTween����
        StartCoroutine(PlayDamageTextAnimation());
    }

    private IEnumerator PlayDamageTextAnimation()
    {
        // 1. ����Ч���Ŵ�
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);

        // 2. �ȴ��Ŵ󶯻����
        yield return new WaitForSeconds(0.5f);

        // 3. ��ʼ���䲢����
        transform.DOMoveY(transform.position.y - 1.0f, 1.0f).SetEase(Ease.InQuad);  // ����
        transform.GetComponent<Image>().DOFade(0f, 1.0f);  // ����

        // 4. �ȴ��������������
        yield return new WaitForSeconds(1.0f);

        // 5. ������������������
        // �ڻ���ǰ��ȡ�� DOTween ���������������ִ��
        DOTween.Kill(transform);

        // �������壨�������壩
        Destroy(gameObject);
    }
}
