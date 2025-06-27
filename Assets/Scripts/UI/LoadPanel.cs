using UnityEngine;
using Framework;
using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadPanel : BasePanel
{
    private RectTransform rectTrans;
    private float easeDuration = 0.3f;
    private bool isAnimating = false;
    private bool isExpanded = false;
    // Close
    [SerializeField] private Button btn_Close;
    // SaveSlots
    // SaveSlots中：0号为NameText，1号为TimeText，2号为NullText
    [Header("存档相关组件")]
    [SerializeField] private TextMeshProUGUI[] slot01;
    [SerializeField] private TextMeshProUGUI[] slot02;
    [SerializeField] private TextMeshProUGUI[] slot03;
    [SerializeField] private Button btn_s1;
    [SerializeField] private Button btn_s2;
    [SerializeField] private Button btn_s3;
    [SerializeField] private Button btn_s1_del;
    [SerializeField] private Button btn_s2_del;
    [SerializeField] private Button btn_s3_del;
    [Header("变量")]
    public static bool isLoad = true;

    private void Awake()
    {
        // AnimationComponent
        rectTrans = GetComponent<RectTransform>();
        rectTrans.localScale = Vector3.zero;
        // Close
        btn_Close = transform.Find("Btn_Close").GetComponent<Button>();
        // TMP
        GameObject obj_slot01 = GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.name == "SaveSlot_01").gameObject;
        GameObject obj_slot02 = GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.name == "SaveSlot_02").gameObject;
        GameObject obj_slot03 = GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.name == "SaveSlot_03").gameObject;
        btn_s1 = obj_slot01.GetComponent<Button>();
        btn_s2 = obj_slot02.GetComponent<Button>();
        btn_s3 = obj_slot03.GetComponent<Button>();
        btn_s1_del = obj_slot01.transform.Find("Delete").GetComponent<Button>();
        btn_s2_del = obj_slot02.transform.Find("Delete").GetComponent<Button>();
        btn_s3_del = obj_slot03.transform.Find("Delete").GetComponent<Button>();
        slot01 = obj_slot01.GetComponentsInChildren<TextMeshProUGUI>();
        slot02 = obj_slot02.GetComponentsInChildren<TextMeshProUGUI>();
        slot03 = obj_slot03.GetComponentsInChildren<TextMeshProUGUI>();
        // AnimationData
        easeDuration = 0.5f;
        isAnimating = false;
        isExpanded = false;
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
            });
    }

    public override void ShowPanel()
    {
        if (isAnimating || isExpanded) return;
        isAnimating = true;

        rectTrans.DOScale(Vector3.one, easeDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isAnimating = false;
                isExpanded = true;
            });
        UpdatePanelInfo();
    }

    protected void AddPanelListener()
    {
        btn_Close.onClick.AddListener(HidePanel);
        btn_s1.onClick.AddListener(OnClickSlot01);
        btn_s2.onClick.AddListener(OnClickSlot02);
        btn_s3.onClick.AddListener(OnClickSlot03);
        btn_s1_del.onClick.AddListener(OnDeleteSlot01);
        btn_s2_del.onClick.AddListener(OnDeleteSlot02);
        btn_s3_del.onClick.AddListener(OnDeleteSlot03);
    }

    private void UpdatePanelInfo()
    {
        for (int i = 0; i < 3; i++)
        {
            GameData data = GameDataManager.Instance.LoadGame(i + 1);
            if (data != null)
            {
                switch (i)
                {
                    case 0:
                        btn_s1_del.gameObject.SetActive(true);

                        slot01[0].gameObject.SetActive(true);
                        slot01[0].text = "SaveTime: " + data.SaveTime;

                        slot01[1].gameObject.SetActive(false);
                        break;
                    case 1:
                        btn_s2_del.gameObject.SetActive(true);

                        slot02[0].gameObject.SetActive(true);
                        slot02[0].text = "SaveTime: " + data.SaveTime;

                        slot02[1].gameObject.SetActive(false);
                        break;
                    case 2:
                        btn_s3_del.gameObject.SetActive(true);

                        slot03[0].gameObject.SetActive(true);
                        slot03[0].text = "SaveTime: " + data.SaveTime;

                        slot03[1].gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        slot01[0].gameObject.SetActive(false);
                        slot01[1].gameObject.SetActive(true);
                        btn_s1_del.gameObject.SetActive(false);
                        break;
                    case 1:
                        slot02[0].gameObject.SetActive(false);
                        slot02[1].gameObject.SetActive(true);
                        btn_s2_del.gameObject.SetActive(false);
                        break;
                    case 2:
                        slot03[0].gameObject.SetActive(false);
                        slot03[1].gameObject.SetActive(true);
                        btn_s3_del.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }

    private void OnDeleteSlot01()
    {
        GameDataManager.Instance.DeleteGame(1);
        UpdatePanelInfo();
    }

    private void OnDeleteSlot02()
    {
        GameDataManager.Instance.DeleteGame(2);
        UpdatePanelInfo();
    }

    private void OnDeleteSlot03()
    {
        GameDataManager.Instance.DeleteGame(3);
        UpdatePanelInfo();
    }

    private void OnClickSlot01()
    {
        if (isLoad)
        {
            GameDataManager.Instance.LoadGame(1,true);
            SceneManager.LoadScene(1);
            HidePanel();
        }
        else
        {
            GameDataManager.Instance.SaveGame(new GameData(1), 1);
            GameDataManager.Instance.LoadGame(1, true);
            SceneManager.LoadScene(1);
            HidePanel();
        }
    }
    private void OnClickSlot02()
    {
        if (isLoad)
        {
            GameDataManager.Instance.LoadGame(2, true);
            SceneManager.LoadScene(2);
            HidePanel();
        }
        else
        {
            GameDataManager.Instance.SaveGame(new GameData(2), 2);
            GameDataManager.Instance.LoadGame(2, true);
            SceneManager.LoadScene(2);
            HidePanel();
        }
    }
    private void OnClickSlot03()
    {
        if (isLoad)
        {
            GameDataManager.Instance.LoadGame(3, true);
            SceneManager.LoadScene(3);
            HidePanel();
        }
        else
        {
            GameDataManager.Instance.SaveGame(new GameData(3), 3);
            GameDataManager.Instance.LoadGame(3, true);
            SceneManager.LoadScene(3);
            HidePanel();
        }
    }
}
