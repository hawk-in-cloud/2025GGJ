using Framework;
using UnityEngine;
using UnityEngine.UI;

public class HPPanel : BasePanel
{
    Image[] imgs;
    int index = 0;
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
        imgs = GetComponentsInChildren<Image>();
        index = imgs.Length - 1;
        EventManager.Instance.AddEventListener(E_EventType.E_GirlInjured, () =>
        {
            imgs[index].gameObject.SetActive(false);
            index--;
        });
    }
}
