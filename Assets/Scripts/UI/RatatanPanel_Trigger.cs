using Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; // ��Ҫ�����������ռ���ʹ��UIImage��ع���

public class RatatanPanel_Trigger : MonoBehaviour
{
    [Header("�������")]
    public float detectionRadius = 100f; // ���뾶�����ص�λ��

    RatatanPanel panel;
    RectTransform rect;

    private void Awake()
    {
        panel = GetComponentInParent<RatatanPanel>();
        rect = GetComponent<RectTransform>();

        Init();
    }

    private void OnValidate()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(detectionRadius, rect.sizeDelta.y);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init()
    {
        EventManager.Instance.AddEventListener(E_EventType.E_Input_J, InputJ);
        EventManager.Instance.AddEventListener(E_EventType.E_Input_K, InputK);
        EventManager.Instance.AddEventListener(E_EventType.E_Input_L, InputL);
    }
    /// <summary>
    /// ����
    /// </summary>
    public void CallBack()
    {
        EventManager.Instance.RemoveEventListener(E_EventType.E_Input_J, InputJ);
        EventManager.Instance.RemoveEventListener(E_EventType.E_Input_K, InputK);
        EventManager.Instance.RemoveEventListener(E_EventType.E_Input_L, InputL);
    }

    void InputJ()
    {
        CheckNearbyBeatImages(E_RatatanType.Ra);
    }

    void InputK()
    {
        CheckNearbyBeatImages(E_RatatanType.Ta);
    }

    void InputL()
    {
        CheckNearbyBeatImages(E_RatatanType.Tan);
    }

    // ��⸽����beat��ǩUIImage
    void CheckNearbyBeatImages(E_RatatanType type)
    {
        // ��������һ���������ActiveBeatItem[0]
        if (panel.ActiveBeatItems.Count <= 0)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }
            
        GameObject obj = panel.ActiveBeatItems[0];
        // ���ж��Ƿ��ھ�������
        if (Vector2.Distance(rect.anchoredPosition, obj.GetComponent<RectTransform>().anchoredPosition) > detectionRadius)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }
        // ���ж��Ƿ�����ȷ����
        if (obj.GetComponent<RatatanPanel_Beat>().type != type)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }
        // ���ȫ���ж��ɹ����򴥷��ɹ��ж�
        EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Beat_Success, type);
    }
}