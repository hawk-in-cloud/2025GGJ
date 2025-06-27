using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// The integration for loading AB related resources can be tested by loading the corresponding resources through EditResMgr during development
    /// </summary>
    public class ABResManager : Singleton<ABResManager>
    {
        // If it is true, it will be loaded through EditResMgr. If it is false, it will be loaded through ABMgr AB package
        private bool isDebug
        {
            get
            {
                if(GlobalManager.GameMode == E_GameMode.DEBUG)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private ABResManager() { }

        public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false) where T : Object
        {
#if UNITY_EDITOR
            if (isDebug)
            {
                // We have customized the management method for resources in AB package, and the corresponding folder name is the package name
                T res = EditorResManager.Instance.LoadEditorRes<T>($"{abName}/{resName}");
                callBack?.Invoke(res as T);
            }
            else
            {
                ABManager.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
            }
#else
        ABManager.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
#endif
        }
    }
}