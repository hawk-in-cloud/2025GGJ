using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class white : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform triggerRect;
    public float moveSpeed = 200f; // �ƶ��ٶȣ�����/�룩
    public float destroyOffset = 100f; // ��trigger����Զ�ľ�������

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        triggerRect = GameObject.Find("Trigger").GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (rectTransform == null || triggerRect == null)
            return;

        // �����ƶ�
        rectTransform.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;

        // ����Ƿ��Ѿ��ƶ���trigger���ָ������
        if (rectTransform.anchoredPosition.x < triggerRect.anchoredPosition.x - destroyOffset)
        {
            PoolManager.Instance.PushObj(this.gameObject);
        }
    }
}
