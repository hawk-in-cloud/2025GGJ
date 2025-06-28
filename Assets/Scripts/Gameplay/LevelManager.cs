using Framework;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public bool isLevelUp = false;
    private LevelManager() { }
    public void EnterLevelUpMode()
    {
        isLevelUp = true;
        UIManager.Instance.ShowPanel("LevelUpPanel");
        Time.timeScale = 0;
    }
    public void ExitLevelUpMode()
    {
        isLevelUp = false;
        UIManager.Instance.HidePanel("LevelUpPanel");
        Time.timeScale = 1;
    }
}
