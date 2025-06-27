using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuPanel : MonoBehaviour
{
    public void Btn_NewGame()
    {
        UIManager.Instance.ShowPanel("LoadPanel");
        LoadPanel.isLoad = false;
    }
    public void Btn_Continued()
    {
        UIManager.Instance.ShowPanel("LoadPanel");
        LoadPanel.isLoad = true;
    }

    public void Btn_Settings()
    {
        UIManager.Instance.ShowPanel("MenuPanel");
    }

    public void Btn_QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit(); // ÍË³öÓÎÏ·
#endif
    }
}
