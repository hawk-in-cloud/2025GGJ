using Framework;
using System.Collections.Generic;
using UnityEngine;


public enum E_RatatanType
{
    Ra,
    Ta,
    Tan,
}
public class RatatanPanel : BasePanel
{
    // 维护所有活跃的beat对象 -> 模拟队列
    public List<GameObject> ActiveBeatItems = new List<GameObject>();

    public RectTransform spawnFontTrans;
    RectTransform spawnRect;
    RatatanPanel_Trigger trigger;
    public override void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
    public override void HidePanel()
    {
        this.gameObject.SetActive(false);
    }
    private void Awake()
    {
        spawnRect = transform.Find("SpawnPoint").GetComponent<RectTransform>();
        trigger = transform.Find("Trigger").GetComponent<RatatanPanel_Trigger>();
        
        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Spawn, (type) =>
        {
            SpawnBeat(type);
        });

        EventManager.Instance.AddEventListener(E_EventType.E_Destroy, () =>
        {
            if(ActiveBeatItems.Count > 0)
            {
                ActiveBeatItems.RemoveAt(0);
            }
        });

        EventManager.Instance.AddEventListener<E_RatatanType>(E_EventType.E_Beat_Success, (type) =>
        {
            GameObject obj = PoolManager.Instance.GetObj("Beat", "Perfect");
            obj.transform.SetParent(transform.parent);
            obj.GetComponent<RectTransform>().localPosition = spawnFontTrans.localPosition;
        });

        EventManager.Instance.AddEventListener(E_EventType.E_Beat_Failure, () =>
        {
            GameObject obj = PoolManager.Instance.GetObj("Beat", "Miss");
            obj.transform.SetParent(transform.parent);
            obj.GetComponent<RectTransform>().localPosition = spawnFontTrans.localPosition;
        });
    }

    void SpawnBeat(E_RatatanType type)
    {
        GameObject obj = null;
        switch (type)
        {
            case E_RatatanType.Ra:
                obj = PoolManager.Instance.GetObj("Beat","Ra");
                break;
            case E_RatatanType.Ta:
                obj = PoolManager.Instance.GetObj("Beat", "Ta");
                break;
            case E_RatatanType.Tan:
                obj = PoolManager.Instance.GetObj("Beat", "Tan");
                break;
        }
        obj.transform.SetParent(transform, false);
        obj.transform.position = spawnRect.position;
        obj.GetComponent<RatatanPanel_Beat>().Init(trigger);
        ActiveBeatItems.Add(obj);
    }
}
