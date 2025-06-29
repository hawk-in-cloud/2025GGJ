using Framework;
using System.Collections.Generic;
using Gameplay.BaseItem;
using UnityEngine;
using UnityEngine.UI;


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
<<<<<<< Updated upstream

=======
    public RectTransform spawnFontTrans;

    //菜单按钮
    public Button reTurnBtn;
>>>>>>> Stashed changes
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
