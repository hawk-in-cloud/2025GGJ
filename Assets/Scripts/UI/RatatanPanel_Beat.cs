using UnityEngine;
using Framework;

public class RatatanPanel_Beat : MonoBehaviour
{
    public E_RatatanType type;
    [Header("移动设置")]
    public float moveSpeed = 200f; // 移动速度（像素/秒）
    public float destroyOffset = 100f; // 在trigger左侧多远的距离销毁

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

        // 向左移动
        rectTransform.anchoredPosition += Vector2.left * moveSpeed * Time.deltaTime;

        // 检查是否已经移动到trigger左侧指定距离
        if (rectTransform.anchoredPosition.x < triggerRect.anchoredPosition.x - destroyOffset)
        {
            DestroyBeat();
        }
    }

    private void DestroyBeat()
    {
        // 可以在这里添加销毁前的效果（如动画、音效等）

        // 销毁
        EventManager.Instance.EventTrigger(E_EventType.E_Destroy);
        // 对现池回收
        PoolManager.Instance.PushObj(this.gameObject);
    }

    // 在编辑器可视化销毁位置
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