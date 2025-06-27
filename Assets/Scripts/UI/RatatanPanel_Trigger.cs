using Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; // 需要添加这个命名空间来使用UIImage相关功能

public class RatatanPanel_Trigger : MonoBehaviour
{
    [Header("检测设置")]
    public float detectionRadius = 100f; // 检测半径（像素单位）

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
    /// 初始化
    /// </summary>
    public void Init()
    {
        EventManager.Instance.AddEventListener(E_EventType.E_Input_J, InputJ);
        EventManager.Instance.AddEventListener(E_EventType.E_Input_K, InputK);
        EventManager.Instance.AddEventListener(E_EventType.E_Input_L, InputL);
    }
    /// <summary>
    /// 回收
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

    // 检测附近的beat标签UIImage
    void CheckNearbyBeatImages(E_RatatanType type)
    {
        // 离得最近的一般情况下是ActiveBeatItem[0]
        if (panel.ActiveBeatItems.Count <= 0)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }
            
        GameObject obj = panel.ActiveBeatItems[0];
        // 先判断是否在距离以内
        if (Vector2.Distance(rect.anchoredPosition, obj.GetComponent<RectTransform>().anchoredPosition) > detectionRadius)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }
        // 再判断是否是正确类型
        if (obj.GetComponent<RatatanPanel_Beat>().type != type)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }
        // 如果全部判定成功，则触发成功判定
        EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Beat_Success, type);
    }
}