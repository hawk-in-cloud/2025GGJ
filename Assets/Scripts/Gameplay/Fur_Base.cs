using Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fur_Base : MonoBehaviour
{
    [Header("�Ҿ�״̬")]
    public bool isActive = false;
    [Header("���Ų���")]
    public float scaleFactor = 1.2f;
    public float scaleSpeed = 5f; // �Ŵ���С���ٶ�

    Vector3 originalScale; // ԭʼ��С
    SpriteRenderer sp;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale; // ��¼����ĳ�ʼ��С
        isActive = false;
        sp.color = Color.gray;
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
        // ��ȡ���������ռ��λ��
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (LevelManager.Instance.isLevelUp)
            Debug.Log($"{GetComponent<Collider2D>() != null}" +
                $"{GetComponent<Collider2D>().bounds.Contains(mousePosition)}");

        // �������Ƿ���������
        if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().bounds.Contains(mousePosition) && LevelManager.Instance.isLevelUp)
        {
            // �����ͣʱ���Ŵ�����
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleFactor, Time.unscaledDeltaTime * scaleSpeed);
            if (Input.GetMouseButtonDown(0))
            {
                this.isActive = true;
                sp.color = Color.white;
                EventManager.Instance.EventTrigger(E_EventType.E_Exp_EndLevelUp);
            }
        }
        else
        {
            // ����뿪ʱ���ָ���ԭʼ��С
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * scaleSpeed);
        }
    }
}
