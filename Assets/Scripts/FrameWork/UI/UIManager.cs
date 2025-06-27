using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public enum E_UILayout
    {
        BACK,
        MID,
        FRONT,
    }
    public class UIManager : Singleton<UIManager>
    {
        // Dictionary to store UI panels by their name
        private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

        // References to the main UI Canvas and EventSystem
        private GameObject uiCanvas;

        // Layout
        private GameObject uiLayout_Back;
        private GameObject uiLayout_Mid;
        private GameObject uiLayout_Front;

        // Private constructor to ensure singleton pattern
        private UIManager()
        {
            // Load and instantiate the UI Canvas if it hasn't been initialized
            if (uiCanvas == null)
            {
                ABResManager.Instance.LoadResAsync<GameObject>(abPanelName, "Canvas", (obj) =>
                {
                    uiCanvas = GameObject.Instantiate(obj);
                    uiCanvas.name = "Canvas";
                    GameObject.DontDestroyOnLoad(uiCanvas); // Prevents the Canvas from being destroyed between scenes
                }, true);

                ABResManager.Instance.LoadResAsync<GameObject>(abPanelName, "UILayout_Back", (obj) =>
                {
                    uiLayout_Back = GameObject.Instantiate(obj, uiCanvas.transform);
                    uiLayout_Back.name = "UILayout_Back";
                }, true);

                ABResManager.Instance.LoadResAsync<GameObject>(abPanelName, "UILayout_Mid", (obj) =>
                {
                    uiLayout_Mid = GameObject.Instantiate(obj, uiCanvas.transform);
                    uiLayout_Mid.name = "UILayout_Mid";
                }, true);

                ABResManager.Instance.LoadResAsync<GameObject>(abPanelName, "UILayout_Front", (obj) =>
                {
                    uiLayout_Front = GameObject.Instantiate(obj, uiCanvas.transform);
                    uiLayout_Front.name = "UILayout_Front";
                }, true);
            }
        }

        // Property that defines the asset bundle name for UI elements
        private string abPanelName
        {
            get
            {
                return "UI";
            }
        }

        // Show a UI panel by loading it asynchronously if not already loaded
        public void ShowPanel(string panelName,E_UILayout layout = E_UILayout.MID)
        {
            if (!panelDic.ContainsKey(panelName))
            {
                
                ABResManager.Instance.LoadResAsync<GameObject>(abPanelName, panelName, (obj) =>
                {
                    // Instantiate the panel under the UI Canvas

                    GameObject panelObj = null;

                    switch(layout)
                    {
                        case E_UILayout.BACK:
                            panelObj = GameObject.Instantiate(obj, uiLayout_Back.transform);
                            break;
                        case E_UILayout.MID:
                            panelObj = GameObject.Instantiate(obj, uiLayout_Mid.transform);
                            break;
                        case E_UILayout.FRONT:
                            panelObj = GameObject.Instantiate(obj, uiLayout_Front.transform);
                            break;
                    }

                    panelObj.name = panelName;

                    // Get the BasePanel component and store it in the dictionary
                    BasePanel panel = panelObj.GetComponent<BasePanel>();
                    panelDic.Add(panelName, panel);
                }, true);
            }

            // Show the panel
            panelDic[panelName].ShowPanel();

            // SortLayerout
            panelDic[panelName].gameObject.transform.SetAsLastSibling();
        }

        // Hide a UI panel by calling its HidePanel method
        public void HidePanel(string panelName)
        {
            panelDic[panelName].HidePanel();
        }
    }
}