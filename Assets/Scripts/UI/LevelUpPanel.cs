using Framework;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : BasePanel
{
    Button btn;
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
        this.btn = GetComponentInChildren<Button>();
        btn.onClick.AddListener(() =>
        {
            LevelManager.Instance.ExitLevelUpMode();
        });
    }
}
