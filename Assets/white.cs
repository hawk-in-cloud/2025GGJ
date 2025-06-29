using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class white : MonoBehaviour
{
    RectTransform rectTransform;
    RectTransform triggerRect;
    public float moveSpeed = 200f; // 移动速度（像素/秒）
    public float destroyOffset = 100f; // 在trigger左侧多远的距离销毁

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        triggerRect = GameObject.Find("Trigger").GetComponent<RectTransform>();
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
            PoolManager.Instance.PushObj(this.gameObject);
        }
    }
}
