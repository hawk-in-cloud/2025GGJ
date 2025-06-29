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
        AudioManager.Instance.PauseBKMusic();
        Time.timeScale = 0;
    }
    public void ExitLevelUpMode()
    {
        isLevelUp = false;
        UIManager.Instance.HidePanel("LevelUpPanel");
        AudioManager.Instance.ResumeBKMusic();
        Time.timeScale = 1;
    }
}
