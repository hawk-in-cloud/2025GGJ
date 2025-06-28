using Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fur_Base : MonoBehaviour
{
    [Header("家具状态")]
    public bool isActive = false;
    [Header("缩放参数")]
    public float scaleFactor = 1.2f;
    public float scaleSpeed = 5f; // 放大缩小的速度

    Vector3 originalScale; // 原始大小
    SpriteRenderer sp;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale; // 记录物体的初始大小
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
        // 获取鼠标在世界空间的位置
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (LevelManager.Instance.isLevelUp)
            Debug.Log($"{GetComponent<Collider2D>() != null}" +
                $"{GetComponent<Collider2D>().bounds.Contains(mousePosition)}");

        // 检查鼠标是否在物体上
        if (GetComponent<Collider2D>() != null && GetComponent<Collider2D>().bounds.Contains(mousePosition) && LevelManager.Instance.isLevelUp)
        {
            // 鼠标悬停时，放大物体
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
            // 鼠标离开时，恢复到原始大小
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * scaleSpeed);
        }
    }
}
