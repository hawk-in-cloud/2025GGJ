using Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

public enum E_DebugType
{
    GlobalManager,
    GameDataManager,
    PoolManager,
    EventManager,
}

public class FrameworkDebugMessager : EditorWindow
{
    [MenuItem("Tools/FrameworkDebugMessager")]
    public static void ShowWindow()
    {
        // 获取当前的窗口实例，或者创建一个新的窗口
        FrameworkDebugMessager window = GetWindow<FrameworkDebugMessager>("Framework Debug Messager");
        window.Show();
    }

    private E_DebugType e_DebugType;

    // GlobalManager
    private E_GameMode e_GameMode
    {
        get { 
            return GlobalManager.GameMode;
        }
        set { 
            GlobalManager.GameMode = value;
        }
    }
    // PoolManager
    private bool poolManager_ShowDebugInfo
    {
        get
        {
            return PoolManager.Instance.ShowDebugInfo;
        }
        set
        {
            PoolManager.Instance.ShowDebugInfo = value;
        }
    }
    private Dictionary<string, PoolData> poolDic
    {
        get
        {
            return PoolManager.Instance.PoolDic;
        }
    }
    // EventManager
    private bool eventManager_ShowDebugInfo
    {
        get
        {
            return EventManager.Instance.ShowDebugInfo;
        }
        set
        {
            EventManager.Instance.ShowDebugInfo = value;
        }
    }
    private Dictionary<E_EventType, EventInfoBase> eventDic
    {
        get
        {
            return EventManager.Instance.EventDic;
        }
    }

    private Dictionary<E_EventType, bool> folderBoolean = new Dictionary<E_EventType, bool>();

    private void OnGUI()
    {
        if (!EditorApplication.isPlaying)
            EditorGUILayout.HelpBox("警告!DebugMessager在非运行状态下无法显示调试信息!", MessageType.Warning);
        // 设置窗口的标题
        e_DebugType = (E_DebugType)EditorGUILayout.EnumPopup("选择关注对象", e_DebugType);

        switch(e_DebugType)
        {
            case E_DebugType.GlobalManager:
                DisplayGlobalManager();
                break;
            case E_DebugType.GameDataManager:
                DisplayGameDataManager();
                break;
            case E_DebugType.PoolManager:
                DisplayPoolManager();
                break;
            case E_DebugType.EventManager:
                DisplayEventManager();
                break;
        }
    }

    private void DisplayProperties(object obj)
    {
        // 获取类型
        Type type = obj.GetType();

        // 获取所有字段
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // 获取字段名称和值
            string fieldName = field.Name;
            object fieldValue = field.GetValue(obj);

            // 显示字段名称和值
            GUILayout.Label($"{fieldName}: {fieldValue}");
        }
    }

    private void DisplayGlobalManager()
    {
        e_GameMode = (E_GameMode)EditorGUILayout.EnumPopup("GlobalManager模式", e_GameMode);
    }

    private void DisplayGameDataManager()
    {
        GameData currentGameData = GameDataManager.CurrentGame;
        string savingPath = GameDataManager.Instance.SavingPath;
        string isExist = System.IO.File.Exists(savingPath) ? "路径不存在" : "路径已创建";

        GUILayout.Label("当前存档路径(以Save0为例):\n" + savingPath, EditorStyles.boldLabel);


        GUILayout.Label("当前存档路径存在情况: " + isExist, EditorStyles.boldLabel);

        GUILayout.Label("当前存档信息:", EditorStyles.boldLabel);

        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("非运行状态", MessageType.Warning);
            return;
        }

        if (currentGameData == null)
        {
            EditorGUILayout.HelpBox("不存在存档信息", MessageType.Warning);
            return;
        }

        DisplayProperties(currentGameData);
    }

    private void DisplayPoolManager()
    {
        poolManager_ShowDebugInfo = EditorGUILayout.ToggleLeft("打印PoolManager相关信息", poolManager_ShowDebugInfo);
        GUILayout.Label("当前池子信息:", EditorStyles.boldLabel);

        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("非运行状态", MessageType.Warning);
            return;
        }

        if (poolDic.Count == 0 || poolDic == null)
        {
            EditorGUILayout.HelpBox("对象池不存在内容", MessageType.Info);
            return;
        }

        // 创建一个GUIStyle来设置框的样式
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = Texture2D.whiteTexture; // 设置背景为白色
        boxStyle.border = new RectOffset(5, 5, 5, 5); // 设置边框厚度
        boxStyle.padding = new RectOffset(10, 10, 10, 10); // 设置内部填充

        // 开始创建一个可滚动区域
        GUILayout.BeginVertical(boxStyle);

        foreach (KeyValuePair<string, PoolData> poolEntry in poolDic)
        {
            // 获取池名
            string poolName = poolEntry.Key;
            PoolData poolData = poolEntry.Value;

            // 创建池子的标题
            GUILayout.Label($"池子: {poolName}", EditorStyles.boldLabel);
            GUILayout.Label($"对象数量: {poolData.Count}");
            // 使用反射显示PoolData的其他属性
            // DisplayProperties(poolData);
        }

        // 结束框架区域
        GUILayout.EndVertical();
    }

    private void DisplayEventManager()
    {
        eventManager_ShowDebugInfo = EditorGUILayout.ToggleLeft("打印EventManager相关信息", eventManager_ShowDebugInfo);
        GUILayout.Label("已注册事件信息:", EditorStyles.boldLabel);

        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("非运行状态", MessageType.Warning);
            return;
        }

        if (eventDic.Count == 0 || eventDic == null)
        {
            EditorGUILayout.HelpBox("未注册任何事件", MessageType.Info);
            return;
        }

        // GUIStyle
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = Texture2D.whiteTexture; // 设置背景为白色
        boxStyle.border = new RectOffset(5, 5, 5, 5); // 设置边框厚度
        boxStyle.padding = new RectOffset(10, 10, 10, 10); // 设置内部填充

        GUILayout.BeginVertical(boxStyle);

        foreach (KeyValuePair<E_EventType, EventInfoBase> eventInfo in eventDic)
        {
            string eventName = eventInfo.Key.ToString();
            EventInfoBase eventData = eventInfo.Value;

            // 获取 事件名称
            GUILayout.Label($"事件名称: {eventName}", EditorStyles.boldLabel);
            // 获取 订阅者数量 与 订阅者信息
            GUILayout.Label($"订阅者数量: {eventData.GetSubscriberCount()}");

            // 
            if(!folderBoolean.ContainsKey(eventInfo.Key))
            {
                folderBoolean.Add(eventInfo.Key, false);
            }

            // 使用Foldout来显示/折叠订阅者信息
            folderBoolean[eventInfo.Key] = EditorGUILayout.Foldout(folderBoolean[eventInfo.Key], "订阅者信息");

            if (folderBoolean[eventInfo.Key])
            {
                // 获取订阅者信息
                List<string> strs = eventData.GetSubscriberInfo();
                foreach (string str in strs)
                {
                    GUILayout.Label(str);
                }
            }
        }

        GUILayout.EndVertical();
    }
} 