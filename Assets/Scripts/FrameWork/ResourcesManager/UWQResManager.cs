using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Framework
{
    public class UWQResManager : SingletonMono<UWQResManager>
    {
        /// <summary>
        /// Load resources using UnityWebRequest.
        /// </summary>
        /// <typeparam name="T">Type can only be string, byte[], Texture, or AssetBundle. Other types are currently not supported.</typeparam>
        /// <param name="path">Resource path, including the protocol (http, ftp, file).</param>
        /// <param name="callBack">Callback function invoked on successful loading.</param>
        /// <param name="failCallBack">Callback function invoked on loading failure.</param>
        public void LoadRes<T>(string path, UnityAction<T> callBack, UnityAction failCallBack) where T : class
        {
            StartCoroutine(ReallyLoadRes<T>(path, callBack, failCallBack));
        }

        private IEnumerator ReallyLoadRes<T>(string path, UnityAction<T> callBack, UnityAction failCallBack) where T : class
        {
            // Supported types: string, byte[], Texture, AssetBundle
            Type type = typeof(T);
            // Object used for loading
            UnityWebRequest req = null;
            if (type == typeof(string) ||
                type == typeof(byte[]))
                req = UnityWebRequest.Get(path);
            else if (type == typeof(Texture))
                req = UnityWebRequestTexture.GetTexture(path);
            else if (type == typeof(AssetBundle))
                req = UnityWebRequestAssetBundle.GetAssetBundle(path);
            else
            {
                failCallBack?.Invoke();
                yield break;
            }

            yield return req.SendWebRequest();
            // If loading is successful
            if (req.result == UnityWebRequest.Result.Success)
            {
                if (type == typeof(string))
                    callBack?.Invoke(req.downloadHandler.text as T);
                else if (type == typeof(byte[]))
                    callBack?.Invoke(req.downloadHandler.data as T);
                else if (type == typeof(Texture))
                    callBack?.Invoke(DownloadHandlerTexture.GetContent(req) as T);
                else if (type == typeof(AssetBundle))
                    callBack?.Invoke(DownloadHandlerAssetBundle.GetContent(req) as T);
            }
            else
                failCallBack?.Invoke();
            // Dispose of the UnityWebRequest object
            req.Dispose();
        }
    }
}