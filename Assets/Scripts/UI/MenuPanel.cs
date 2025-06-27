using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using DG.Tweening;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;

public class MenuPanel : BasePanel
{
    private RectTransform rectTrans;
    private float easeDuration = 0.3f;
    private bool isAnimating = false;
    private bool isExpanded = false;
    // MusicPanel
    private Slider musicSlider;
    private Toggle musicToggle;
    private float musicValue = 0.5f;
    // SoundPanel
    private Slider soundSlider;
    private Toggle soundToggle;
    private float soundValue = 0.5f;
    // ScreenMode
    private bool isFullScreen = true;
    private Button btn_Windowed;
    private Button btn_FullScreen;
    private GameObject checkMark_Windowed;
    private GameObject checkMark_FullScreen;
    // Btn
    private Button btn_Close;
    private Button btn_Title;
    private Button btn_Continued;
    private void Awake()
    {
        // AnimationComponent
        rectTrans = GetComponent<RectTransform>();
        rectTrans.localScale = Vector3.zero;
        // AnimationData
        easeDuration = 0.5f;
        isAnimating = false;
        isExpanded = false;
        // MusicPanel
        GameObject musicObj = transform.Find("Panel_Music").gameObject;
        musicSlider = musicObj.GetComponentInChildren<Slider>();
        musicToggle = musicObj.GetComponentInChildren<Toggle>();
        // SoundPanel
        GameObject soundObj = transform.Find("Panel_Sound").gameObject;
        soundSlider = soundObj.GetComponentInChildren<Slider>();
        soundToggle = soundObj.GetComponentInChildren<Toggle>();
        // ScreenModes
        btn_Windowed = GetComponentsInChildren<Transform>()
                      .FirstOrDefault(t => t.name == "Btn_Windowed")?.gameObject.GetComponent<Button>();
        btn_FullScreen = GetComponentsInChildren<Transform>()
                      .FirstOrDefault(t => t.name == "Btn_FullScreen")?.gameObject.GetComponent<Button>();
        checkMark_Windowed = GetComponentsInChildren<Transform>()
                      .FirstOrDefault(t => t.name == "Checkmark_Windowed")?.gameObject;
        checkMark_FullScreen = GetComponentsInChildren<Transform>()
                      .FirstOrDefault(t => t.name == "Checkmark_FullScreen")?.gameObject;
        checkMark_Windowed.SetActive(false);
        btn_FullScreen.interactable = false;
        // Btn
        btn_Close = GetComponentsInChildren<Transform>()
                      .FirstOrDefault(t => t.name == "Btn_Close") ?.gameObject.GetComponent<Button>();
        btn_Title = GetComponentsInChildren<Transform>()
                      .FirstOrDefault(t => t.name == "Btn_Title")?.gameObject.GetComponent<Button>();
        btn_Continued = GetComponentsInChildren<Transform>()
                      .FirstOrDefault(t => t.name == "Btn_Continued")?.gameObject.GetComponent<Button>();
        // AddListener
        AddPanelListener();
    }
    public override void HidePanel()
    {
        if (isAnimating || !isExpanded) return;
        isAnimating = true;

        rectTrans.DOScale(Vector3.zero, easeDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                isAnimating = false;
                isExpanded = false;
                this.gameObject.SetActive(false);
            });
    }

    public override void ShowPanel()
    {
        if (isAnimating || isExpanded) return;
        isAnimating = true;

        this.gameObject.SetActive(true);

        rectTrans.DOScale(Vector3.one, easeDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isAnimating = false;
                isExpanded = true;
            });
        
    }
    protected void AddPanelListener()
    {
        musicSlider.onValueChanged.AddListener(OnMusicValueChange);
        musicToggle.onValueChanged.AddListener(OnMusicToggleChange);
        soundSlider.onValueChanged.AddListener(OnSoundValueChange);
        soundToggle.onValueChanged.AddListener(OnSoundToggleChange);
        btn_FullScreen.onClick.AddListener(ChangeScreenMode);
        btn_Windowed.onClick.AddListener(ChangeScreenMode);
        btn_Close.onClick.AddListener(Btn_Close);
        btn_Continued.onClick.AddListener(Btn_Close);
        btn_Title.onClick.AddListener(Btn_Title);
    }

    private void Btn_Close()
    {
        HidePanel();
    }
    private void Btn_Title()
    {
        SceneManager.LoadScene(0);
        HidePanel();
    }
    public void OnMusicValueChange(float value)
    {
        AudioManager.Instance.ChangeBKMusicValue(value);
        musicValue = value;

        if (AudioManager.Instance.BkMusicValue == 0)
        {
            musicToggle.isOn = false;
        }
        else
        {
            musicToggle.isOn = true;
        }
    }
    
    public void OnMusicToggleChange(bool boolean)
    {
        musicSlider.onValueChanged.RemoveListener(OnMusicValueChange);
        if(boolean)
        {
            AudioManager.Instance.ChangeBKMusicValue(musicValue);
            musicSlider.value = musicValue;
        }
        else
        {
            AudioManager.Instance.ChangeBKMusicValue(0f);
            musicSlider.value = 0f;
        }
        musicSlider.onValueChanged.AddListener(OnMusicValueChange);
    }
    public void OnSoundValueChange(float value)
    {
        AudioManager.Instance.ChangeSoundValue(value);
        soundValue = value;

        if (AudioManager.Instance.SoundValue == 0f)
        {
            soundToggle.isOn = false;
        }
        else
        {
            soundToggle.isOn = true;
        }
    }
    public void OnSoundToggleChange(bool boolean)
    {
        soundSlider.onValueChanged.RemoveListener(OnSoundValueChange);
        if (boolean)
        {
            AudioManager.Instance.ChangeSoundValue(soundValue);
            soundSlider.value = soundValue;
        }
        else
        {
            AudioManager.Instance.ChangeSoundValue(0f);
            soundSlider.value = 0f;
        }
        soundSlider.onValueChanged.AddListener(OnSoundValueChange);
    }
    public void ChangeScreenMode()
    {
        isFullScreen = !isFullScreen;

        if (isFullScreen)
        {
            checkMark_FullScreen.SetActive(true);
            btn_FullScreen.interactable = false;
            checkMark_Windowed.SetActive(false);
            btn_Windowed.interactable = true;
#if UNITY_EDITOR
            Debug.Log("Screen.fullScreen = true");
#else
            Screen.fullScreen = true;
#endif
        }
        else
        {
            checkMark_FullScreen.SetActive(false);
            btn_FullScreen.interactable = true;
            checkMark_Windowed.SetActive(true);
            btn_Windowed.interactable = false;
#if UNITY_EDITOR
            Debug.Log("Screen.fullScreen = false");
#else
            Screen.fullScreen = false;
#endif
        }
    }
    public void ChangeScreenSize()
    {

    }

    
}
