using Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Needed for coroutines

public class RatatanPanel_Trigger : MonoBehaviour
{
    [Header("�������")]
    public float detectionRadius = 100f; // ���뾶�����ص�λ��
    [Header("�Ӿ�Ч��")]
    public float flashDuration = 0.5f; // ��ɫ�仯����ʱ��

    RatatanPanel panel;
    RectTransform rect;
    Image image;

    private void Awake()
    {
        panel = GetComponentInParent<RatatanPanel>();
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();

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
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, (type) =>
        {
            StartCoroutine(FlashColor(Color.yellow, Color.white));
        });
        EventManager.Instance.AddEventListener(E_EventType.E_Beat_Failure, () =>
        {
            StartCoroutine(FlashColor(Color.red, Color.white));
        });
    }

    /// <summary>
    /// ����
    /// </summary>
    public void CallBack()
    {
        EventManager.Instance.RemoveEventListener(E_EventType.E_Input_J, InputJ);
        EventManager.Instance.RemoveEventListener(E_EventType.E_Input_K, InputK);
        EventManager.Instance.RemoveEventListener(E_EventType.E_Input_L, InputL);
        EventManager.Instance.RemoveEventListener(E_EventType.E_Beat_Success, () => { });
        EventManager.Instance.RemoveEventListener(E_EventType.E_Beat_Failure, () => { });
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
        if (panel.ActiveBeatItems.Count <= 0)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }

        GameObject obj = panel.ActiveBeatItems[0];
        if (Vector2.Distance(rect.anchoredPosition, obj.GetComponent<RectTransform>().anchoredPosition) > detectionRadius)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }
        if (obj.GetComponent<RatatanPanel_Beat>().type != type)
        {
            EventManager.Instance.EventTrigger(E_EventType.E_Beat_Failure);
            return;
        }

        PoolManager.Instance.PushObj(panel.ActiveBeatItems[0]);
        panel.ActiveBeatItems.RemoveAt(0);
        EventManager.Instance.EventTrigger<E_RatatanType>(E_EventType.E_Beat_Success, type);
        EventManager.Instance.EventTrigger<float>(E_EventType.E_Exp_GetExp, 10f);
    }
    IEnumerator FlashColor(Color startColor, Color endColor)
    {
        image.color = startColor;

        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            image.color = Color.Lerp(startColor, endColor, elapsedTime / flashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        image.color = endColor;
    }
}