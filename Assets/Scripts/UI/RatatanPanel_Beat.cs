using UnityEngine;
using Framework;

public class RatatanPanel_Beat : MonoBehaviour
{
    public E_RatatanType type;
    [Header("�ƶ�����")]
    public float moveSpeed = 200f; // �ƶ��ٶȣ�����/�룩
    public float destroyOffset = 100f; // ��trigger����Զ�ľ�������

    private RatatanPanel_Trigger trigger;
    private RectTransform rectTransform;
    private RectTransform triggerRect;

    public void Init(RatatanPanel_Trigger t)
    {
        trigger = t;
        rectTransform = GetComponent<RectTransform>();
        triggerRect = t.GetComponent<RectTransform>();
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
            DestroyBeat();
        }
    }

    private void DestroyBeat()
    {
        // �����������������ǰ��Ч�����綯������Ч�ȣ�

        // ����
        EventManager.Instance.EventTrigger(E_EventType.E_Destroy);
        // ���ֳػ���
        PoolManager.Instance.PushObj(this.gameObject);
    }

    // �ڱ༭�����ӻ�����λ��
    private void OnDrawGizmosSelected()
    {
        if (triggerRect != null && destroyOffset > 0)
        {
            Vector3 destroyPos = triggerRect.position + Vector3.left * destroyOffset;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(triggerRect.position, destroyPos);
            Gizmos.DrawWireCube(destroyPos, Vector3.one * 20f);
        }
    }
}