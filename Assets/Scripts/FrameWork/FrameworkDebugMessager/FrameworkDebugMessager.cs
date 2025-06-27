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
        // ��ȡ��ǰ�Ĵ���ʵ�������ߴ���һ���µĴ���
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
            EditorGUILayout.HelpBox("����!DebugMessager�ڷ�����״̬���޷���ʾ������Ϣ!", MessageType.Warning);
        // ���ô��ڵı���
        e_DebugType = (E_DebugType)EditorGUILayout.EnumPopup("ѡ���ע����", e_DebugType);

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
        // ��ȡ����
        Type type = obj.GetType();

        // ��ȡ�����ֶ�
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // ��ȡ�ֶ����ƺ�ֵ
            string fieldName = field.Name;
            object fieldValue = field.GetValue(obj);

            // ��ʾ�ֶ����ƺ�ֵ
            GUILayout.Label($"{fieldName}: {fieldValue}");
        }
    }

    private void DisplayGlobalManager()
    {
        e_GameMode = (E_GameMode)EditorGUILayout.EnumPopup("GlobalManagerģʽ", e_GameMode);
    }

    private void DisplayGameDataManager()
    {
        GameData currentGameData = GameDataManager.CurrentGame;
        string savingPath = GameDataManager.Instance.SavingPath;
        string isExist = System.IO.File.Exists(savingPath) ? "·��������" : "·���Ѵ���";

        GUILayout.Label("��ǰ�浵·��(��Save0Ϊ��):\n" + savingPath, EditorStyles.boldLabel);


        GUILayout.Label("��ǰ�浵·���������: " + isExist, EditorStyles.boldLabel);

        GUILayout.Label("��ǰ�浵��Ϣ:", EditorStyles.boldLabel);

        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("������״̬", MessageType.Warning);
            return;
        }

        if (currentGameData == null)
        {
            EditorGUILayout.HelpBox("�����ڴ浵��Ϣ", MessageType.Warning);
            return;
        }

        DisplayProperties(currentGameData);
    }

    private void DisplayPoolManager()
    {
        poolManager_ShowDebugInfo = EditorGUILayout.ToggleLeft("��ӡPoolManager�����Ϣ", poolManager_ShowDebugInfo);
        GUILayout.Label("��ǰ������Ϣ:", EditorStyles.boldLabel);

        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("������״̬", MessageType.Warning);
            return;
        }

        if (poolDic.Count == 0 || poolDic == null)
        {
            EditorGUILayout.HelpBox("����ز���������", MessageType.Info);
            return;
        }

        // ����һ��GUIStyle�����ÿ����ʽ
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = Texture2D.whiteTexture; // ���ñ���Ϊ��ɫ
        boxStyle.border = new RectOffset(5, 5, 5, 5); // ���ñ߿���
        boxStyle.padding = new RectOffset(10, 10, 10, 10); // �����ڲ����

        // ��ʼ����һ���ɹ�������
        GUILayout.BeginVertical(boxStyle);

        foreach (KeyValuePair<string, PoolData> poolEntry in poolDic)
        {
            // ��ȡ����
            string poolName = poolEntry.Key;
            PoolData poolData = poolEntry.Value;

            // �������ӵı���
            GUILayout.Label($"����: {poolName}", EditorStyles.boldLabel);
            GUILayout.Label($"��������: {poolData.Count}");
            // ʹ�÷�����ʾPoolData����������
            // DisplayProperties(poolData);
        }

        // �����������
        GUILayout.EndVertical();
    }

    private void DisplayEventManager()
    {
        eventManager_ShowDebugInfo = EditorGUILayout.ToggleLeft("��ӡEventManager�����Ϣ", eventManager_ShowDebugInfo);
        GUILayout.Label("��ע���¼���Ϣ:", EditorStyles.boldLabel);

        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("������״̬", MessageType.Warning);
            return;
        }

        if (eventDic.Count == 0 || eventDic == null)
        {
            EditorGUILayout.HelpBox("δע���κ��¼�", MessageType.Info);
            return;
        }

        // GUIStyle
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = Texture2D.whiteTexture; // ���ñ���Ϊ��ɫ
        boxStyle.border = new RectOffset(5, 5, 5, 5); // ���ñ߿���
        boxStyle.padding = new RectOffset(10, 10, 10, 10); // �����ڲ����

        GUILayout.BeginVertical(boxStyle);

        foreach (KeyValuePair<E_EventType, EventInfoBase> eventInfo in eventDic)
        {
            string eventName = eventInfo.Key.ToString();
            EventInfoBase eventData = eventInfo.Value;

            // ��ȡ �¼�����
            GUILayout.Label($"�¼�����: {eventName}", EditorStyles.boldLabel);
            // ��ȡ ���������� �� ��������Ϣ
            GUILayout.Label($"����������: {eventData.GetSubscriberCount()}");

            // 
            if(!folderBoolean.ContainsKey(eventInfo.Key))
            {
                folderBoolean.Add(eventInfo.Key, false);
            }

            // ʹ��Foldout����ʾ/�۵���������Ϣ
            folderBoolean[eventInfo.Key] = EditorGUILayout.Foldout(folderBoolean[eventInfo.Key], "��������Ϣ");

            if (folderBoolean[eventInfo.Key])
            {
                // ��ȡ��������Ϣ
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