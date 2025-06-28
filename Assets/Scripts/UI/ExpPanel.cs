using Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro; // Add DOTween namespace

public class ExpPanel : BasePanel
{
    public int Level = 1;
    public float Exp = 0f;
    Image fill;
    private Tween currentTween; // To track the current animation

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
        fill = GetComponentsInChildren<Image>().FirstOrDefault(v => v.name == "Fill");
        fill.fillAmount = 0;

        EventManager.Instance.AddEventListener<float>(E_EventType.E_Exp_GetExp, (exp) =>
        {
            // Kill any ongoing animation to prevent conflicts
            if (currentTween != null && currentTween.IsActive())
            {
                currentTween.Kill();
            }

            float newExp = this.Exp + exp;

            if (newExp >= 100f)
            {
                // Animate to full, then reset and level up
                currentTween = fill.DOFillAmount(1f, 0.5f)
                    .OnComplete(() =>
                    {
                        this.Exp = newExp - 100f;

                        fill.fillAmount = 0;
                        Level++;
                        // TODO.此处执行升级逻辑
                        Debug.Log("升级！当前等级为" + Level);
                        LevelManager.Instance.EnterLevelUpMode();

                        // If we still have remaining exp after level up, animate that
                        if (this.Exp > 0)
                        {
                            float fAmount = this.Exp / 100f;
                            currentTween = fill.DOFillAmount(fAmount, 0.5f);
                        }
                    });
            }
            else
            {
                // Just animate to the new exp value
                this.Exp = newExp;
                float fAmount = this.Exp / 100f;
                currentTween = fill.DOFillAmount(fAmount, 0.5f);
            }
        });
    }

    private void OnDestroy()
    {
        // Clean up any active tweens when the object is destroyed
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }
}